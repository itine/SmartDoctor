
using SmartDoctor.Testing.Models;
using System.Threading.Tasks;

namespace SmartDoctor.Desease.Core
{
    public interface IDiseaseRepository
    {
        Task<Diseases> DiseaseDiagnostic(long answerId);
        Task<string> GetDeseaseNameById(int diseaseId);
    }
}
