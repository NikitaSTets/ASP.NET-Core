using System;
using System.IO;
using ASP.NET_Core_Check.Constraint;
using ASP.NET_Core_Check.Filters;
using ASP.NET_Core_Check.HealthCheck;
using ASP.NET_Core_Check.Infrastructure;
using ASP.NET_Core_Check.Infrastructure.HostedServices;
using ASP.NET_Core_Check.Middlewares;
using ASP.NET_Core_Check.Models;
using ASP.NET_Core_Check.ParameterTransformers;
using ASP.NET_Core_Check.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace ASP.NET_Core_Check
{
    public class StartupTesting
    {

        public void ConfigureServices(IServiceCollection services)
        {
            //for test CaptureStartupErrors
            //throw new Exception();
            services.AddRazorPages(options =>
            {
                //options.Conventions.AddFolderApplicationModelConvention(
                //    "/Movie",
                //    model => model.Filters.Add(new CustomPageFilter()));
            })
                //.AddXmlSerializerFormatters()
                .AddMvcOptions(options =>
                {
                    options.EnableEndpointRouting = false;
                    options.MaxModelValidationErrors = 50;
                    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(
                        _ => "The field is required.");
                });

            services.AddRouting(options =>
            {
                options.ConstraintMap.Add("customName", typeof(MyCustomConstraint));

                options.ConstraintMap["slugify"] = typeof(SlugifyParameterTransformer);
            });

            services.AddSingleton<LogFilterAttribute>();

            services.AddHostedService<LifetimeEventsHostedService>();

            services.AddHostedService<StartupHostedService>();
            services.AddSingleton<StartupHostedServiceHealthCheck>();

            services.AddHealthChecks()
                .AddDbContextCheck<ApplicationDbContext>(tags: new[] { "foo_tag" })
                .AddCheck<ExampleHealthCheck>("example_health_check", HealthStatus.Unhealthy, new[] { "example_tag" })
                .AddCheck("Foo", () => HealthCheckResult.Healthy("Foo is OK!"), tags: new[] { "foo_tag" })
                .AddCheck("Bar", () => HealthCheckResult.Unhealthy("Bar is unhealthy!"), tags: new[] { "bar_tag" })
                .AddCheck("Baz", () => HealthCheckResult.Healthy("Baz is OK!"), tags: new[] { "baz_tag" })
                .AddCheck<StartupHostedServiceHealthCheck>("hosted_service_startup", failureStatus: HealthStatus.Degraded, tags: new[] { "ready" });

            services.Configure<HealthCheckPublisherOptions>(options =>
            {
                options.Delay = TimeSpan.FromSeconds(2);
                options.Predicate = (check) => check.Tags.Contains("ready");
            });

            services.AddSingleton<IHealthCheckPublisher, ReadinessPublisher>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("EmployeeOnly", policy => policy.RequireClaim("EmployeeNumber"));
                options.AddPolicy("Founders", policy => policy.RequireClaim("EmployeeNumber", "1", "2", "3", "4", "5"));
            });

            services.AddDirectoryBrowser();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var errorFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exception = errorFeature.Error; //you may want to check what
                                                        //the exception is
                        var path = errorFeature.Path;
                    await context.Response.WriteAsync("Error: " + exception.Message);
                });
            });

            app.UseHttpsRedirection();

            app.UseMvc();

            app.UseMiddleware<TestTokenMiddleware>();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseStaticFiles();

            var provider = new FileExtensionContentTypeProvider();
            // Add new mappings
            provider.Mappings[".myapp"] = "application/x-msdownload";
            provider.Mappings[".htm3"] = "text/html";
            provider.Mappings[".image"] = "image/png";
            // Replace an existing mapping
            provider.Mappings[".rtf"] = "application/x-msdownload";
            // Remove MP4 videos.
            provider.Mappings.Remove(".mp4");

            const string cacheMaxAge = "604800";
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "MyStaticFiles")),
                RequestPath = "/StaticFiles",
                ContentTypeProvider = provider,
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={cacheMaxAge}");
                }
            });

            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "MyStaticFiles/Images")),
                RequestPath = "/MyImages"
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    Predicate = (check) => check.Tags.Contains("foo_tag")
                                           || check.Tags.Contains("baz_tag")
                                           || check.Tags.Contains("example_tag"),
                    ResultStatusCodes =
                    {
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Degraded] = StatusCodes.Status200OK,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    },
                    ResponseWriter = ResponseWriters.WriteResponse
                })
                    .RequireHost("localhost:44363");

                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
                {
                    Predicate = (check) => check.Tags.Contains("ready"),
                });

                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
                {
                    Predicate = (_) => false
                });


                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller:slugify=Home}/{action:slugify=Index}/{id?}");

                endpoints.MapGet("/Test/{name:alpha}/{age:range(18, 99)}", async context =>
                {
                    var name = context.Request.RouteValues["name"];
                    var age = context.Request.RouteValues["age"];

                    await context.Response.WriteAsync($"Test {name}! Age = {age}");
                });

                endpoints.MapGet("/{message:int}", async context =>
                {
                    var message = context.Request.RouteValues["message"];

                    await context.Response.WriteAsync($"{message}");
                });

                endpoints.MapGet("/Lol/{id:customName}", async context =>
                {
                    await context.Response.WriteAsync($"Test test!");
                });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "admin",
                    pattern: "admin/{controller}/{action=Index}");

                endpoints.MapAreaControllerRoute(
                    name: "store_area",
                    areaName: "service",
                    pattern: "service/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "identity",
                    pattern: "identity/{controller}/{action=Index}");

                endpoints.MapRazorPages();
            });
        }
    }
}