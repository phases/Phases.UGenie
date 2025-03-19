using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.TextToImage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Phases.UmbracoGenie.Services.Interfaces;

namespace Phases.UmbracoGenie.Services
{
    public class SemanticKernelService : ISemanticKernelService
    {
        private readonly ChatHistory _chatHistory;
        private readonly IKernelFactory _kernelFactory;

        public SemanticKernelService(IKernelFactory kernelFactory)
        {
            _chatHistory = new ChatHistory();
            _kernelFactory = kernelFactory;
        }

        public async Task<string> ChatAsync(string userMessage)
        {
            _chatHistory.AddUserMessage(userMessage);
            OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };
            var kernel = await _kernelFactory.CreateKernelAsync();
            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            var result = await chatCompletionService.GetChatMessageContentsAsync(
                _chatHistory,
                executionSettings: openAIPromptExecutionSettings,
                kernel: kernel);

            var response = result.FirstOrDefault()?.Content ?? "No response generated.";
            _chatHistory.AddAssistantMessage(response);

            return response;
        }

        public async Task<string> GenerateTextAsync(string prompt, string? systemPrompt = null)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(systemPrompt))
                    _chatHistory.AddSystemMessage(systemPrompt ?? "You are a helpful AI assistant.");

                _chatHistory.AddUserMessage(prompt);

                var kernel = await _kernelFactory.CreateKernelAsync();
                var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

                var executionSettings = new OpenAIPromptExecutionSettings
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
                };

                var result = await chatCompletionService.GetChatMessageContentsAsync(
                    _chatHistory,
                    executionSettings: executionSettings,
                    kernel: kernel);

                var response = result.FirstOrDefault()?.Content ?? "No response generated.";
                response = response.Replace("```html", "").Replace("```", "");
                _chatHistory.AddAssistantMessage(response);

                return response;
            }
            catch (HttpRequestException ex)
            {
                return $"Error communicating with the AI service: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"An unexpected error occurred: {ex.Message}";
            }
        }

        public async Task<string> GenerateImageAsync(string prompt)
        {
            try
            {
                // Extract dimensions from prompt
                var (width, height, cleanPrompt) = ExtractDimensionsFromPrompt(prompt);

                var kernel = await _kernelFactory.CreateKernelAsync();
                var imageService = kernel.GetRequiredService<ITextToImageService>();

                var result = await imageService.GenerateImageAsync(
                    cleanPrompt,
                    width: width,
                    height: height);

                return result;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error communicating with the AI service: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"An unexpected error occurred: {ex.Message}", ex);
            }
        }

        private (int width, int height, string cleanPrompt) ExtractDimensionsFromPrompt(string prompt)
        {
            int width = 1024, height = 1024;

            // Extract width
            var widthMatch = System.Text.RegularExpressions.Regex.Match(prompt, @"width:(\d+)");
            if (widthMatch.Success && int.TryParse(widthMatch.Groups[1].Value, out int parsedWidth))
            {
                width = parsedWidth;
                prompt = prompt.Replace(widthMatch.Value, "").Trim();
            }

            // Extract height
            var heightMatch = System.Text.RegularExpressions.Regex.Match(prompt, @"height:(\d+)");
            if (heightMatch.Success && int.TryParse(heightMatch.Groups[1].Value, out int parsedHeight))
            {
                height = parsedHeight;
                prompt = prompt.Replace(heightMatch.Value, "").Trim();
            }

            return (width, height, prompt);
        }

        public void ClearHistory()
        {
            _chatHistory.Clear();
        }
    }
}
