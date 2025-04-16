using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Infrastructure.Packaging;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseModelDefinitions;
using Umbraco.Cms.Infrastructure.Scoping;
using Phases.UmbracoGenie.Models;
using Phases.UmbracoGenie.Models.Dtos;
using Phases.UmbracoGenie.Repositories.Interfaces;

namespace Phases.UmbracoGenie.Migrations
{
    public class UmbracoGenieTablesMigration : MigrationBase
    {
        public UmbracoGenieTablesMigration(IMigrationContext context) : base(context)
        {
        }

        protected override void Migrate()
        {
            if (TableExists("UmbracoGenieConfig") == false)
            {
                Create.Table<UmbracoGenieConfig>().Do();
                Logger.LogInformation("The UmbracoGenie table added");
            }
            else
            {
                if (!ColumnExists("UmbracoGenieConfig", "AzureOpenAITextModelName"))
                {
                    Create.Column("AzureOpenAITextModelName").OnTable("UmbracoGenieConfig").AsString().Nullable().WithDefaultValue("").Do();
                    Logger.LogInformation("Added column AzureOpenAITextModelName");
                }

                if (!ColumnExists("UmbracoGenieConfig", "AzureOpenAITextModelAPIKey"))
                {
                    Create.Column("AzureOpenAITextModelAPIKey").OnTable("UmbracoGenieConfig").AsString().Nullable().WithDefaultValue("").Do();
                    Logger.LogInformation("Added column AzureOpenAITextModelAPIKey");
                }

                if (!ColumnExists("UmbracoGenieConfig", "AzureOpenAITextEndpoint"))
                {
                    Create.Column("AzureOpenAITextEndpoint").OnTable("UmbracoGenieConfig").AsString().Nullable().WithDefaultValue("").Do();
                    Logger.LogInformation("Added column AzureOpenAITextEndpoint");
                }
                Logger.LogInformation("The database table UmbracoGenie already exists, skipping");
            }
        }
    }
}
