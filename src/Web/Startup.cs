using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog.Web;
using Tesseract.Core;
using Tesseract.Core.Logic;
using Tesseract.Core.MultiTenancy;

namespace Tesseract.Web
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;

        public IConfiguration Configuration { get; }

        public Startup(ILogger<Startup> logger, IConfiguration configuration, IHostingEnvironment environment)
        {
            _logger = logger;
            Configuration = configuration;
            environment.ConfigureNLog($"nlog.{environment.EnvironmentName}.config");
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

                services.Configure<MongoDbConfig>(Configuration.GetSection("MongoDb"));
                services.Configure<RedisConfig>(Configuration.GetSection("Redis"));
                services.Configure<ElasticsearchConfig>(Configuration.GetSection("Elasticsearch"));

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
                //app.UseCorrelationId(new CorrelationIdOptions { IncludeInResponse = false });
                app
                    .Use(async (ctx, next) =>
                    {
                        //var factory = ctx.RequestServices.GetService<ITenantContextFactory>();
                        //factory.Create();

                        var tca = ctx.RequestServices.GetService<ITenantContextAccessor>();
                        tca.TenantContext = new TenantContext(new Core.Context.TenantContextInfo("fanap-plus"));

                        await next();

                        // todo: maybe assing NULL to TenantContextAccessor's TenantContext?
                    })
                    .Use(async (ctx, next) =>
                    {
                        var srv = ctx.RequestServices.GetService<ICurrentTenantLogic>();
                        await srv.PopulateInfo();

                        await next();
                    })
                    .UseMvc()
                    .Run(async context =>
                    {
                        await context.Response.WriteAsync($"Tesseract ({env.EnvironmentName}) is up and running!");
                    });
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Error occured during pipeline configuration.");
                throw;
            }
        }
    }
}