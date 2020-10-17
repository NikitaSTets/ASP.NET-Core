using ASP.NET_Core_Check.Constraint;
using ASP.NET_Core_Check.ParameterTransformers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            services.AddRouting(options =>
            {
                options.ConstraintMap.Add("customName", typeof(MyCustomConstraint));

                options.ConstraintMap["slugify"] = typeof(SlugifyParameterTransformer);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //for mapping when work only with route attributes
                //endpoints.MapControllers();

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller:slugify=Home}/{action:slugify=Index}/{id?}");

                endpoints.MapGet("/Test/{name:alpha}/{age:range(18, 99)}", async context =>
                {
                    var name = context.Request.RouteValues["name"];
                    var age = context.Request.RouteValues["age"];

                    await context.Response.WriteAsync($"Test {name}! Age = {age}");
                });

                endpoints.MapGet("/{message}", async context =>
                {
                    var message = context.Request.RouteValues["message"];

                    await context.Response.WriteAsync($"{message}");
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

                //endpoints.MapFallbackToPage("/Privacy");

                endpoints.MapRazorPages();
            });
        }
    }
}