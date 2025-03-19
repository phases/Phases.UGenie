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
using Microsoft.Extensions.Logging;

namespace Phases.UmbracoGenie.Controllers
{
    public class GenieController : UmbracoAuthorizedApiController
    {
        private readonly IGenieService _genieService;
        private readonly ILogger<GenieController> _logger;

        public GenieController(IGenieService genieService, ILogger<GenieController> logger)
        {
            _genieService = genieService;
            _logger = logger;
        }

        [HttpPost]
        [Route("/umbraco/backoffice/api/Genie/SaveConfiguration")]
        public async Task<IActionResult> SaveConfiguration([FromBody] GenieConfigurationDto configuration)
        {
            if (configuration == null)
            {
                _logger.LogWarning("SaveConfiguration called with null configuration");
                return BadRequest("Configuration cannot be null");
            }
            _logger.LogInformation("Saving configuration...");
            var result = await _genieService.SaveConfigurationAsync(configuration);

            if (result)
            {
                _logger.LogInformation("Configuration saved successfully");
                return Ok(new { success = true, message = "Configuration saved successfully" });
            }

            _logger.LogError("Failed to save configuration");
            return BadRequest(new { success = false, message = "Failed to save configuration" });
        }

        [HttpGet]
        [Route("/umbraco/backoffice/api/Genie/GetConfiguration")]
        public async Task<IActionResult> GetConfiguration()
        {
            try
            {
                _logger.LogInformation("Fetching configuration...");
                var config = await _genieService.GetConfigurationAsync();
                _logger.LogInformation("Configuration fetched successfully");
                return Ok(config);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load configuration");
                return BadRequest(new { success = false, message = "Failed to load configuration" });
            }
        }

        [HttpGet]
        [Route("/umbraco/backoffice/api/Genie/IsGenieTextBoxEnabled")]
        public async Task<IActionResult> IsGenieTextBoxEnabled()
        {
            try
            {
                return Ok(await _genieService.GetIsGenieTextBoxEnabled());
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Failed to load configuration");
                return BadRequest(new { success = false, message = "Failed to load configuration" });
            }
        }

        [HttpGet]
        [Route("/umbraco/backoffice/api/Genie/IsGenieTextAreaEnabled")]
        public async Task<IActionResult> IsGenieTextAreaEnabled()
        {
            try
            {
                return Ok(await _genieService.GetIsGenieTextAreaEnabled());
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "Failed to load configuration");
                return BadRequest(new { success = false, message = "Failed to load configuration" });
            }
        }
    }
}
