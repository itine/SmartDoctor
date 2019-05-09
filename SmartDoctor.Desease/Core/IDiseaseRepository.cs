
using SmartDoctor.Medical.Models;
using System.Threading.Tasks;

namespace SmartDoctor.Medical.Core
{
    public interface IDiseaseRepository
    {
        Task<Diseases> DiseaseDiagnostic(long answerId);
        Task<string> GetDeseaseNameById(int diseaseId);
    }
}
