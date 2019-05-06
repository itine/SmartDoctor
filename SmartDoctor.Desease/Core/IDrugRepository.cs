using SmartDoctor.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDoctor.Desease.Core
{
    public interface IDrugRepository
    {
        Task<Rootobject> GetDrugs();
    }
}
