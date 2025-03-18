using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Phases.UmbracoGenie.Models.Dtos;

namespace Phases.UmbracoGenie.Services.Interfaces
{
    public interface IGenieService
    {
        Task<bool> SaveConfigurationAsync(GenieConfigurationDto configuration);
        Task<GenieConfigurationDto> GetConfigurationAsync();
    }
}
