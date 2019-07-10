using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Versionamento.WebApi.Middlewares
{
    public class GlobalExceptionHandlerMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalExceptionHandlerMiddleware> logger;
        private readonly IHostingEnvironment environment;

        public GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger, IHostingEnvironment environment)
        {
            this.logger = logger;
            this.environment = environment;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await this.HandlerAsync(context, ex);
            }
        }

        private async Task HandlerAsync(HttpContext context, Exception ex)
        {
            var problemDetails = new TraceProblemaDetails
            {
                Instance = context.Request.HttpContext.Request.Path
            };

            if (ex is BadHttpRequestException badHttpRequestException)
            {
                problemDetails.Type = "https://httpstatuses.com/400";
                problemDetails.Title = "Requisicao invalida";
                problemDetails.Status = StatusCodes.Status400BadRequest;
                problemDetails.Detail = badHttpRequestException.Message;
            }
            else
            {
                problemDetails.Type = "https://httpstatuses.com/500";
                problemDetails.Title = ex.Message;
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Detail = ex.ToString();
            }

            context.Response.StatusCode = problemDetails.Status.Value;
            context.Response.ContentType = "application/problem+json";

            var jsonLog = JsonConvert.SerializeObject(problemDetails, Formatting.Indented);
            if (this.environment.IsProduction()) problemDetails.Detail = null;
            this.logger.LogWarning(jsonLog);

            var json = JsonConvert.SerializeObject(problemDetails, Formatting.Indented);
            await context.Response.WriteAsync(json);
        }
    }

    internal class TraceProblemaDetails : ProblemDetails
    {
        public string TraceId => Guid.NewGuid().ToString();
        public string TraceTime => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff");
    }
}
