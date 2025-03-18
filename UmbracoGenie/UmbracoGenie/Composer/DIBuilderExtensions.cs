using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;
using Phases.UmbracoGenie.Repositories;
using Phases.UmbracoGenie.Repositories.Interfaces;
using Phases.UmbracoGenie.Services;
using Phases.UmbracoGenie.Services.Interfaces;
using Phases.UmbracoGenie.Utils;

namespace Phases.UmbracoGenie.Composer
{
    public static class DIBuilderExtensions
    {
        public static IUmbracoBuilder RegisterCustomServices(this IUmbracoBuilder builder)
        {
            builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.ContractResolver = new CustomContractResolver();
                });
            builder.Services.AddScoped<IGenieService, GenieService>();
            builder.Services.AddScoped<IGenieRepository, GenieRepository>();
            builder.Services.AddScoped<IKernelFactory, KernelFactory>();
            builder.Services.AddScoped<ISemanticKernelService,SemanticKernelService>();
            builder.Services.AddScoped<IImageGenerationService, ImageGenerationService>();
            return builder;
        }
        public static IUmbracoBuilder AddCustomServices(this IUmbracoBuilder builder)
        {
            builder.RegisterCustomServices();
            return builder;
        }
    }
}
