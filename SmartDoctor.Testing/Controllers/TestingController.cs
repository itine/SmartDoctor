using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartDoctor.Data.Models;
using SmartDoctor.Testing.Core;
using System;
using System.Threading.Tasks;

namespace SmartDoctor.Testing.Controllers
{
    [Authorize]
    public class TestingController : Controller
    {
        private readonly ITestRepository _testRepository;
        public TestingController(ITestRepository testRepository)
        {
            _testRepository = testRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetTest()
        {
            try
            {
                return Json(
                      new
                      {
                          Success = true,
                          Data = JsonConvert.SerializeObject(await _testRepository.GetTest())
                      });
            }
            catch (Exception exception)
            {
                return Json(new { Success = false, exception.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> PassTheTest(AnswerModel model)
        {
            try
            {
                await _testRepository.PassTest(model);
                return Json(
                      new
                      {
                          Success = true,
                          Data = "ok"
                      });
            }
            catch (Exception exception)
            {
                return Json(new { Success = false, exception.Message });
            }
        }
    }
}