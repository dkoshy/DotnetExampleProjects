using CourseLibrary.API.Services;
using Library.API.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace WebApp.SampleRESTAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConfiguration _configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(setup =>
            {
                setup.ReturnHttpNotAcceptable = true;
                //setup.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                setup.OutputFormatters.RemoveType<SystemTextJsonOutputFormatter>();
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            }).AddXmlDataContractSerializerFormatters()
            .ConfigureApiBehaviorOptions(setupActions =>
            {
                setupActions.InvalidModelStateResponseFactory = conext =>
                {
                    var problemDetailsFactory = conext.HttpContext.RequestServices
                                               .GetRequiredService<ProblemDetailsFactory>();
                    var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(conext.HttpContext,
                        conext.ModelState);

                    problemDetails.Detail = "See error Details for more information";
                    problemDetails.Instance = conext.HttpContext.Request.Path;

                    //findout which status code to use

                    var actionExectionContext =
                       conext as ActionExecutingContext;

                    if((conext.ModelState.ErrorCount > 0 ) && 
                    (actionExectionContext?.ActionArguments.Count == 
                    conext.ActionDescriptor.Parameters.Count))
                    {
                        problemDetails.Type = "https://courselibrary.com/modelvalidationproblem";
                        problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                        problemDetails.Title = "One or more Validation Error happend ";


                        return new UnprocessableEntityObjectResult(problemDetails)
                        {
                            ContentTypes = { "application/problem+json" }
                        };
                    }

                    // if one of the keys wasn't correctly found / couldn't be parsed
                    // we're dealing with null/unparsable input

                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Title = "One or more Erors on input is Occursed";

                    return new BadRequestObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json" }
                    };
                };

            });



            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApplication1", Version = "v1" });
            });

            services.AddDbContext<CourseLibraryContext>(op =>
            {
                op.UseSqlServer(_configuration.GetConnectionString("dbConnection"));
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<ICourseLibraryRepository, CourseLibraryRepository>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApplication1 v1"));
            }
            else
            {
                app.UseExceptionHandler(appBuilder => {

                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("A Failer happend in the Server Side. Please Try again later");
                    });
                
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
