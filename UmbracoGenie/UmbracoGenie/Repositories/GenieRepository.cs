using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Phases.UmbracoGenie.Models.Dtos;
using Phases.UmbracoGenie.Models;
using Phases.UmbracoGenie.Repositories.Interfaces;
using Umbraco.Cms.Core.Scoping;

namespace Phases.UmbracoGenie.Repositories
{
    public class GenieRepository : IGenieRepository
    {
        private readonly IScopeProvider _scopeProvider;
        private readonly ILogger<GenieRepository> _logger;

        public GenieRepository(IScopeProvider scopeProvider, ILogger<GenieRepository> logger)
        {
            _scopeProvider = scopeProvider;
            _logger = logger;
        }

        public async Task<GenieConfigurationDto?> GetConfigurationAsync()
        {
            try
            {
                using var scope = _scopeProvider.CreateScope();

                var dbConfig = await scope.Database.FirstOrDefaultAsync<UmbracoGenieConfig>("WHERE Id = 1000");
                _logger.LogInformation("getting Genie configuration");
                if (dbConfig == null)
                {
                    _logger.LogInformation("creating Genie configuration");
                    // Seed default configuration
                    var defaultConfig = new GenieConfigurationDto
                    {
                        TextModels = new List<TextModelDto>
                        {
                            new TextModelDto { Name = "OpenAI", ModelId = "gpt-3.5-turbo", ApiKey = "", Endpoint = "" },
                            new TextModelDto { Name = "Gemini Pro", ModelId = "gemini-pro", ApiKey = "" },
                            new TextModelDto { Name = "Ollama", ModelId = "llama2", Endpoint = "http://localhost:11434" },
                            //new TextModelDto { Name = "HuggingFace", ModelId = "bigscience/bloom", ApiKey = "", Endpoint = "https://api-inference.huggingface.co/models" }
                        },
                        ImageModels = new List<ImageModelDto>
                        {
                            new ImageModelDto { Name = "Azure OpenAI", Model = "dall-e-3", ApiKey = "", Endpoint = "" },
                            new ImageModelDto { Name = "OpenAI", Model = "dall-e-3", ApiKey = "" },
                            new ImageModelDto { Name = "In-House", Model = "", ApiKey = "", Endpoint = "http://localhost:5000/api/image-generation" }
                        },
                        SelectedTextModel = new TextModelDto { Name = "OpenAI" },
                        SelectedImageModel = new ImageModelDto { Name = "OpenAI" },
                        enableForTextArea = false,
                        enableForTextBox = false
                    };

                    dbConfig ??= new UmbracoGenieConfig();
                    UpdateDbModelFromDto(dbConfig, defaultConfig);
                    await scope.Database.InsertAsync(dbConfig);
                }

                return MapToDto(dbConfig);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Genie configuration");
                throw;
            }
        }

        public async Task<bool> SaveConfigurationAsync(GenieConfigurationDto configuration)
        {
            try
            {
                using var scope = _scopeProvider.CreateScope();

                _logger.LogInformation("saving Genie configuration");
                var dbConfig = await scope.Database.FirstOrDefaultAsync<UmbracoGenieConfig>("WHERE Id = 1000");
                bool isNew = dbConfig == null;

                dbConfig ??= new UmbracoGenieConfig();
                UpdateDbModelFromDto(dbConfig, configuration);

                if (isNew)
                {
                    _logger.LogInformation("inserting Genie configuration");
                    await scope.Database.ExecuteAsync("SET IDENTITY_INSERT UmbracoGenieConfig ON");
                    await scope.Database.InsertAsync(dbConfig);
                    // Disable IDENTITY_INSERT after insert
                    await scope.Database.ExecuteAsync("SET IDENTITY_INSERT UmbracoGenieConfig OFF");
                }
                else
                {
                    _logger.LogInformation("updating Genie configuration");
                    await scope.Database.UpdateAsync(dbConfig);
                }

                scope.Complete();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving Genie configuration");
                return false;
            }
        }

        private GenieConfigurationDto MapToDto(UmbracoGenieConfig dbModel)
        {
            var dto = new GenieConfigurationDto
            {
                TextModels = new List<TextModelDto>
                {
                    new TextModelDto
                    {
                        Name = "OpenAI",
                        ModelId = dbModel.OpenAITextModelName,
                        ApiKey = dbModel.OpenAITextModelAPIKey,
                        Endpoint = dbModel.OpenAITextEndpoint
                    },
                    new TextModelDto
                    {
                        Name = "Gemini",
                        ModelId = dbModel.GeminiTextModelName,
                        ApiKey = dbModel.GeminiTextModelAPIKey
                    },
                    new TextModelDto
                    {
                        Name = "Ollama",
                        ModelId = dbModel.OllamaTextModelName,
                        ApiKey = dbModel.OllamaTextModelAPIKey,
                        Endpoint = dbModel.OllamaTextEndpoint
                    },
                    //new TextModelDto
                    //{
                    //    Name = "HuggingFace",
                    //    ModelId = dbModel.HuggingFaceTextModelName,
                    //    ApiKey = dbModel.HuggingFaceTextModelAPIKey,
                    //    Endpoint = dbModel.HuggingFaceTextEndpoint
                    //}
                },
                ImageModels = new List<ImageModelDto>
                {
                    new ImageModelDto
                    {
                        Name = "Azure OpenAI",
                        Model = dbModel.AzureOpenAIImageModelName,
                        ModelId = dbModel.AzureOpenAIImageModelName,
                        ApiKey = dbModel.AzureOpenAIImageModelAPIKey,
                        Endpoint = dbModel.AzureOpenAIImageModelEndpoint
                    },
                    new ImageModelDto
                    {
                        Name = "OpenAI",
                        Model = dbModel.OpenAIImageModelName,
                        ModelId = dbModel.OpenAIImageModelName,
                        ApiKey = dbModel.OpenAIImageModelAPIKey,
                        Endpoint = dbModel.OpenAIImageModelEndpoint
                    },
                    new ImageModelDto
                    {
                        Name = "In-House",
                        ApiKey = dbModel.InHouseImageModelAPIKey,
                        Model = dbModel.InHouseImageModelName,
                        ModelId = dbModel.InHouseImageModelName,
                        Endpoint = dbModel.InHouseImageModelEndpoint
                    }
                }
            };

            // Set selected models based on defaults
            dto.SelectedTextModel = dto.TextModels.FirstOrDefault(m => m.Name == dbModel.DefaultTextGenerationModel);
            dto.SelectedImageModel = dto.ImageModels.FirstOrDefault(m => m.Name == dbModel.DefaultImageGenerationModel);
            dto.enableForTextArea = dbModel.enableForTextArea.HasValue ? 
                                    (dbModel.enableForTextArea.Value == 1 ? (bool?)true : (bool?)false) : 
                                    null;

            dto.enableForTextBox = dbModel.enableForTextBox.HasValue ? 
                                (dbModel.enableForTextBox.Value == 1 ? (bool?)true : (bool?)false) : 
                                null;



            return dto;
        }

        private void UpdateDbModelFromDto(UmbracoGenieConfig dbModel, GenieConfigurationDto dto)
        {
            // Set defaults
            dbModel.Id = 1000;
            dbModel.DefaultTextGenerationModel = dto.SelectedTextModel?.Name;
            dbModel.DefaultImageGenerationModel = dto.SelectedImageModel?.Name;
            dbModel.enableForTextArea = dto.enableForTextArea.HasValue ? (dto.enableForTextArea.Value ? 1 : 0) : 0;
            dbModel.enableForTextBox = dto.enableForTextBox.HasValue ? (dto.enableForTextBox.Value ? 1 : 0) : 0;

            // Update Text Models
            var openAI = dto.TextModels?.FirstOrDefault(m => m.Name == "OpenAI");
            if (openAI != null)
            {
                dbModel.OpenAITextModelName = openAI.ModelId;
                dbModel.OpenAITextModelAPIKey = openAI.ApiKey;
                dbModel.OpenAITextEndpoint = openAI.Endpoint;
            }

            var gemini = dto.TextModels?.FirstOrDefault(m => m.Name == "Gemini");
            if (gemini != null)
            {
                dbModel.GeminiTextModelName = gemini.ModelId;
                dbModel.GeminiTextModelAPIKey = gemini.ApiKey;
            }

            var ollama = dto.TextModels?.FirstOrDefault(m => m.Name == "Ollama");
            if (ollama != null)
            {
                dbModel.OllamaTextModelName = ollama.ModelId;
                dbModel.OllamaTextModelAPIKey = ollama.ApiKey;
                dbModel.OllamaTextEndpoint = ollama.Endpoint;
            }

            //var huggingFace = dto.TextModels?.FirstOrDefault(m => m.Name == "HuggingFace");
            //if (huggingFace != null)
            //{
            //    dbModel.HuggingFaceTextModelName = huggingFace.ModelId;
            //    dbModel.HuggingFaceTextModelAPIKey = huggingFace.ApiKey;
            //    dbModel.HuggingFaceTextEndpoint = huggingFace.Endpoint;
            //}

            // Update Image Models
            var openAIImage = dto.ImageModels?.FirstOrDefault(m => m.Name == "OpenAI");
            if (openAIImage != null)
            {
                dbModel.OpenAIImageModelName = openAIImage.ModelId;
                dbModel.OpenAIImageModelAPIKey = openAIImage.ApiKey;
                dbModel.OpenAIImageModelEndpoint = openAIImage.Endpoint;
            }

            var azureOpenAI = dto.ImageModels?.FirstOrDefault(m => m.Name == "Azure OpenAI");
            if (azureOpenAI != null)
            {
                dbModel.AzureOpenAIImageModelName = azureOpenAI.ModelId;
                dbModel.AzureOpenAIImageModelAPIKey = azureOpenAI.ApiKey;
                dbModel.AzureOpenAIImageModelEndpoint = azureOpenAI.Endpoint;
            }

            var inHouse = dto.ImageModels?.FirstOrDefault(m => m.Name == "In-House");
            if (inHouse != null)
            {
                dbModel.InHouseImageModelName = inHouse.ModelId;
                dbModel.InHouseImageModelAPIKey = inHouse.ApiKey;
                dbModel.InHouseImageModelEndpoint = inHouse.Endpoint;
            }
        }
    }
}
