using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;
using Phases.UmbracoGenie.Repositories.Interfaces;
using Phases.UmbracoGenie.Services.Interfaces;

namespace Phases.UmbracoGenie.Services
{
    public class ImageGenerationService : IImageGenerationService
    {
        private readonly ILogger<ImageGenerationService> _logger;
        private readonly ISemanticKernelService _semanticKernel;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IGenieRepository _repository;
        private readonly IMediaService _mediaService;
        private readonly MediaFileManager _mediaFileManager;
        private readonly MediaUrlGeneratorCollection _mediaUrlGenerators;
        private readonly IShortStringHelper _shortStringHelper;
        private readonly IContentTypeBaseServiceProvider _contentTypeBaseServiceProvider;

        public ImageGenerationService(
            ILogger<ImageGenerationService> logger,
            ISemanticKernelService semanticKernel,
            IHttpClientFactory clientFactory,
            IGenieRepository repository,
            IMediaService mediaService,
            MediaFileManager mediaFileManager,
            MediaUrlGeneratorCollection mediaUrlGenerators,
            IShortStringHelper shortStringHelper,
            IContentTypeBaseServiceProvider contentTypeBaseServiceProvider)
        {
            _logger = logger;
            _semanticKernel = semanticKernel;
            _clientFactory = clientFactory;
            _repository = repository;
            _mediaService = mediaService;
            _mediaFileManager = mediaFileManager;
            _mediaUrlGenerators = mediaUrlGenerators;
            _shortStringHelper = shortStringHelper;
            _contentTypeBaseServiceProvider = contentTypeBaseServiceProvider;
        }

        public async Task<string> GenerateAndSaveImageAsync(string prompt)
        {
            _logger.LogInformation("GenerateImage {0}", prompt);
            var config = await _repository.GetConfigurationAsync();
            var imageBytes = new byte[0];
            var imagePath = string.Empty;

            if (config?.SelectedImageModel != null)
            {
                imageBytes = await GetImageBytesAsync(prompt, config);
            }

            if (imageBytes.Length > 0)
            {
                imagePath = await SaveImageToMediaLibraryAsync(imageBytes);
            }

            return imagePath;
        }

        private async Task<byte[]> GetImageBytesAsync(string prompt, dynamic config)
        {
            var client = _clientFactory.CreateClient();
            client.Timeout = TimeSpan.FromMinutes(5);

            if (config.SelectedImageModel.Name != "In-House")
            {
                var imageUrl = await _semanticKernel.GenerateImageAsync(prompt);
                return await client.GetByteArrayAsync(imageUrl);
            }
            else
            {
                var requestBody = new
                {
                    prompt = prompt,
                    n_samples = 1,
                    plms = true,
                    skip_grid = true,
                    ckpt = "sd-v1-4.ckpt"
                };
                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("", content);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
            }

            return Array.Empty<byte>();
        }

        private async Task<string> SaveImageToMediaLibraryAsync(byte[] imageBytes)
        {
            var existingRootFolder = _mediaService.GetRootMedia()?.FirstOrDefault(r => r.Name == "Genie");
            int parentFolderId = await CreateOrGetAiImagesFolderId(existingRootFolder);

            if (parentFolderId > 0)
            {
                var guid = Guid.NewGuid();
                var fileName = $"{guid}.png";
                var newImage = _mediaService.CreateMediaWithIdentity(guid.ToString(), parentFolderId, "Image");

                using (MemoryStream memoryStream = new MemoryStream(imageBytes))
                {
                    newImage.SetValue(_mediaFileManager, _mediaUrlGenerators, _shortStringHelper, _contentTypeBaseServiceProvider, Constants.Conventions.Media.File, fileName, memoryStream);
                    var savedMedia = _mediaService.Save(newImage);
                    if (savedMedia.Success)
                    {
                        var savedImage = _mediaService.GetById(newImage.Id);
                        var umbracoFilePath = savedImage?.GetValue<string>(Constants.Conventions.Media.File);
                        var jsonObject = JObject.Parse(umbracoFilePath!);
                        return jsonObject["src"].ToString();
                    }
                }
            }

            return string.Empty;
        }

        private async Task<int> CreateOrGetAiImagesFolderId(IMedia? existingRootFolder)
        {
            if (existingRootFolder != null)
            {
                return existingRootFolder.Id;
            }

            var newParentFolder = _mediaService.CreateMediaWithIdentity("Ai-images", -1, Constants.Conventions.MediaTypes.Folder);
            if (newParentFolder != null)
            {
                _mediaService.Save(newParentFolder);
                return newParentFolder.Id;
            }

            return 0;
        }
    }
}
