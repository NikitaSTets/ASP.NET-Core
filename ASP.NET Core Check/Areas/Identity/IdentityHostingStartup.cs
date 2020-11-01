using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(ASP.NET_Core_Check.Areas.Identity.IdentityHostingStartup))]
namespace ASP.NET_Core_Check.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => { });
        }
    }
}