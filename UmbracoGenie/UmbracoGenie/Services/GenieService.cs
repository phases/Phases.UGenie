using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Persistence;
using Phases.UmbracoGenie.Models.Dtos;
using Phases.UmbracoGenie.Repositories.Interfaces;
using Phases.UmbracoGenie.Services.Interfaces;

namespace Phases.UmbracoGenie.Services
{
    public class GenieService : IGenieService
    {
        private readonly IGenieRepository _repository;

        public GenieService(IGenieRepository repository)
        {
            _repository = repository;
        }

        public async Task<GenieConfigurationDto> GetConfigurationAsync()
        {
            var config = await _repository.GetConfigurationAsync();
            return config ?? new GenieConfigurationDto
            {
                TextModels = new List<TextModelDto>(),
                ImageModels = new List<ImageModelDto>()
            };
        }

        public async Task<bool> SaveConfigurationAsync(GenieConfigurationDto configuration)
        {
            return await _repository.SaveConfigurationAsync(configuration);
        }
    }
}
