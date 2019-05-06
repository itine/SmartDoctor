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
using SmartDoctor.Data.Models;
using SmartDoctor.Helper;
using SmartDoctor.Testing.Models;

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
                .Where( x=> x.IsTakenToCalculate.HasValue && x.IsTakenToCalculate.Value).ToListAsync();

        public async Task<IEnumerable<Questions>> GetQuestions()
        {
            var questions = await _context.Questions.ToListAsync();
            if (!questions.Any())
                throw new Exception("Questions not found");
            return questions;
        }

        public async Task<long> PassTest(AnswerModel answerModel)
        {
            var answerData = string.Join(';', answerModel.Answers);
            var answer = new Answers
            {
                AnswerData = answerData,
                AnswerDate = DateTime.UtcNow,
                DataSetName = answerModel.AnswerName,
                IsTakenToCalculate = false,
                PatientId = answerModel.PatientId
            };
            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();
            return answer.AnswerId;
        }

        public async Task EvaluateAnswer(Answers answer)
        {
            answer = await _context.Answers.LastOrDefaultAsync();
            var data = new DataTable("Define the disease");
            var questions = (await GetQuestions()).ToArray();
            var answers = await GetAnswers();
            foreach (var question in questions)
                data.Columns.Add(new DataColumn(question.Text, typeof(string)));
            data.Columns.Add(new DataColumn("Diagnosed Disease", typeof(string)));
            foreach(var trainningAnswer in answers)
            {
                var answerArr = trainningAnswer.AnswerData.Split(';');
                if (answerArr.Length != questions.Length)
                    throw new Exception("Answers count must be equals questions count");
                var patient = data.NewRow();
                for (var i = 0; i < answerArr.Length; i++)
                    patient[questions[i].Text] = answerArr[i];
                var diseaseResponse = await RequestExecutor.ExecuteRequestAsync(
                    MicroservicesEnum.Disease, RequestUrl.GetDiseaseNameById,
                        new Parameter[] {
                            new Parameter("diseaseId", (int)trainningAnswer.DeseaseId.Value, ParameterType.RequestBody)
                        });
                var diseaseName = JsonConvert.DeserializeObject<MksResponse>(diseaseResponse);
                patient["Diagnosed Disease"] = diseaseName.Data;
                data.Rows.Add(patient);
            }
            var codification = new Codification(data);
            var codifiedData = codification.Apply(data);
            int[][] input = codifiedData.ToJagged<int>(questions.Select(x => x.Text).ToArray());
            int[] predictions = codifiedData.ToArray<int>("Diagnosed Disease");
            var decisionTreeLearningAlgorithm = new ID3Learning { };
            var decisionTree = decisionTreeLearningAlgorithm.Learn(input, predictions);
            var answerArray = answer.AnswerData.Split(';');
            if (answerArray.Length != questions.Length)
                throw new Exception("Answers count must be equals questions count");
            var inputValues = new string [questions.Length, 2];
            for (var i = 0; i < answerArray.Length; i++)
            {
                inputValues[i, 0] = questions[i].Text;
                inputValues[i, 1] = answerArray[i];
            }
            var query = codification.Transform(inputValues);
            var result = decisionTree.Decide(query);
            var diagnosis = codification.Revert("Diagnosed Disease", result);

            Console.WriteLine($"Diagnosed disease: {diagnosis}"); 
        }
    }
}
