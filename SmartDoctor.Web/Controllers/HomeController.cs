using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartDoctor.Data.Models;
using SmartDoctor.Helper;
using SmartDoctor.Web.Models;

namespace SmartDoctor.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Drugs()
        {
            var medicalResponse = JsonConvert.DeserializeObject<MksResponse>(
                await RequestExecutor.ExecuteRequestAsync(
                    MicroservicesEnum.Medical, RequestUrl.GetDrugs));
            if (!medicalResponse.Success)
                throw new Exception(medicalResponse.Data);
            var drugs = JsonConvert.DeserializeObject<List<Drug>>(medicalResponse.Data);
            return View(drugs);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
