using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using ExpenseMailService.Api.Models;
using ExpenseMailService.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace ExpenseMailService.Api
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(ILogger<Startup> logger, IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services.AddWebEncoders();

            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Expense API", Version = "v1" });
                var basePath = AppContext.BaseDirectory;
                var assemblyName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
                var xmlPath = Path.Combine(basePath, $"{assemblyName}.xml");
                c.IncludeXmlComments(xmlPath);
            });

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IParseXmlService, ParseXmlService>();
        }

        public void Configure(IApplicationBuilder app, IApplicationLifetime appLifetime)
        {
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseSwagger(options =>
            {
                options.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                    swaggerDoc.Host = httpReq.Host.Value);
                options.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                    swaggerDoc.Schemes = new[] { Uri.UriSchemeHttp });
            });

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Expense API");
            });
        

            app.UseMvc();
        }
    }
}
