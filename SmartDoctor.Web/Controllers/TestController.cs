using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using SmartDoctor.Data.ContextModels;
using SmartDoctor.Data.Models;
using SmartDoctor.Helper;
using SmartDoctor.Web.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDoctor.Web.Controllers
{
    public class TestController : Controller
    {
        private readonly IControllerRepository _controllerRepository;
        public TestController(IControllerRepository controllerRepository)
        {
            _controllerRepository = controllerRepository;
        }

        public IActionResult Index() => View();

        public async Task<IActionResult> GetTest()
        {
            var testingResponse = JsonConvert.DeserializeObject<MksResponse>(
                await RequestExecutor.ExecuteRequestAsync(
                    MicroservicesEnum.Testing, RequestUrl.GetQuestions));
            var questions = new List<Questions>();
            if (!testingResponse.Success)
                throw new Exception(testingResponse.Data);
            else
                questions.AddRange(JsonConvert.DeserializeObject<IEnumerable<Questions>>(testingResponse.Data));
            return View(questions);
        }

        [HttpPost]
        public async Task<IActionResult> PassTheTest(AnswerItem[] answers)
        {
            try
            {
                var userId = _controllerRepository.GetUserId(User);
                var testingResponse = JsonConvert.DeserializeObject<MksResponse>(
                   await RequestExecutor.ExecuteRequestAsync(
                       MicroservicesEnum.Testing, RequestUrl.PassTheTest, new Parameter[]{
                           new Parameter("model", new AnswerModel
                           {
                               UserId = userId,
                               Answers = answers
                           }, ParameterType.RequestBody)}));
                if (!testingResponse.Success)
                    throw new Exception(testingResponse.Data);
                return RedirectToAction("Index", "Test");
            }
            catch (Exception exception)
            {
                ModelState.AddModelError(nameof(exception), exception.ToString());
                throw;
            }
        }
    }
}