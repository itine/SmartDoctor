using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using SmartDoctor.Data.JsonModels;
using SmartDoctor.Data.Models;
using SmartDoctor.Helper;
using SmartDoctor.Web.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDoctor.Web.Controllers
{
    public class OutpatientController : Controller
    {
        private readonly IControllerRepository _controllerRepository;
        public OutpatientController(IControllerRepository controllerRepository)
        {
            _controllerRepository = controllerRepository;
        }

        public async Task<IActionResult> Index()
        {
            var medicalResponse = JsonConvert.DeserializeObject<MksResponse>(
              await RequestExecutor.ExecuteRequestAsync(
                  MicroservicesEnum.Medical, RequestUrl.GetOutpatients));
            if (!medicalResponse.Success)
                throw new Exception(medicalResponse.Data);
            var outpatients = JsonConvert.DeserializeObject<List<OutpatientModel>>(medicalResponse.Data);
            return View(outpatients);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(CardStatusModel model)
        {
            var medicalResponse = JsonConvert.DeserializeObject<MksResponse>(
               await RequestExecutor.ExecuteRequestAsync(
                   MicroservicesEnum.Medical, RequestUrl.ChangeCardStatus, new Parameter[]{
                           new Parameter("model", JsonConvert.SerializeObject(model), ParameterType.RequestBody)}));
            if (!medicalResponse.Success)
                throw new Exception(medicalResponse.Data);
            return RedirectToAction("Index", "Outpatient");
        }
    }
}