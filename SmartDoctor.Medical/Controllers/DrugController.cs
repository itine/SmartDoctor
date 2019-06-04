using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartDoctor.Medical.Core;

namespace SmartDoctor.Medical.Controllers
{
    public class DrugController : Controller
    {
        private readonly IDrugRepository _drugRepository;
        public DrugController(IDrugRepository drugRepository)
        {
            _drugRepository = drugRepository;
        }

        [HttpGet("GetDrugs")]
        public async Task<IActionResult> GetDrugs()
        {
            try
            {
                var drugs = await _drugRepository.GetDrugs();
                return Json(
                      new
                      {
                          Success = true,
                          Data = JsonConvert.SerializeObject(drugs)
                      });
            }
            catch (Exception exception)
            {
                return Json(new { Success = false, exception.Message });
            }
        }
    }
}