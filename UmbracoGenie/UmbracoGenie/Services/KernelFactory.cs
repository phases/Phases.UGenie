using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Phases.UmbracoGenie.Controllers;
using Phases.UmbracoGenie.Repositories.Interfaces;
using Phases.UmbracoGenie.Services.Interfaces;

namespace Phases.UmbracoGenie.Services
{
    public class KernelFactory : IKernelFactory
    {
        private readonly IGenieRepository _repository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<KernelFactory> _logger;

        public KernelFactory(IGenieRepository repository, IHttpClientFactory httpClientFactory, ILogger<KernelFactory> logger)
        {
            _repository = repository;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<Kernel> CreateKernelAsync()
        {
            var config = await _repository.GetConfigurationAsync();
            var builder = Kernel.CreateBuilder();

            if (config?.SelectedTextModel != null)
            {
                _logger.LogInformation($"Creating kernel for model {config.SelectedTextModel.Name}");
                switch (config.SelectedTextModel.Name)
                {
                    case "AzureOpenAI":
                        if (!string.IsNullOrEmpty(config.SelectedTextModel.ApiKey))
                        {
                            builder.Services.AddAzureOpenAIChatCompletion(
                                config.SelectedTextModel.ModelId,
                                config.SelectedTextModel.Endpoint,
                                config.SelectedTextModel.ApiKey);
                        }
                        break;
                    case "OpenAI":
                        if (!string.IsNullOrEmpty(config.SelectedTextModel.ApiKey))
                        {
                            builder.Services.AddOpenAIChatCompletion(
                                config.SelectedTextModel.ModelId,
                                config.SelectedTextModel.ApiKey);
                        }
                        break;
                    case "Ollama":
                        if (!string.IsNullOrEmpty(config.SelectedTextModel.Endpoint))
                        {
                            var ollamaEndpoint = new Uri(config.SelectedTextModel.Endpoint);
                            builder.Services.AddOllamaChatCompletion(
                                config.SelectedTextModel.ModelId,
                                ollamaEndpoint
                                );
                            builder.Services.AddOllamaTextGeneration(
                                config.SelectedTextModel.ModelId,
                                ollamaEndpoint);
                        }
                        break;
                    case "Gemini":
                        if (!string.IsNullOrEmpty(config.SelectedTextModel.ApiKey))
                        {
                            builder.Services.AddGoogleAIGeminiChatCompletion(
                                config.SelectedTextModel.ModelId,
                                config.SelectedTextModel.ApiKey);

                        }
                        break;
                    //case "HuggingFace":
                    //    if (!string.IsNullOrEmpty(config.SelectedTextModel.ApiKey))
                    //    {
                    //        var endpoint = new Uri(config?.SelectedTextModel?.Endpoint);
                    //        builder.Services.AddHuggingFaceChatCompletion(
                    //            config.SelectedTextModel.ModelId,
                    //            endpoint: endpoint,
                    //            apiKey: config.SelectedTextModel.ApiKey);

                    //    }
                    //    break;
                }
            }

            if (config?.SelectedImageModel != null)
            {
                _logger.LogInformation($"Creating kernel for model {config.SelectedImageModel.Name}");
                switch (config.SelectedImageModel.Name)
                {
                    case "Azure OpenAI":
                        if (!string.IsNullOrEmpty(config.SelectedImageModel.ApiKey))
                        {
                            builder.Services.AddAzureOpenAITextToImage(
                                deploymentName:config.SelectedImageModel.ModelId,
                                endpoint:config.SelectedImageModel.Endpoint,
                                apiKey:config.SelectedImageModel.ApiKey);
                        }
                        break;
                    case "OpenAI":
                        if (!string.IsNullOrEmpty(config.SelectedImageModel.ApiKey))
                        {
                            builder.Services.AddOpenAITextToImage(
                                modelId: config.SelectedImageModel.ModelId,
                                apiKey: config.SelectedImageModel.ApiKey);
                        }
                        break;
                }
            }
            return builder.Build();
        }
    }
}
