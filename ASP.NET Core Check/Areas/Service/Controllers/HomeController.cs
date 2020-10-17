using System;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_Core_Check.Areas.Service.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("TestException")]
        public IActionResult TestException()
        {
            throw new Exception();
            return View();
        }
    }
}