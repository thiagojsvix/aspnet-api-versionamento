
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Xunit;

namespace Versionamento.Tests
{
    public class RetornaApiVersion
    {
        private string WebAppName => "Versionamento.WebApi";
        private readonly HttpClient httpClient;

        public RetornaApiVersion()
        {
            var pathRoot = this.GetAppBasePath(this.WebAppName);
            var builder = new WebHostBuilder().UseContentRoot(pathRoot).UseEnvironment("Development").UseStartup(this.WebAppName);
            var testServer = new TestServer(builder);
            this.httpClient = testServer.CreateClient();
        }

        [Fact]
        public async Task Deve_Retorna_Aviso_Api_Depreciada()
        {
            //arrange            
            var responseMessage = await this.httpClient.GetAsync("api/v1.0/Produto");

            //act


            //assert
            responseMessage.Headers.GetValues("api-supported-versions").Single().Should().Be("1.0, 2.0");
            responseMessage.Headers.GetValues("api-deprecated-versions").Single().Should().Be("0.9");
        }

        private string GetAppBasePath(string applicationWebSiteName)
        {
            var binPath = Environment.CurrentDirectory;
            while (!string.Equals(Path.GetFileName(binPath), "bin", StringComparison.InvariantCultureIgnoreCase))
            {
                binPath = Path.GetFullPath(Path.Combine(binPath, ".."));
                if (string.Equals(Path.GetPathRoot(binPath), binPath, StringComparison.InvariantCultureIgnoreCase))
                    throw new Exception("Could not find bin directory for test library.");
            }

            var testLibPath = Path.GetFullPath(Path.Combine(binPath, ".."));
            var testPath = Path.GetFullPath(Path.Combine(testLibPath, ".."));
            var srcPath = Path.GetFullPath(Path.Combine(testPath, "..", "src"));

            if (!Directory.Exists(srcPath))
                throw new Exception("Could not find src directory.");

            var appBasePath = Path.Combine(srcPath, applicationWebSiteName);
            if (!Directory.Exists(appBasePath))
                throw new Exception("Could not find directory for application.");

            return appBasePath;
        }
    }
}
