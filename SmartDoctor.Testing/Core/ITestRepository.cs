using SmartDoctor.Testing.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDoctor.Testing.Core
{
    public interface ITestRepository
    {
        Task<IEnumerable<Questions>> GetTest();
    }
}
