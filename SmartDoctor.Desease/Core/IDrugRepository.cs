using SmartDoctor.Data.Models;
using System.Threading.Tasks;

namespace SmartDoctor.Medical.Core
{
    public interface IDrugRepository
    {
        Task<Rootobject> GetDrugs();
    }
}
