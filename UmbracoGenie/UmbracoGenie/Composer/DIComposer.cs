using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Phases.UmbracoGenie.Composer
{
    public class DIComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.AddCustomServices();
        }
    }
}
