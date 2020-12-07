using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_Core_Check.Controllers
{
    [Route("TestRoute")]
    public class RouteController : Controller
    {
        [Route("test")]
        public IActionResult Index()
        {
            return View();
        }
    }
}