﻿using bridges_structures_service.Controllers.HealthCheck.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace bridges_structures_service.Controllers.HealthCheck
{
    [Produces("application/json")]
    [Route("api/v1/[Controller]")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            string name = Assembly.GetEntryAssembly()?.GetName().Name;
            string assembly = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bridges_structures_service.dll");
            string version = FileVersionInfo.GetVersionInfo(assembly).FileVersion;

            return Ok(new HealthCheckModel
            {
                AppVersion = version,
                Name = name
            });
        }
    }
}