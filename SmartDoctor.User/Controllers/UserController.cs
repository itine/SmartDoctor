using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartDoctor.Data.Consts;
using SmartDoctor.Data.JsonModels;
using SmartDoctor.User.Core;

namespace SmartDoctor.User.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet(Scope.GetUsers)]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                return Json(
                      new
                      {
                          Success = true,
                          Data = JsonConvert.SerializeObject(await _userRepository.GetUsers())
                      });
            }
            catch (Exception exception)
            {
                return Json(new { Success = false, exception.Message });
            }
        }

        [HttpGet(Scope.GetPatients)]
        public async Task<IActionResult> GetPatients()
        {
            try
            {
                return Json(
                      new
                      {
                          Success = true,
                          Data = JsonConvert.SerializeObject(await _userRepository.GetPatients())
                      });
            }
            catch (Exception exception)
            {
                return Json(new { Success = false, exception.Message });
            }
        }

        [HttpPost(Scope.Authorize)]
        public async Task<IActionResult> Authorize(string phoneNumber, string password)
        {
            try
            {
                return Json(
                      new
                      {
                          Success = true,
                          Data = JsonConvert.SerializeObject(await _userRepository.Authorize(phoneNumber, password))
                      });
            }
            catch (Exception exception)
            {
                return Json(new { Success = false, exception.Message });
            }
        }

        [HttpPost(Scope.Registration)]
        public async Task<IActionResult> Registration([FromBody]PatientModel model)
        {
            try
            {
                await _userRepository.Registration(model);
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

        //[HttpPost(Scope.UpdatePatientInfo)]
        //public async Task<IActionResult> UpdatePatientInfo([FromBody]PatientModel model)
        //{
        //    try
        //    {
        //        await _userRepository.UpdateUserData(model);
        //        return Json(
        //              new
        //              {
        //                  Success = true,
        //                  Data = "ok"
        //              });
        //    }
        //    catch (Exception exception)
        //    {
        //        return Json(new { Success = false, exception.Message });
        //    }
        //}

        [HttpPost(Scope.RemoveUser)]
        public async Task<IActionResult> RemoveUser([FromBody]long userId)
        {
            try
            {
                await _userRepository.RemoveUser(userId);
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