using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Versionamento.WebApi.Middlewares;

namespace Versionamento.WebApi.Extension
{
    public static class GlobalExceptionHandlerExtensions
    {
        public static IServiceCollection AddGlobalExceptionHandler(this IServiceCollection services)
        {
            return services.AddTransient<GlobalExceptionHandlerMiddleware>();
        }

        public static void UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    }
}
