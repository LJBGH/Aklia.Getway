using Aklia.GetWay.API.Startups;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aklia.GetWay.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            //����Ocelot
            services.AddOcelot(new ConfigurationBuilder()
                .AddJsonFile("configuration.json")
                .Build())
                .AddPolly()
                .AddConsul();

            services.AddSwaggerModule(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); 
            }

            app.UseSwaggerConfig();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //var configuration = new OcelotPipelineConfiguration
            //{
            //    PreQueryStringBuilderMiddleware = async (context, next) =>
            //    {
            //        var host = $"{context.Request.Scheme}://{context.Request.Host.Value}";
            //        context.Request.Headers.Add("OriginalHost", host);
            //        await next.Invoke();
            //    }
            //};
            //app.UseOcelot(configuration);

            app.UseOcelot().Wait();
        }
    }
}