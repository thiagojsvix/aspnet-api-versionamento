using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Versionamento.WebApi.HelthCheck;

namespace Versionamento.WebApi.Extension
{
    public static class HelthCheckAddExtensionHandler
    {
        public static IServiceCollection AddHelpCheckHandler(this IServiceCollection services)
        {
            var _drives = DriveInfo.GetDrives();
            var testDrive = _drives.FirstOrDefault(d => d.DriveType == DriveType.Fixed);
            var testDriveActualFreeMegabytes = testDrive.AvailableFreeSpace / 1024 / 1024;
            var targetFreeSpace = testDriveActualFreeMegabytes - 50;

            const long memoryMaximo = 200 * 1024 * 1024;

            var currentPrivateMemory = Process.GetCurrentProcess().PrivateMemorySize64;
            var currentVirtualMemory = Process.GetCurrentProcess().VirtualMemorySize64;

            var maximumPrivateMemory = currentPrivateMemory + memoryMaximo;
            var maximumVirtualMemory = currentVirtualMemory + memoryMaximo;

            services
                .AddHealthChecksUI()
                .AddHealthChecks()
                .AddMemoryHealthCheck("memory")
                .AddDiskStorageHealthCheck(setup => setup.AddDrive(testDrive.Name, targetFreeSpace))
                .AddVirtualMemorySizeHealthCheck(maximumVirtualMemory)
                .AddPrivateMemoryHealthCheck(maximumPrivateMemory)
                .AddCheck<RandomHealthCheck>("random")
                .AddUrlGroup(new Uri("http://httpbin.org/status/200"), "httpbin.org");


            return services;
        }

        public static void UseHealthChecksHandlers(this IApplicationBuilder app)
        {
            app.UseHealthChecks("/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecksUI(config => config.UIPath = "/hc-ui");
        }

        public class RandomHealthCheck : IHealthCheck
        {
            public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
            {
                return Task.FromResult(DateTime.UtcNow.Minute % 2 == 0 ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy(description: "failed"));
            }
        }
    }
}
