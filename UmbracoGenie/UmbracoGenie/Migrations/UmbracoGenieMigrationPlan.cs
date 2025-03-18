using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Packaging;

namespace Phases.UmbracoGenie.Migrations
{
    public class UmbracoGenieMigrationPlan : PackageMigrationPlan
    {
        public UmbracoGenieMigrationPlan() : base("UmbracoGenie")
        {
        }

        protected override void DefinePlan()
        {
            To<UmbracoGenieTablesMigration>("UmbracoGenieTablesMigration");
            To<AddRichTextEditorConfigMigration>("AddRichTextEditorConfig");
        }
    }
}
