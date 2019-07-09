using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.SwaggerGen;
using Versionamento.WebApi.Swagger;

namespace Versionamento.WebApi.Extension
{
    public static class SwaggerAddExtensionHandler
    {
        public static IServiceCollection AddSwaggerHandler(this IServiceCollection services)
        {
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddSwaggerGen(
                options =>
                {
                    options.OperationFilter<SwaggerDefaultValues>();
                    options.IncludeXmlComments(XmlCommentsFilePath);
                });


            return services;
        }

        public static void UseSwaggerHandler(this IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "docs";
                foreach (var description in provider.ApiVersionDescriptions)
                    c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
            });

            app.UseSwagger();
        }


        private static string XmlCommentsFilePath
        {
            get {
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine(basePath, fileName);
            }
        }
    }
}
