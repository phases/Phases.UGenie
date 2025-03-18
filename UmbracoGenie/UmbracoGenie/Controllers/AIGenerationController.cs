using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Web.Common.Controllers;
using Phases.UmbracoGenie.Models;
using System.Text.RegularExpressions;
using Phases.UmbracoGenie.Services.Interfaces;
using Phases.UmbracoGenie.Repositories.Interfaces;
using Umbraco.Extensions;
using Phases.UmbracoGenie;

namespace UmbracoGenie.Controllers
{
    public class AIGenerationController : UmbracoApiController
    {
        private readonly ILogger<AIGenerationController> _logger;
        private readonly ISemanticKernelService _semanticKernel;
        private readonly IImageGenerationService _imageGenerationService;
        public AIGenerationController(IHttpClientFactory clientFactory, ILogger<AIGenerationController> logger, ISemanticKernelService semanticKernelService, IGenieRepository genieRepository,
            IImageGenerationService imageGenerationService)
        {
            _logger = logger;
            _semanticKernel = semanticKernelService;
            _imageGenerationService = imageGenerationService;
        }

        [HttpPost]
        public async Task<IActionResult> Generate([FromBody] PromptModel model)
        {
            _logger.LogInformation("Generate called with prompt: {Prompt}", model.Prompt);
            if (string.IsNullOrWhiteSpace(model.Prompt))
            {
                _logger.LogError("Generate: Invalid AI model specified. Prompt was empty.");
                return BadRequest("Invalid AI model specified");
            }
            string systemPrompt = SystemPrompts.GenerateTextPrompt;
            try
            {
                var result = await _semanticKernel.GenerateTextAsync(model.Prompt, systemPrompt);
                _logger.LogInformation("Generate completed successfully.");
                return new JsonResult(new { text = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Generate encountered an error.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditGeneratedText([FromBody] EditPromptModel model)
        {
            _logger.LogInformation("EditGeneratedText called with prompt: {Prompt}", model.Prompt);
            if (string.IsNullOrWhiteSpace(model.Prompt))
            {
                _logger.LogError("EditGeneratedText: Invalid AI model specified. Prompt was empty.");
                return BadRequest("Invalid AI model specified");
            }
            var systemPrompt = SystemPrompts.EditTextPrompt;
            try
            {
                var result = await _semanticKernel.GenerateTextAsync(model.Prompt, systemPrompt);
                _logger.LogInformation("EditGeneratedText completed successfully.");
                return new JsonResult(new { text = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EditGeneratedText encountered an error.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Paraphrase([FromBody] PromptModel model)
        {
            _logger.LogInformation("Paraphrase called with prompt: {Prompt}", model.Prompt);
            if (string.IsNullOrWhiteSpace(model.Prompt))
            {
                _logger.LogError("Paraphrase: Invalid prompt specified. Prompt was empty.");
                return BadRequest("Invalid prompt specified");
            }
            var systemPrompt = SystemPrompts.ParaphrasePrompt;
            try
            {
                var result = await _semanticKernel.GenerateTextAsync(model.Prompt, systemPrompt);
                _logger.LogInformation("Paraphrase completed successfully.");
                return new JsonResult(new { text = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Paraphrase encountered an error.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetSuggestions([FromBody] PromptModel model)
        {
            _logger.LogInformation("GetSuggestions called with prompt: {Prompt}", model.Prompt);
            var words = model.Prompt.Split(' ');
            var lastWord = words.Length > 0 ? words[words.Length - 1] : "";
            var suggestionPrompt = $"Provide 5 word completion suggestions for '{lastWord}' in the context of '{model.Prompt}'. Just list the words.";
            try
            {
                var result = await _semanticKernel.GenerateTextAsync(model.Prompt);
                if (result != null)
                {
                    var suggestions = result.Split('\n')
                                            .Select(line => line.Trim())
                                            .Where(line => Regex.IsMatch(line, @"^\d+\.\s*(.+)"))
                                            .Select(line => Regex.Replace(line, @"^\d+\.\s*", "").Trim())
                                            .Distinct()
                                            .Take(5)
                                            .ToList();
                    _logger.LogInformation("GetSuggestions completed successfully with {Count} suggestions.", suggestions.Count);
                    return Ok(suggestions);
                }
                _logger.LogError("GetSuggestions: SemanticKernel result was null.");
                return BadRequest("Failed to get suggestions");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetSuggestions encountered an error.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GenerateImage([FromBody] PromptModel model)
        {
            _logger.LogInformation("GenerateImage called with prompt: {Prompt}", model.Prompt);
            if (string.IsNullOrWhiteSpace(model.Prompt))
            {
                _logger.LogError("GenerateImage: Invalid AI model specified. Prompt was empty.");
                return BadRequest("Invalid AI model specified");
            }
            try
            {
                var imagePath = await _imageGenerationService.GenerateAndSaveImageAsync(model.Prompt);
                if (string.IsNullOrEmpty(imagePath))
                {
                    _logger.LogError("GenerateImage: Failed to generate or save the image.");
                    return BadRequest("Failed to generate or save image");
                }
                _logger.LogInformation("GenerateImage completed successfully. Image path: {ImagePath}", imagePath);
                return Ok(imagePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GenerateImage encountered an error.");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}