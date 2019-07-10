using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Versionamento.WebApi.Extension;

[assembly: ApiController]
[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace Versionamento.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => options.OutputFormatters.RemoveType<StringOutputFormatter>())
                    .SetCompatibilityVersion(CompatibilityVersion.Latest)
                    .AddXmlSerializerFormatters();

            services.AddApiVersionHandler()
                    .AddSwaggerHandler()
                    //.AddHelpCheckHandler()
                    .AddGlobalExceptionHandler();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider provider)
        {
            app.UseDeveloperExceptionPage();
            app.UseSwaggerHandler(provider);
            //app.UseHealthChecksHandlers();
            app.UseGlobalExceptionHandler();

            app.UseMvc();
            app.UseHttpsRedirection();
        }
    }
}
