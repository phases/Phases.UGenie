using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Persistence;
using Phases.UmbracoGenie.Models.Dtos;
using Phases.UmbracoGenie.Repositories.Interfaces;
using Phases.UmbracoGenie.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Phases.UmbracoGenie.Services
{
    public class GenieService : IGenieService
    {
        private readonly IGenieRepository _repository;
         private readonly ILogger<GenieService> _logger;

        public GenieService(IGenieRepository repository, ILogger<GenieService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<GenieConfigurationDto> GetConfigurationAsync()
        {
            try
            {
                var config = await _repository.GetConfigurationAsync();
                return config ?? new GenieConfigurationDto
                {
                    TextModels = new List<TextModelDto>(),
                    ImageModels = new List<ImageModelDto>()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetConfigurationAsync");
                throw;
            }
        }

        public async Task<bool> SaveConfigurationAsync(GenieConfigurationDto configuration)
        {
            try
            {
                return await _repository.SaveConfigurationAsync(configuration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in SaveConfigurationAsync");
                throw;
            }
        }
    }
}
