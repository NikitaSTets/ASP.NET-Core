using System;
using ASP.NET_Core_Check.Filters;
using ASP.NET_Core_Check.Models;
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

        [ServiceFilter(typeof(LogFilterAttribute), Order = 1)]
        public IActionResult LogFilterTest()
        {
            return Ok(1);
        }
    }
}