using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Trees;
using Umbraco.Cms.Core;
using Umbraco.Cms.Web.BackOffice.Trees;
using Umbraco.Cms.Web.Common.ModelBinders;
using Umbraco.Cms.Web.Common.Attributes;
using Microsoft.AspNetCore.Http;

namespace Phases.UmbracoGenie.Controllers
{
    [Tree("settings", "UmbracoGenieAlias", IsSingleNodeTree = true, TreeTitle = "Umbraco Genie", TreeGroup = "UmbracoGenie", SortOrder = 3)]
    [PluginController("umbracoGenie")]
    public class UmbracoGenieTreeController : TreeController
    {
        private readonly IMenuItemCollectionFactory _menuItemCollectionFactory;
        public UmbracoGenieTreeController(ILocalizedTextService localizedTextService, IMenuItemCollectionFactory menuItemCollectionFactory, UmbracoApiControllerTypeCollection umbracoApiControllerTypeCollection, IEventAggregator eventAggregator) : base(localizedTextService, umbracoApiControllerTypeCollection, eventAggregator)
        {
            _menuItemCollectionFactory = menuItemCollectionFactory ?? throw new ArgumentNullException(nameof(menuItemCollectionFactory));
        }

        protected override ActionResult<MenuItemCollection> GetMenuForNode(string id, [ModelBinder(typeof(HttpQueryStringModelBinder))] FormCollection queryStrings)
        {
            throw new NotImplementedException();
        }

        protected override ActionResult<TreeNodeCollection> GetTreeNodes(string id, [ModelBinder(typeof(HttpQueryStringModelBinder))] FormCollection queryStrings)
        {
            throw new NotImplementedException();
        }

        protected override ActionResult<TreeNode> CreateRootNode(FormCollection queryStrings)
        {
            var rootResult = base.CreateRootNode(queryStrings);
            if (!(rootResult.Result is null))
            {
                return rootResult;
            }

            var root = rootResult.Value;

            //optionally setting a routepath would allow you to load in a custom UI instead of the usual behaviour for a tree
            root.RoutePath = string.Format("{0}/{1}/{2}", Constants.Applications.Settings, "UmbracoGenieAlias", "edit");
            // set the icon
            root.Icon = "icon-hearts";
            // set to false for a custom tree with a single node.
            root.HasChildren = false;
            //url for menu
            root.MenuUrl = null;

            return root;
        }
    }
}
