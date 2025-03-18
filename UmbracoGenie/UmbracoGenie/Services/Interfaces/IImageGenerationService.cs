using Microsoft.AspNetCore.Mvc;

namespace Phases.UmbracoGenie.Services.Interfaces
{
    public interface IImageGenerationService
    {
        Task<string> GenerateAndSaveImageAsync(string prompt);
    }
}
