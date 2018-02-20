using System;
using ComposerCore.Implementation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tesseract.Common.ComposerStuff;

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
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            try
            {
                var composer = new ComponentContext();

                var serviceProvider = new ComposerServiceProvider(composer);

                return serviceProvider;
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
                app.Run(async context => { await context.Response.WriteAsync("Hello World!"); });
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Error occured during pipeline configuration.");
                throw;
            }
        }
    }
}