using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phases.UmbracoGenie.Services.Interfaces
{
    public interface ISemanticKernelService
    {
        Task<string> ChatAsync(string userMessage);
        Task<string> GenerateTextAsync(string prompt, string? systemPrompt = null);

        Task<string> GenerateImageAsync(string prompt);
    }
}
