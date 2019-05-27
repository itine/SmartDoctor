using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartDoctor.Data.Consts;
using SmartDoctor.Data.Models;
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
                return Json(new { Success = false, Data = exception.Message });
            }
        }

        [HttpPost(Scope.PassTheTest)]
        public async Task<IActionResult> PassTheTest([FromBody] AnswerModel model)
        {
            try
            {
                var id = await _testRepository.PassTest(model);
                return Json(
                    new
                    {
                        Success = true,
                        Data = id
                    });
            }
            catch (Exception exception)
            {
                return Json(new { Success = false, Data = exception.Message });
            }
        }

        [HttpPost(Scope.EvaluateAnswer)]
        public async Task<IActionResult> EvaluateAnswer([FromBody] long id)
        {
            try
            {
                await _testRepository.EvaluateAnswer(id);
                return Json(
                      new
                      {
                          Success = true,
                          Data = "ok"
                      });
            }
            catch (Exception exception)
            {
                return Json(new { Success = false, Data = exception.Message });
            }
        }

        [HttpPost(Scope.IncludeTestToCalculations)]
        public async Task<IActionResult> IncludeTestToCalculations([FromBody]int id)
        {
            try
            {
                await _testRepository.IncludeTestToCalculations(id);
                return Json(
                      new
                      {
                          Success = true,
                          Data = "ok"
                      });
            }
            catch (Exception exception)
            {
                return Json(new { Success = false, Data = exception.Message });
            }
        }

        [HttpPost(Scope.CheckNotViewedAnswer)]
        public async Task<IActionResult> CheckNotViewedAnswer([FromBody]int userId)
        {
            try
            {
                
                return Json(
                      new
                      {
                          Success = true,
                          Data = await _testRepository.CheckNotViewedAnswer(userId)
                       });
            }
            catch (Exception exception)
            {
                return Json(new { Success = false, Data = exception.Message });
            }
        }

        [HttpGet(Scope.GetPreDiseaseId)]
        public async Task<IActionResult> GetPreDiseaseId(int userId)
        {
            try
            {

                return Json(
                      new
                      {
                          Success = true,
                          Data = await _testRepository.GetPreDiseaseId(userId)
                      });
            }
            catch (Exception exception)
            {
                return Json(new { Success = false, Data = exception.Message });
            }
        }

        [HttpPost(Scope.RemoveAnswer)]
        public async Task<IActionResult> RemoveAnswer([FromBody] int userId)
        {
            try
            {
                await _testRepository.RemoveAnswer(userId);
                return Json(
                      new
                      {
                          Success = true,
                          Data = "ok"
                      });
            }
            catch (Exception exception)
            {
                return Json(new { Success = false, Data = exception.Message });
            }
        }

        [HttpGet(Scope.GetPatientsWithNoReception)]
        public IActionResult GetPatientsWithNoReception()
        {
            try
            {

                return Json(
                      new
                      {
                          Success = true,
                          Data = JsonConvert.SerializeObject(_testRepository.GetPatientsWithNoReception())
                      });
            }
            catch (Exception exception)
            {
                return Json(new { Success = false, Data = exception.Message });
            }
        }
    }
}