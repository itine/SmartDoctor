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
        Task EvaluateAnswer(Answers answer);
    }
}
