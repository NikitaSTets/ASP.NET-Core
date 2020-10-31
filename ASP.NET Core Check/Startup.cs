using System;
using System.Collections.Generic;
using ASP.NET_Core_Check.Constraint;
using ASP.NET_Core_Check.Filters;
using ASP.NET_Core_Check.HealthCheck;
using ASP.NET_Core_Check.Infrastructure;
using ASP.NET_Core_Check.Infrastructure.HostedServices;
using ASP.NET_Core_Check.Middlewares;
using ASP.NET_Core_Check.Models;
using ASP.NET_Core_Check.ParameterTransformers;
using ASP.NET_Core_Check.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace ASP.NET_Core_Check
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private static void HandleBranch(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                var branchVer = context.Request.Query["branch"];
                await context.Response.WriteAsync($"Branch used = {branchVer}");
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
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

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options
                    =>
                {
                    options.LoginPath = "/Account/Login/";
                    options.AccessDeniedPath = "/Account/Forbidden/";
                    options.LogoutPath = "/Account/Logout";
                    options.ReturnUrlParameter = "ReturnUrl";
                });
            services.AddHostedService<StartupHostedService>();
            services.AddSingleton<StartupHostedServiceHealthCheck>();

            services.AddHealthChecks()
                .AddDbContextCheck<ApplicationDbContext>(tags: new[] { "foo_tag" })
                .AddSqlServer(Configuration["ConnectionStrings:DefaultConnection"], "SELECT * FROM [aspnet-core-check].[dbo].[AspNetUsers]", "test", HealthStatus.Healthy, new[] { "example_tag" })
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

            services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")))
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                 {
                     options.SignIn.RequireConfirmedAccount = false;
                     options.Password.RequireDigit = false;
                     options.Password.RequireLowercase = false;
                     options.Password.RequiredUniqueChars = 0;
                     options.Password.RequiredLength = 0;
                     options.Password.RequireNonAlphanumeric = false;
                     options.Password.RequireUppercase = false;
                     options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                     options.Lockout.MaxFailedAccessAttempts = 10;
                 })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                options.SlidingExpiration = true;
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/Forbidden";
                options.LogoutPath = "/Account/Logout";
                options.ReturnUrlParameter = "ReturnUrl";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World From 2nd Middleware");
            //});

            //app.Use(async (context, next) =>
            //{
            //    await context.Response.WriteAsync("Hello World From 1st Middleware!");

            //    await next();
            //});

            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();

                //app.UseExceptionHandler("/Home/Error");

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
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseMvc();

            app.MapWhen(context => context.Request.Query.ContainsKey("branch"), HandleBranch);

            app.UseMiddleware<TestTokenMiddleware>();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //for mapping when work only with route attributes
                //endpoints.MapControllers();
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

                //endpoints.MapGet("/{message}", async context =>
                //{
                //    var message = context.Request.RouteValues["message"];

                //    await context.Response.WriteAsync($"{message}");
                //});

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
                //endpoints.MapFallbackToPage("/Privacy");

                endpoints.MapRazorPages();
            });
        }
    }
}