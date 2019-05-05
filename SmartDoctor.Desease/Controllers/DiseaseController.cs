﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartDoctor.Desease.Core;
using System;
using System.Threading.Tasks;

namespace SmartDoctor.Desease.Controllers
{
    //[Authorize]
    public class DiseaseController : Controller
    {
        private readonly IDiseaseRepository _diseaseRepository;
        public DiseaseController(IDiseaseRepository diseaseRepository)
        {
            _diseaseRepository = diseaseRepository;
        }

        [HttpGet("DiseaseDiagnostic")]
        public async Task<IActionResult> DiseaseDiagnostic()
        {
            try
            {
                return Json(
                      new
                      {
                          Success = true,
                          Data = JsonConvert.SerializeObject(await _diseaseRepository.DiseaseDiagnostic(2))
                      });
            }
            catch (Exception exception)
            {
                return Json(new { Success = false, exception.Message });
            }
        }
    }
}