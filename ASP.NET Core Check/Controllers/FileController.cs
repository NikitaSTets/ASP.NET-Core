using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace ASP.NET_Core_Check.Controllers
{
    public class FileController : Controller
    {
        private readonly IHostEnvironment _environment;


        public FileController(IHostEnvironment environment)
        {
            _environment = environment;
        }
        
        [Authorize]
        public IActionResult BannerImage()
        {
            var filePath = Path.Combine(
                _environment.ContentRootPath, "MyStaticFiles", "images", "red-rose.jpg");

            return PhysicalFile(filePath, "image/jpeg");
        }
    }
}