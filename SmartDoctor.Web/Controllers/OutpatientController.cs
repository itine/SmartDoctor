using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using SmartDoctor.Data.ContextModels;
using SmartDoctor.Data.Enums;
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
            return Json(new { Success = true, Message = "ok" });
        }

        [HttpGet]
        public async Task<IActionResult> PatientReception(PatientFioModel model)
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
            ViewBag.PatientId = patient.PatientId;
            ViewBag.PatientFio = patient.Fio;
            return View(patient);
        }

        [HttpPost]
        public async Task<IActionResult> AccessAnswer(long patientId)
        {
            try
            {
                var testingResponse = JsonConvert.DeserializeObject<MksResponse>(
                  await RequestExecutor.ExecuteRequestAsync(
                       MicroservicesEnum.Testing, RequestUrl.GetNotViewedAnswer, new Parameter[]
                       {
                            new Parameter("patientId", patientId, ParameterType.GetOrPost)
                       }));
                if (!testingResponse.Success)
                    throw new Exception(testingResponse.Data);
                var answerId = JsonConvert.DeserializeObject<long>(testingResponse.Data);
                var testingResponse2 = JsonConvert.DeserializeObject<MksResponse>(
                      await RequestExecutor.ExecuteRequestAsync(
                           MicroservicesEnum.Testing, RequestUrl.IncludeTestToCalculations, new Parameter[]
                           {
                                 new Parameter("id", answerId, ParameterType.RequestBody)
                           }));
                if (!testingResponse2.Success)
                    throw new Exception(testingResponse2.Data);
                //update outpatient
                var doctorId = _controllerRepository.GetUserId(User);
                var medicalResponse = JsonConvert.DeserializeObject<MksResponse>(
                    await RequestExecutor.ExecuteRequestAsync(
                        MicroservicesEnum.Medical, RequestUrl.GetOutpatientByPatientAndDoctorId, new Parameter[]{
                               new Parameter(
                                   "model", JsonConvert.SerializeObject(new DoctorPatientModel
                                   {
                                       DoctorId = doctorId,
                                       PatientId = patientId
                                   }), ParameterType.GetOrPost)
                          }));
                if (!medicalResponse.Success)
                    throw new Exception(medicalResponse.Data);
                var outpatient = JsonConvert.DeserializeObject<OutpatientModel>(medicalResponse.Data);
                var medicalResponse2 = JsonConvert.DeserializeObject<MksResponse>(
                   await RequestExecutor.ExecuteRequestAsync(
                       MicroservicesEnum.Medical, RequestUrl.ChangeCardStatus, new Parameter[]{
                               new Parameter("model", JsonConvert.SerializeObject(new CardStatusModel
                               {
                                   CardId = outpatient.OutpatientCardId,
                                   Status = (byte) OutpatientStatuses.DoctorVisit
                               }), ParameterType.RequestBody)}));
                if (!medicalResponse2.Success)
                    throw new Exception(medicalResponse2.Data);
                return Json(new
                {
                    Success = true,
                    Data = outpatient.OutpatientCardId
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Message  = ex.Message.ToString()
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DiscardAnswer(long patientId)
        {
            try
            {
                var testingResponse = JsonConvert.DeserializeObject<MksResponse>(
                      await RequestExecutor.ExecuteRequestAsync(
                           MicroservicesEnum.User, RequestUrl.GetUserByPatientId, new Parameter[]
                           {
                               new Parameter("patientId", patientId, ParameterType.GetOrPost)
                           }));
                if (!testingResponse.Success)
                    throw new Exception(testingResponse.Data);
                var user = JsonConvert.DeserializeObject<Users>(testingResponse.Data);
                var testingResponse2 = JsonConvert.DeserializeObject<MksResponse>(
                  await RequestExecutor.ExecuteRequestAsync(
                       MicroservicesEnum.Testing, RequestUrl.RemoveAnswer, new Parameter[]
                       {
                            new Parameter("userId", user.UserId, ParameterType.RequestBody)
                       }));
                if (!testingResponse2.Success)
                    throw new Exception(testingResponse2.Data);
                //update outpatient
                var doctorId = _controllerRepository.GetUserId(User);
                var medicalResponse = JsonConvert.DeserializeObject<MksResponse>(
                    await RequestExecutor.ExecuteRequestAsync(
                        MicroservicesEnum.Medical, RequestUrl.GetOutpatientByPatientAndDoctorId, new Parameter[]{
                               new Parameter(
                                   "model", JsonConvert.SerializeObject(new DoctorPatientModel
                                   {
                                       DoctorId = doctorId,
                                       PatientId = patientId
                                   }), ParameterType.GetOrPost)
                          }));
                if (!medicalResponse.Success)
                    throw new Exception(medicalResponse.Data);
                var outpatient = JsonConvert.DeserializeObject<OutpatientModel>(medicalResponse.Data);
                var medicalResponse2 = JsonConvert.DeserializeObject<MksResponse>(
                   await RequestExecutor.ExecuteRequestAsync(
                       MicroservicesEnum.Medical, RequestUrl.ChangeCardStatus, new Parameter[]{
                               new Parameter("model", JsonConvert.SerializeObject(new CardStatusModel
                               {
                                   CardId = outpatient.OutpatientCardId,
                                   Status = (byte) OutpatientStatuses.DoctorVisit
                               }), ParameterType.RequestBody)}));
                if (!medicalResponse2.Success)
                    throw new Exception(medicalResponse2.Data);
                return Json(new
                {
                    Success = true,
                    Data = outpatient.OutpatientCardId
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Message = ex.Message.ToString()
                });
            }
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

        [HttpPost]
        public async Task<IActionResult> UpdateDescription(CardDescriptionModel model)
        {
            var medicalResponse = JsonConvert.DeserializeObject<MksResponse>(
               await RequestExecutor.ExecuteRequestAsync(
                   MicroservicesEnum.Medical, RequestUrl.UpdateDescription, new Parameter[]{
                           new Parameter("model", JsonConvert.SerializeObject(model), ParameterType.RequestBody)}));
            if (!medicalResponse.Success)
                throw new Exception(medicalResponse.Data);
            return RedirectToAction("Index", "Outpatient");
        }
    }
}