﻿using Microsoft.Extensions.Logging;
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
                var database = scope.Database;

                var dbConfig = await database.FirstOrDefaultAsync<UmbracoGenieConfigs>("WHERE Id = 1000");
                _logger.LogInformation("getting Genie configuration");
                if (dbConfig == null)
                {
                    _logger.LogInformation("creating Genie configuration");
                    var defaultConfig = new GenieConfigurationDto
                    {
                        TextModels = new List<TextModelDto>
                        {
                            new TextModelDto { Name = "OpenAI", ModelId = "gpt-3.5-turbo", ApiKey = "" },
                            new TextModelDto { Name = "AzureOpenAI", ModelId = "gpt-3.5-turbo", ApiKey = "", Endpoint = "" },
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

                    dbConfig ??= new UmbracoGenieConfigs();
                    UpdateDbModelFromDto(dbConfig, defaultConfig);

                    // Handle different database providers
                    var provider = database.DatabaseType.GetProviderName();
                    _logger.LogInformation($"provider {0}", provider);
                    if (provider != "System.Data.SQLite")
                    {
                        await database.ExecuteAsync("SET IDENTITY_INSERT UmbracoGenieConfigs ON");
                    }

                    try 
                    {
                        await database.InsertAsync(dbConfig);
                    }
                    finally 
                    {
                        if (provider != "System.Data.SQLite")
                        {
                            await database.ExecuteAsync("SET IDENTITY_INSERT UmbracoGenieConfigs OFF");
                        }
                    }
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
                var database = scope.Database;

                _logger.LogInformation("saving Genie configuration");
                var dbConfig = await database.FirstOrDefaultAsync<UmbracoGenieConfigs>("WHERE Id = 1000");
                bool isNew = dbConfig == null;

                dbConfig ??= new UmbracoGenieConfigs();
                UpdateDbModelFromDto(dbConfig, configuration);

                if (isNew)
                {
                    _logger.LogInformation("inserting Genie configuration");
                    var provider = database.DatabaseType.GetProviderName();
                    if (provider != "System.Data.SQLite")
                    {
                        await database.ExecuteAsync("SET IDENTITY_INSERT UmbracoGenieConfigs ON");
                    }

                    try 
                    {
                        await database.InsertAsync(dbConfig);
                    }
                    finally 
                    {
                        if (provider != "System.Data.SQLite")
                        {
                            await database.ExecuteAsync("SET IDENTITY_INSERT UmbracoGenieConfigs OFF");
                        }
                    }
                }
                else
                {
                    _logger.LogInformation("updating Genie configuration");
                    await database.UpdateAsync(dbConfig);
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

        private GenieConfigurationDto MapToDto(UmbracoGenieConfigs dbModel)
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
                    },
                    new TextModelDto
                    {
                        Name = "AzureOpenAI",
                        ModelId = dbModel.AzureOpenAITextModelName,
                        ApiKey = dbModel.AzureOpenAITextModelAPIKey,
                        Endpoint = dbModel.AzureOpenAITextEndpoint
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

        private void UpdateDbModelFromDto(UmbracoGenieConfigs dbModel, GenieConfigurationDto dto)
        {
            // Set defaults
            dbModel.Id = 1000;
            dbModel.DefaultTextGenerationModel = dto.SelectedTextModel?.Name;
            dbModel.DefaultImageGenerationModel = dto.SelectedImageModel?.Name;
            dbModel.enableForTextArea = dto.enableForTextArea.HasValue ? (dto.enableForTextArea.Value ? 1 : 0) : 0;
            dbModel.enableForTextBox = dto.enableForTextBox.HasValue ? (dto.enableForTextBox.Value ? 1 : 0) : 0;

            // Update Text Models
            var azureOpenAI = dto.TextModels?.FirstOrDefault(m => m.Name == "AzureOpenAI");
            if (azureOpenAI != null)
            {
                dbModel.AzureOpenAITextModelName = azureOpenAI.ModelId;
                dbModel.AzureOpenAITextModelAPIKey = azureOpenAI.ApiKey;
                dbModel.AzureOpenAITextEndpoint = azureOpenAI.Endpoint;
            }

            var openAI = dto.TextModels?.FirstOrDefault(m => m.Name == "OpenAI");
            if (openAI != null)
            {
                dbModel.OpenAITextModelName = openAI.ModelId;
                dbModel.OpenAITextModelAPIKey = openAI.ApiKey;
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

            var azureOpenAIImage = dto.ImageModels?.FirstOrDefault(m => m.Name == "Azure OpenAI");
            if (azureOpenAIImage != null)
            {
                dbModel.AzureOpenAIImageModelName = azureOpenAIImage.ModelId;
                dbModel.AzureOpenAIImageModelAPIKey = azureOpenAIImage.ApiKey;
                dbModel.AzureOpenAIImageModelEndpoint = azureOpenAIImage.Endpoint;
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
