using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Versionamento.WebApi.Extension
{
    public static class ApiVersionAddExtensionHandler
    {
        public static IServiceCollection AddApiVersionHandler(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.Conventions.Controller<V2.Controllers.ProdutoController>().HasApiVersion(new ApiVersion(2, 0));
            });
            services.AddVersionedApiExplorer(
                options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });

            return services;
        }
    }
}
