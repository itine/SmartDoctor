using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartDoctor.Data.Models;
using SmartDoctor.Helper;
using SmartDoctor.Testing.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDoctor.Web.Controllers
{
    public class TestController : Controller
    {
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
            ViewBag.PatientId = 1;
            return View(questions);
        }

        [HttpPost]
        public async Task<IActionResult> PassTheTest(AnswerModel model)
        {
            try
            {
                var testingResponse = JsonConvert.DeserializeObject<MksResponse>(
                   await RequestExecutor.ExecuteRequestAsync(
                       MicroservicesEnum.Testing, RequestUrl.PassTheTest));
                var questions = new List<Questions>();
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