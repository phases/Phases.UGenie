﻿using NPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Phases.UmbracoGenie.Models
{
    [TableName("UmbracoGenieConfigs")]
    [PrimaryKey("Id", AutoIncrement = false)]
    [ExplicitColumns]
    public class UmbracoGenieConfigs
    {
        [PrimaryKeyColumn]
        [NullSetting(NullSetting = NullSettings.NotNull)]
        [Column("Id")]
        public int Id { get; set; }

        //default
        [Column("DefaultTextGenerationModel")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? DefaultTextGenerationModel { get; set; }

        [Column("DefaultImageGenerationModel")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? DefaultImageGenerationModel { get; set; }

        //openai
        [Column("OpenAITextModelName")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? OpenAITextModelName { get; set; }

        [Column("OpenAITextModelAPIKey")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? OpenAITextModelAPIKey { get; set; }

        //azureopenai
        [Column("AzureOpenAITextModelName")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? AzureOpenAITextModelName { get; set; }

        [Column("AzureOpenAITextModelAPIKey")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? AzureOpenAITextModelAPIKey { get; set; }

        [Column("AzureOpenAITextEndpoint")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? AzureOpenAITextEndpoint { get; set; }

        //gemini
        [Column("GeminiTextModelName")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? GeminiTextModelName { get; set; }

        [Column("GeminiTextModelAPIKey")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? GeminiTextModelAPIKey { get; set; }


        //ollama
        [Column("OllamaTextModelName")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? OllamaTextModelName { get; set; }

        [Column("OllamaTextModelAPIKey")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? OllamaTextModelAPIKey { get; set; }

        [Column("OllamaTextEndpoint")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? OllamaTextEndpoint { get; set; }


        //huggingface
        [Column("HuggingFaceTextModelName")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? HuggingFaceTextModelName { get; set; }

        [Column("HuggingFaceTextModelAPIKey")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? HuggingFaceTextModelAPIKey { get; set; }

        [Column("HuggingFaceTextEndpoint")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? HuggingFaceTextEndpoint { get; set; }


        //OpenAI
        [Column("OpenAIImageModelName")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? OpenAIImageModelName { get; set; }

        [Column("OpenAIImageModelAPIKey")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? OpenAIImageModelAPIKey { get; set; }

        [Column("OpenAIImageModelEndpoint")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? OpenAIImageModelEndpoint { get; set; }

        //AzureOpenAI
        [Column("AzureOpenAIImageModelName")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? AzureOpenAIImageModelName { get; set; }

        [Column("AzureOpenAIImageModelAPIKey")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? AzureOpenAIImageModelAPIKey { get; set; }

        [Column("AzureOpenAIImageModelEndpoint")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? AzureOpenAIImageModelEndpoint { get; set; }

        //inHouse
        [Column("inHouseImageModelName")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? InHouseImageModelName { get; set; }

        [Column("inHouseImageModelAPIKey")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? InHouseImageModelAPIKey { get; set; }

        [Column("inHouseImageModelEndpoint")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string? InHouseImageModelEndpoint { get; set; }

        [Column("enableForTextArea")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public int? enableForTextArea { get; set; }

        [Column("enableForTextBox")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public int? enableForTextBox { get; set; }

    }
}
