using SmartDoctor.Data.ContextModels;
using SmartDoctor.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDoctor.Testing.Core
{
    public interface ITestRepository
    {
        Task<IEnumerable<Questions>> GetQuestions();
        Task<long> PassTest(AnswerModel answer);
        Task<IEnumerable<Answers>> GetAnswers();
        Task EvaluateAnswer(long answerId);
        Task IncludeTestToCalculations(long answerId);
        Task<bool> CheckNotViewedAnswer(long userId);
        Task<long> GetNotViewedAnswer(long patientId);
        Task RemoveAnswer(long userId);
        Task<long> GetPreDiseaseId(long userId);
        long[] GetPatientsWithNoReception();
    }
}
