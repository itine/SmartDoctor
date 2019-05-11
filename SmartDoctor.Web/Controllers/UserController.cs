using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using SmartDoctor.Data.ContextModels;
using SmartDoctor.Data.Enums;
using SmartDoctor.Data.JsonModels;
using SmartDoctor.Data.Models;
using SmartDoctor.Helper;
using SmartDoctor.Web.Core;

namespace SmartDoctor.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IControllerRepository _controllerRepository;
        public UserController(IControllerRepository controllerRepository)
        {
            _controllerRepository = controllerRepository;
        }

        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.Role = RoleTypes.None;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Patients()
        {
            var role = await _controllerRepository.InitRole(User);
            ViewBag.Role = role;
            if (role == RoleTypes.None)
                return RedirectToAction("Login", "User");
            if (role == RoleTypes.Patient)
                return RedirectToAction("Index", "Home");
            var userResponse = JsonConvert.DeserializeObject<MksResponse>(
               await RequestExecutor.ExecuteRequestAsync(
                   MicroservicesEnum.User, RequestUrl.GetPatients));
            var users = new List<PatientModel>();
            if (!userResponse.Success)
                throw new Exception(userResponse.Data);
            else
                users.AddRange(JsonConvert.DeserializeObject<IEnumerable<PatientModel>>(userResponse.Data));
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AuthModel model)
        {
            try
            {
                var userResponse = JsonConvert.DeserializeObject<MksResponse>(
                    await RequestExecutor.ExecuteRequestAsync(
                        MicroservicesEnum.User, RequestUrl.Authorize, new Parameter[] {
                            new Parameter("model", JsonConvert.SerializeObject(model), ParameterType.RequestBody)
                        }));
                var user = new Users();
                if (userResponse.Success)
                {
                    user = JsonConvert.DeserializeObject<Users>(userResponse.Data);
                    await Authenticate(user.UserId.ToString());
                    ViewBag.Role = (RoleTypes) user.Role;
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.Role = RoleTypes.None;
                ModelState.AddModelError("", "Uncorrect login or password");
            }
            catch (Exception exception)
            {
                return Json(new { Success = false, exception.Message });
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Logoff()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            ViewBag.Role = await _controllerRepository.InitRole(User);
            return RedirectToAction("Login", "User");
        }

        [HttpGet]
        public async Task<IActionResult> Registration()
        {
            ViewBag.Role = await _controllerRepository.InitRole(User);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registration(PatientModel model)
        {
            try
            {
                var userResponse = JsonConvert.DeserializeObject<MksResponse>(
                    await RequestExecutor.ExecuteRequestAsync(
                        MicroservicesEnum.User, RequestUrl.Registration, new Parameter[] {
                            new Parameter("model", model, ParameterType.RequestBody)
                        }));
                if (!userResponse.Success)
                    throw new Exception(userResponse.Data);
                await Authenticate(model.UserId);
                ViewBag.Role = await _controllerRepository.InitRole(User);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception exception)
            {
                return Json(new { Success = false, exception.Message });
            }
        }

        private async Task Authenticate(string userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userId)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePatient(PatientModel model)
        {
            try
            {
                var userResponse = JsonConvert.DeserializeObject<MksResponse>(
                  await RequestExecutor.ExecuteRequestAsync(
                      MicroservicesEnum.User, RequestUrl.UpdatePatientInfo, new Parameter[] {
                            new Parameter("model", JsonConvert.SerializeObject(model), ParameterType.RequestBody)
                      }));
                if (!userResponse.Success)
                    throw new Exception(userResponse.Data);
                return Json(
                      new
                      {
                          Success = true,
                          Data = "Ok"
                      });

            }
            catch (Exception exception)
            {
                return Json(new { Success = false, exception.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                var userResponse = JsonConvert.DeserializeObject<MksResponse>(
                   await RequestExecutor.ExecuteRequestAsync(
                       MicroservicesEnum.User, RequestUrl.RemoveUser, new Parameter[] {
                            new Parameter("userId", long.Parse(userId), ParameterType.RequestBody)
                       }));
                if (!userResponse.Success)
                    throw new Exception(userResponse.Data);
                return Json(new { Success = true, Message = "ok" });
            }
            catch (Exception exception)
            {
                return Json(new { Success = false, exception.Message });
            }
        }
    }
}