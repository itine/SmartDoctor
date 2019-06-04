using SmartDoctor.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDoctor.Medical.Core
{
    public interface IDrugRepository
    {
        Task<List<Drug>> GetDrugs();
    }
}
