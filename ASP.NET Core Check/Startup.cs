using ASP.NET_Core_Check.Constraint;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
               
                endpoints.MapRazorPages();
            });
        }
    }
}