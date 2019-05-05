
using SmartDoctor.Testing.Models;
using System.Threading.Tasks;

namespace SmartDoctor.Desease.Core
{
    public interface IDiseaseRepository
    {
        Task<Deseases> DiseaseDiagnostic(long answerId);
        Task<Deseases> GetDiseaseById(int diseaseId);
    }
}
