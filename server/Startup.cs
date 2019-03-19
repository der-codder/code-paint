using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CodePaint.WebApi.Models;
using CodePaint.WebApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace CodePaint.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            // Init Serilog configuration
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var config = new ServerConfig();
            Configuration.Bind(config);

            services.AddHttpClient<IVSMarketplaceClient, VSMarketplaceClient>()
                .ConfigurePrimaryHttpMessageHandler(
                    conf => new HttpClientHandler
                    {
                        AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
                    }
                );

            var galleryContext = new GalleryContext(config.MongoDB);
            services.AddSingleton<IGalleryInfoRepository>(
                new GalleryInfoRepository(galleryContext)
            );
            services.AddSingleton<IGalleryStatisticsRepository>(
                new GalleryStatisticsRepository(galleryContext)
            );

            services.AddTransient<IGalleryRefreshService, GalleryRefreshService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // logging
            loggerFactory.AddSerilog();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
