using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Tesseract.Core;
using Tesseract.Web.CorrelationId;

namespace Tesseract.Web
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;

        public Startup(ILogger<Startup> logger)
        {
            _logger = logger;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddMvc()
                    .AddJsonOptions(options =>
                    {
                        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                        options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    });

                services.AddCorrelationId();
                services.AddTesseractCoreServices();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Error occured during ServiceProvider configuration.");
                throw;
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            try
            {
                app.UseCorrelationId(new CorrelationIdOptions { IncludeInResponse = false });
                app.UseMvc();
                app.Run(async context => { await context.Response.WriteAsync($"Tesseract ({env.EnvironmentName}) is up and running!"); });
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Error occured during pipeline configuration.");
                throw;
            }
        }
    }
}