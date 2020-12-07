using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ASP.NET_Core_Check
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var assemblyName = typeof(Startup).GetTypeInfo().Assembly.FullName;

            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("http://*:5000;http://localhost:5001;https://hostname:5002");
                    webBuilder.UseSetting(WebHostDefaults.ApplicationKey, "CustomApplicationName");

                    webBuilder.UseStartup(assemblyName);
                })
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.AddJsonFile("hostsettings.json", optional: true);
                    configHost.AddEnvironmentVariables(prefix: "TestPrefix_");
                })
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.Properties.Add("applicationName", "te");
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);

                    var b = hostContext.Configuration.GetValue<string>("AppPreConfigurationTestSetting");

                    config.AddEnvironmentVariables("TestPrefix_");

                    var a = new[] { "SomeValue=321", "ValueTest=4" };
                    config.AddCommandLine(a);
                });
        }
    }
}