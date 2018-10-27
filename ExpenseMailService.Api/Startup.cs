using System.Diagnostics.CodeAnalysis;
using ExpenseMailService.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

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

            services.AddMvc()
                .AddXmlSerializerFormatters()
                .AddXmlDataContractSerializerFormatters();

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IParseXmlService, ParseXmlService>();
        }

        public void Configure(IApplicationBuilder app, IApplicationLifetime appLifetime)
        {
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
