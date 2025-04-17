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
            try
            {
                if (TableExists("UmbracoGenieConfigs") == false)
                {
                    Create.Table<UmbracoGenieConfigs>().Do();
                    Delete.Table("UmbracoGenieConfig").Do();
                    Logger.LogInformation("The UmbracoGenie table added");
                }
                else
                {
                    Delete.Table("UmbracoGenieConfig").Do();
                    Logger.LogInformation("Database table schema updated successfully");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error during UmbracoGenieTablesMigration");
                throw;
            }
        }
    }
}
