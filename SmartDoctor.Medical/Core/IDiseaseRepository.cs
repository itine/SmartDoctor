using SmartDoctor.Data.ContextModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDoctor.Medical.Core
{
    public interface IDiseaseRepository
    {
        Task<Diseases> DiseaseDiagnostic(long answerId);
        Task<string> GetDiseaseNameById(long diseaseId);
        Task<long> GetDiseaseIdByName(string name);
        Task<IEnumerable<string>> GetAllDiseases();
    }
}
