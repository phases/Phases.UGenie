using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Phases.UmbracoGenie.Models.Dtos;

namespace Phases.UmbracoGenie.Repositories.Interfaces
{
    public interface IGenieRepository
    {
        Task<GenieConfigurationDto?> GetConfigurationAsync();
        Task<bool> SaveConfigurationAsync(GenieConfigurationDto configuration);
    }
}
