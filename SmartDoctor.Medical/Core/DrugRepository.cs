using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using SmartDoctor.Data.Models;
using SmartDoctor.Helper;

namespace SmartDoctor.Medical.Core
{
    public class DrugRepository : IDrugRepository
    {
        public async Task<List<Drug>> GetDrugs()
        {
            var actualDrugs = await RequestExecutor.ExecuteExternalRequestAsync("/drugs", Method.GET);
            return JsonConvert.DeserializeObject<List<Drug>>(actualDrugs);
        }
    }
}
