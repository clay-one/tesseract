using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tesseract.Core;
using Tesseract.Worker.Services;

// ReSharper disable StringLiteralTypo

namespace Tesseract.Worker
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            await GetHostBuilder(args)
                .Build()
                .RunAsync();
        }

        private static IHostBuilder GetHostBuilder(string[] args)
        {
            return new HostBuilder()
                .ConfigureHostConfiguration(configBuilder =>
                {
                    configBuilder
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile(@"hostsettings.json", false)
                        .AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostBuilderContext, appConfigBuilder) =>
                {
                    var envName = hostBuilderContext.HostingEnvironment.EnvironmentName;
                    appConfigBuilder
                        .AddJsonFile(@"appsettings.json", false)
                        .AddJsonFile($@"appsettings.{envName}.json", true)
                        .AddEnvironmentVariables($@"TSRCT_{envName}")
                        .AddCommandLine(args);
                })
                .ConfigureServices((hostBuilderContext, serviceCollection) =>
                {
                    serviceCollection.AddTesseractCoreServices();
                    serviceCollection.AddHostedService<MonitorService>();
                })
                .ConfigureLogging((hostBuilderContext, loggingBuilder) =>
                {
                    loggingBuilder
                        .AddConfiguration(hostBuilderContext.Configuration)
                        .AddConsole();
                })
                .UseConsoleLifetime();
        }
    }
}