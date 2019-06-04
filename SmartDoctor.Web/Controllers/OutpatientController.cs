using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using SmartDoctor.Data.JsonModels;
using SmartDoctor.Data.Models;
using SmartDoctor.Helper;
using SmartDoctor.Web.Core;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IActionResult> TakePatient()
        {
            var testingResponse = JsonConvert.DeserializeObject<MksResponse>(
              await RequestExecutor.ExecuteRequestAsync(
                  MicroservicesEnum.Testing, RequestUrl.GetPatientsWithNoReception));
            if (!testingResponse.Success)
                throw new Exception(testingResponse.Data);
            var ids = JsonConvert.DeserializeObject<long[]>(testingResponse.Data);
            var userResponse = JsonConvert.DeserializeObject<MksResponse>(
             await RequestExecutor.ExecuteRequestAsync(
                 MicroservicesEnum.User, RequestUrl.GetPatientsByIds, new Parameter[]{
                     new Parameter("ids", JsonConvert.SerializeObject(ids), ParameterType.RequestBody )
                 }));
            if (!userResponse.Success)
                throw new Exception(userResponse.Data);
            var outpatients = JsonConvert.DeserializeObject<List<PatientModel>>(userResponse.Data);
            ViewBag.Patients = outpatients.Select(x => x.Fio);
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CreateCard()
        {
            var userId = _controllerRepository.GetUserId(User);
            var testingResponse = JsonConvert.DeserializeObject<MksResponse>(
               await RequestExecutor.ExecuteRequestAsync(
                   MicroservicesEnum.Medical, RequestUrl.CreateOutpatientCard, new Parameter[]{
                           new Parameter("userId", userId, ParameterType.RequestBody) }));
            if (!testingResponse.Success)
                throw new Exception(testingResponse.Data);
            return Json(new { Success = true,  Message = "ok" });
        }

        [HttpPost]
        public async Task<IActionResult> ToReception(PatientFioModel model)
        {
            var userResponse = JsonConvert.DeserializeObject<MksResponse>(await RequestExecutor.ExecuteRequestAsync(
                  MicroservicesEnum.User, RequestUrl.GetPatientByFio,
                      new Parameter[] {
                            new Parameter("fio", model.Fio, ParameterType.GetOrPost)
                      }));
            if (!userResponse.Success)
                throw new Exception(userResponse.Data);
            var patient = JsonConvert.DeserializeObject<PatientModel>(userResponse.Data);
            var testingResponse = JsonConvert.DeserializeObject<MksResponse>(
               await RequestExecutor.ExecuteRequestAsync(
                   MicroservicesEnum.Testing, RequestUrl.GetPreDiseaseId, new Parameter[]{
                           new Parameter("userId", long.Parse(patient.UserId), ParameterType.GetOrPost)
                       }));
            if (!testingResponse.Success)
                throw new Exception(testingResponse.Data);
            var diseaseId = JsonConvert.DeserializeObject<long>(testingResponse.Data);
            var medicalResponse = JsonConvert.DeserializeObject<MksResponse>(
               await RequestExecutor.ExecuteRequestAsync(
                   MicroservicesEnum.Medical, RequestUrl.GetDiseaseNameById, new Parameter[]{
                           new Parameter(
                               "diseaseId", diseaseId, ParameterType.GetOrPost)
                       }));
            if (!medicalResponse.Success)
                throw new Exception(medicalResponse.Data);
            ViewBag.PreDiagnos = medicalResponse.Data;
            return RedirectToAction("PatientReception", "Outpatient", patient);
        }


        [HttpGet]
        public async Task<IActionResult> PatientReception(PatientModel model)
        {
            ViewBag.PreDiagnos = "test";
            return RedirectToAction("PatientReception", "Outpatient"); 
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