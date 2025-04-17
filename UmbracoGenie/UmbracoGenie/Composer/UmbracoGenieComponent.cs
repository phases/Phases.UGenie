using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations.Upgrade;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Cms.Core.Migrations;
using Phases.UmbracoGenie.Migrations;

namespace Phases.UmbracoGenie.Composer
{
    public class UmbracoGenieComponent : IComponent
    {
        private readonly ICoreScopeProvider _coreScopeProvider;
        private readonly IMigrationPlanExecutor _migrationPlanExecutor;
        private readonly IKeyValueService _keyValueService;
        private readonly IRuntimeState _runtimeState;

        public UmbracoGenieComponent(
            ICoreScopeProvider coreScopeProvider,
            IMigrationPlanExecutor migrationPlanExecutor,
            IKeyValueService keyValueService,
            IRuntimeState runtimeState)
        {
            _coreScopeProvider = coreScopeProvider;
            _migrationPlanExecutor = migrationPlanExecutor;
            _keyValueService = keyValueService;
            _runtimeState = runtimeState;
        }
        public void Initialize()
        {
            if (_runtimeState.Level < RuntimeLevel.Run)
            {
                return;
            }
            // Create a migration plan for a specific project/feature
            // We can then track that latest migration state/step for this project/feature
            var migrationPlan = new MigrationPlan("UmbracoGenieUpdate");

            // This is the steps we need to take
            // Each step in the migration adds a unique value
            migrationPlan.From(string.Empty)
                .To<UmbracoGenieTablesMigration>("umbracogenie-db-1")
                .To<UmbracoGenieTablesMigration>(new Guid("920D13EC-EDBE-42F6-94E5-B75F9FB27284"));

            // Go and upgrade our site (Will check if it needs to do the work or not)
            // Based on the current/latest step
            var upgrader = new Upgrader(migrationPlan);
            upgrader.Execute(_migrationPlanExecutor, _coreScopeProvider, _keyValueService);
        }

        public void Terminate()
        {
            
        }
    }
}
