using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using SmartDoctor.Data.Models;
using SmartDoctor.Helper;

namespace SmartDoctor.Desease.Core
{
    public class DrugRepository : IDrugRepository
    {
        public async Task<Rootobject> GetDrugs()
        {
            var a = await RequestExecutor.ExecuteExternalRequestAsync("", Method.GET);
            return JsonConvert.DeserializeObject<Rootobject>(a);
        }
    }
}
