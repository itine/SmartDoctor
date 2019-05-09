using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartDoctor.Data.Consts;
using SmartDoctor.Data.Models;
using SmartDoctor.Helper;
using SmartDoctor.Testing.Core;
using System;
using System.Threading.Tasks;

namespace SmartDoctor.Testing.Controllers
{
    public class TestingController : Controller
    {
        private readonly ITestRepository _testRepository;
        public TestingController(ITestRepository testRepository)
        {
            _testRepository = testRepository;
        }

        [HttpGet(Scope.GetQuestions)]
        public async Task<IActionResult> GetQuestions()
        {
            try
            {
                return Json(
                      new
                      {
                          Success = true,
                          Data = JsonConvert.SerializeObject(await _testRepository.GetQuestions())
                      });
            }
            catch (Exception exception)
            {
                return Json(new { Success = false, exception.Message });
            }
        }

        [HttpPost(Scope.PassTheTest)]
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

        [HttpPost(Scope.EvaluateAnswer)]
        public async Task<IActionResult> EvaluateAnswer(int id)
        {
            try
            {
                await _testRepository.EvaluateAnswer(new Models.Answers { });
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