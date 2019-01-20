using System;
using System.Collections.Generic;
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
using Swashbuckle.AspNetCore.Swagger;
using License = Swashbuckle.AspNetCore.Swagger.License;

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
                c.SwaggerDoc("v1", new Info {Title = "Expense API", Version = "v1"});
                var basePath = AppContext.BaseDirectory;
                var assemblyName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
                var xmlPath = Path.Combine(basePath, $"{assemblyName}.xml");
                c.IncludeXmlComments(xmlPath);
                c.MapType(typeof(ExpenseInputDto), () => new Schema
                {
                    Description = "XML-like content or XML content which will contain Total tag in it.",
                    Properties = new Dictionary<string, Schema>
                    {
                        {
                            "xml-like-content", new Schema
                            {
                                Example = "Hi Yvaine,\r\nPlease create an expense claim for the below. Relevant details are marked up as\r\nrequested…\r\n<expense><cost_centre>DEV002</cost_centre>\r\n<total>1024.01</total><payment_method>personal card</payment_method>\r\n</expense>\r\nFrom: Ivan Castle\r\nSent: Friday, 16 February 2018 10:32 AM\r\nTo: Antoine Lloyd <Antoine.Lloyd@example.com>\r\nSubject: test\r\nHi Antoine,\r\nPlease create a reservation at the <vendor>Viaduct Steakhouse</vendor> our\r\n<description>development team’s project end celebration dinner</description> on\r\n<date>Tuesday 27 April 2017</date>. We expect to arrive around\r\n7.15pm. Approximately 12 people but I’ll confirm exact numbers closer to the day.\r\nRegards,\r\nIvan\r"
                            }
                        }
                    }
                });
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
                {
#if DEBUG
                    swaggerDoc.Schemes = new[]
                    {
                        Uri.UriSchemeHttp
                    };
#else
                    swaggerDoc.Schemes = new[]
                    {
                        Uri.UriSchemeHttps

                    };
#endif
                });

                options.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                    swaggerDoc.Info = new Info()
                    {
                        Contact = new Contact()
                        {
                            Email = "mexanichp@gmail.com", Name = "Mykhailo",
                            Url = "https://www.linkedin.com/in/mykhailo-k-bb9705aa/"
                        },
                        Title = ".NET Core App made with ❤ by mexanich.",
                        License = new License
                            {Name = "MIT", Url = "https://github.com/mexanichp/ExpenseServicre/blob/master/LICENSE.md"}
                    });
            });

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Expense API");
                options.DocumentTitle = "ExpenseService API";
            });


            app.UseMvc();
        }
    }
}