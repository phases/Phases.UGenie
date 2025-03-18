using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Cms.Web.Common.Controllers;
using Phases.UmbracoGenie.Models.Dtos;
using Phases.UmbracoGenie.Services.Interfaces;

namespace Phases.UmbracoGenie.Controllers
{
    public class GenieController : UmbracoAuthorizedApiController
    {
        private readonly IGenieService _genieService;

        public GenieController(IGenieService genieService)
        {
            _genieService = genieService;
        }

        [HttpPost]
        [Route("/umbraco/backoffice/api/Genie/SaveConfiguration")]
        public async Task<IActionResult> SaveConfiguration([FromBody] GenieConfigurationDto configuration)
        {
            if (configuration == null)
                return BadRequest("Configuration cannot be null");

            var result = await _genieService.SaveConfigurationAsync(configuration);

            if (result)
                return Ok(new { success = true, message = "Configuration saved successfully" });

            return BadRequest(new { success = false, message = "Failed to save configuration" });
        }

        [HttpGet]
        [Route("/umbraco/backoffice/api/Genie/GetConfiguration")]
        public async Task<IActionResult> GetConfiguration()
        {
            try
            {
                var config = await _genieService.GetConfigurationAsync();
                return Ok(config);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Failed to load configuration" });
            }
        }
    }
}
