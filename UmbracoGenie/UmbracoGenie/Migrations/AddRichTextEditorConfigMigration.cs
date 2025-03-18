using System.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Packaging;

public class AddRichTextEditorConfigMigration : PackageMigrationBase
{
    private readonly IHostEnvironment _hostEnvironment;

    public AddRichTextEditorConfigMigration(
        IPackagingService packagingService,
        IMediaService mediaService,
        MediaFileManager mediaFileManager,
        MediaUrlGeneratorCollection mediaUrlGenerators,
        IShortStringHelper shortStringHelper,
        IContentTypeBaseServiceProvider contentTypeBaseServiceProvider,
        IMigrationContext context,
        IOptions<PackageMigrationSettings> packageMigrationsSettings,
        IHostEnvironment hostEnvironment)
        : base(
            packagingService,
            mediaService,
            mediaFileManager,
            mediaUrlGenerators,
            shortStringHelper,
            contentTypeBaseServiceProvider,
            context,
            packageMigrationsSettings)
    {
        _hostEnvironment = hostEnvironment;
    }

    protected override void Migrate()
    {
        var appSettingsPath = Path.Combine(_hostEnvironment.ContentRootPath, "appsettings.json");

        if (!File.Exists(appSettingsPath))
        {
            Logger.LogWarning("appsettings.json not found. Skipping migration.");
            return;
        }

        try
        {
            // Load the existing appsettings.json
            var json = File.ReadAllText(appSettingsPath);
            var jsonObject = JsonConvert.DeserializeObject<JObject>(json) ?? new JObject();

            var umbracoSection = jsonObject["Umbraco"]?["CMS"] as JObject ?? new JObject();

            // Check if RichTextEditor config already exists
            if (umbracoSection["RichTextEditor"] == null)
            {
                var richTextEditorConfig = JObject.Parse(@"
                {
                    ""CustomConfig"": {
                        ""external_plugins"": ""{\""aiTextGenerator\"":\""/App_Plugins/UmbracoGenie/RTEPlugin/plugin.js\""}""
                    },
                    ""Commands"": [
                        {
                            ""Alias"": ""generateText"",
                            ""Name"": ""Generate Text"",
                            ""Mode"": ""Insert""
                        },
                        {
                            ""Alias"": ""openCustomPopup"",
                            ""Name"": ""Custom Request"",
                            ""Mode"": ""Insert""
                        },
                        {
                            ""Alias"": ""paraphraseContent"",
                            ""Name"": ""Paraphrase"",
                            ""Mode"": ""Insert""
                        }
                    ]
                }");

                // Add the RichTextEditor config
                umbracoSection["RichTextEditor"] = richTextEditorConfig;
                jsonObject["Umbraco"]["CMS"] = umbracoSection;

                // Save the updated appsettings.json
                File.WriteAllText(appSettingsPath, JsonConvert.SerializeObject(jsonObject, Formatting.Indented));
                Logger.LogInformation("RichTextEditor configuration added successfully.");
            }
            else
            {
                Logger.LogInformation("RichTextEditor configuration already exists. Skipping.");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "An error occurred while adding RichTextEditor configuration.");
        }
    }
}
