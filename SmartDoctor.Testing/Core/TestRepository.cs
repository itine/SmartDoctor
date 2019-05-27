using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.Math;
using Accord.Statistics.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using SmartDoctor.Data.ContextModels;
using SmartDoctor.Data.Models;
using SmartDoctor.Helper;

namespace SmartDoctor.Testing.Core
{
    public class TestRepository : ITestRepository
    {
        private readonly SmartDoctor_TestDataContext _context;

        public TestRepository(SmartDoctor_TestDataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Answers>> GetAnswers() =>
            await _context.Answers
                .Where(x => x.IsTakenToCalculate.HasValue && x.IsTakenToCalculate.Value).ToListAsync();

        public async Task<IEnumerable<Questions>> GetQuestions()
        {
            var questions = await _context.Questions.ToListAsync();
            if (!questions.Any())
                throw new Exception("Questions not found");
            return questions;
        }

        public async Task<long> PassTest(AnswerModel answerModel)
        {
            var userResponse = await RequestExecutor.ExecuteRequestAsync(
                MicroservicesEnum.User, RequestUrl.GetPatientByUserId,
                    new Parameter[] {
                            new Parameter("userId", answerModel.UserId, ParameterType.GetOrPost)
                    });
            var patientData = JsonConvert.DeserializeObject<MksResponse>(userResponse);
            if (!patientData.Success)
                throw new Exception(patientData.Data);
            var patientCtx = JsonConvert.DeserializeObject<Patients>(patientData.Data);
            var stringArr = answerModel.Answers.Select(x => x.Answer);
            var answerData = string.Join(';', stringArr);
            var answer = new Answers
            {
                AnswerData = answerData,
                AnswerDate = DateTime.UtcNow,
                PatientId = patientCtx.PatientId
            };
            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();
            return answer.AnswerId;
        }

        public async Task EvaluateAnswer(long answerId)
        {
            var answer = await _context.Answers.FirstOrDefaultAsync(x => x.AnswerId == answerId);
            if (answer == null)
                throw new Exception("Answer not foung");
            var data = new DataTable("Define the disease");
            var questions = (await GetQuestions()).ToArray();
            var questionsLength = questions.Length;
            var answers = await GetAnswers();
            foreach (var question in questions)
                data.Columns.Add(new DataColumn(question.Text, typeof(string)));
            data.Columns.Add(new DataColumn("Age category", typeof(byte)));
            data.Columns.Add(new DataColumn("Gender", typeof(bool)));
            data.Columns.Add(new DataColumn("Diagnosed Disease", typeof(string)));
            foreach (var trainningAnswer in answers)
            {
                var answerArr = trainningAnswer.AnswerData.Split(';');
                if (answerArr.Length != questionsLength)
                    throw new Exception("Answers count must be equals questions count");
                var patient = data.NewRow();
                for (var i = 0; i < answerArr.Length; i++)
                    patient[questions[i].Text] = answerArr[i];
                var userResponse = await RequestExecutor.ExecuteRequestAsync(
                    MicroservicesEnum.User, RequestUrl.GetPatientById,
                        new Parameter[] {
                            new Parameter("patientId", (int)trainningAnswer.PatientId.Value, ParameterType.GetOrPost)
                        });
                var patientData = JsonConvert.DeserializeObject<MksResponse>(userResponse);
                if (!patientData.Success)
                    throw new Exception(patientData.Data);
                var patientCtx = JsonConvert.DeserializeObject<Patients>(patientData.Data);
                patient["Age category"] = (byte) new AgeLimit((byte)Math.Round((DateTime.UtcNow - patientCtx.DateBirth).TotalDays / 365.2425)).Limit;
                patient["Gender"] = patientCtx.Gender;
                var diseaseResponseName = await RequestExecutor.ExecuteRequestAsync(
                   MicroservicesEnum.Medical, RequestUrl.GetDiseaseNameById,
                       new Parameter[] {
                            new Parameter("diseaseId", 
                            trainningAnswer.DeseaseId.Value,
                            ParameterType.GetOrPost)
                       });
                var diseaseNameResponse = JsonConvert.DeserializeObject<MksResponse>(diseaseResponseName);
                if (!diseaseNameResponse.Success)
                    throw new Exception(diseaseNameResponse.Data);
                patient["Diagnosed Disease"] = JsonConvert.DeserializeObject<string>(diseaseNameResponse.Data);
                data.Rows.Add(patient);
            }
            var codification = new Codification(data);
            var codifiedData = codification.Apply(data);
            int[][] input = codifiedData.ToJagged<int>(questions.Select(x => x.Text).ToArray());
            int[] predictions = codifiedData.ToArray<int>("Diagnosed Disease");
            var decisionTreeLearningAlgorithm = new ID3Learning { };
            var decisionTree = decisionTreeLearningAlgorithm.Learn(input, predictions);
            var answerArray = answer.AnswerData.Split(';');
            if (answerArray.Length != questionsLength)
                throw new Exception("Answers count must be equals questions count");
            var inputValues = new string[questions.Length, 2];
            for (var i = 0; i < answerArray.Length; i++)
            {
                inputValues[i, 0] = questions[i].Text;
                inputValues[i, 1] = answerArray[i];
            }
            var query = codification.Transform(inputValues);
            var result = decisionTree.Decide(query);
            var diagnosis = codification.Revert("Diagnosed Disease", result);
            var diseaseIdResponse = await RequestExecutor.ExecuteRequestAsync(
                   MicroservicesEnum.Medical, RequestUrl.GetDiseaseIdByName,
                       new Parameter[] {
                            new Parameter("name", diagnosis, ParameterType.GetOrPost)
                       });
            var diseaseResponseId = JsonConvert.DeserializeObject<MksResponse>(diseaseIdResponse);
            if (!diseaseResponseId.Success)
                throw new Exception(diseaseResponseId.Data);
            answer.DeseaseId = long.Parse(diseaseResponseId.Data);
            await _context.SaveChangesAsync();
        }

        public async Task IncludeTestToCalculations(long answerId)
        {
            var answer = await _context.Answers.FirstOrDefaultAsync(x => x.AnswerId == answerId);
            if (answer == null)
                throw new Exception("Answer not foung");
            answer.IsTakenToCalculate = true;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckNotViewedAnswer(long userId)
        {
            var userResponse = await RequestExecutor.ExecuteRequestAsync(
                   MicroservicesEnum.User, RequestUrl.GetPatientByUserId,
                       new Parameter[] {
                            new Parameter("userId", userId, ParameterType.GetOrPost)
                       });
            var patientData = JsonConvert.DeserializeObject<MksResponse>(userResponse);
            if (!patientData.Success)
                throw new Exception(patientData.Data);
            var patientCtx = JsonConvert.DeserializeObject<Patients>(patientData.Data);
            return await _context.Answers.AnyAsync(x => x.PatientId.HasValue
                && x.PatientId.Value == patientCtx.PatientId && !x.IsTakenToCalculate.HasValue);
        }

        public async Task RemoveAnswer(long userId)
        {
            var userResponse = await RequestExecutor.ExecuteRequestAsync(
                  MicroservicesEnum.User, RequestUrl.GetPatientByUserId,
                      new Parameter[] {
                            new Parameter("userId", userId, ParameterType.GetOrPost)
                      });
            var patientData = JsonConvert.DeserializeObject<MksResponse>(userResponse);
            if (!patientData.Success)
                throw new Exception(patientData.Data);
            var patientCtx = JsonConvert.DeserializeObject<Patients>(patientData.Data);
            var answer = await _context.Answers.FirstOrDefaultAsync(x => !x.IsTakenToCalculate.HasValue
                && x.PatientId.HasValue && x.PatientId == patientCtx.PatientId);
            if (answer != null)
            {
                _context.Answers.Remove(answer);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<long> GetPreDiseaseId(long userId)
        {
            var userResponse = await RequestExecutor.ExecuteRequestAsync(
                  MicroservicesEnum.User, RequestUrl.GetPatientByUserId,
                      new Parameter[] {
                            new Parameter("userId", userId, ParameterType.GetOrPost)
                      });
            var patientData = JsonConvert.DeserializeObject<MksResponse>(userResponse);
            if (!patientData.Success)
                throw new Exception(patientData.Data);
            var patientCtx = JsonConvert.DeserializeObject<Patients>(patientData.Data);
            var answer = await _context.Answers.FirstOrDefaultAsync(x => !x.IsTakenToCalculate.HasValue
              && x.PatientId.HasValue && x.PatientId == patientCtx.PatientId && x.DeseaseId.HasValue);
            if (answer == null)
                throw new Exception("answer not found");
            return answer.DeseaseId.Value;
        }

        public long[] GetPatientsWithNoReception() =>
            _context.Answers.Where(x => !x.IsTakenToCalculate.HasValue).Select(x => x.PatientId.Value).Distinct().ToArray();


    }
}
