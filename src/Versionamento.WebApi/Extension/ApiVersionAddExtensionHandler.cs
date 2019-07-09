using Microsoft.Extensions.DependencyInjection;

namespace Versionamento.WebApi.Extension
{
    public static class ApiVersionAddExtensionHandler
    {
        public static IServiceCollection AddApiVersionHandler(this IServiceCollection services)
        {

            services.AddApiVersioning(options => options.ReportApiVersions = true);
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
