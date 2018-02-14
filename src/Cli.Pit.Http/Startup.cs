using System.Threading;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Tesseract.Cli.Pit.Http
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<DataCollector>();
            services.AddSingleton<DataReporter>();
        }

        public void Configure(IApplicationBuilder app, DataCollector dataCollector, DataReporter dataReporter)
        {
            new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    dataReporter.Report();
                }

                // ReSharper disable once FunctionNeverReturns
            }).Start();

            app.Run(async context =>
            {
                var data = new RequestData
                {
                    RemoteIp = context.Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                    Uri = context.Request.GetUri().ToString(),
                    Verb = context.Request.Method,
                    NumberOfAccounts = 1
                };
                dataCollector.Add(data);

                context.Response.StatusCode = 200;
                await context.Response.WriteAsync(string.Empty);
            });
        }
    }
}