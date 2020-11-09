using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_Core_Check.Areas.Service.Controllers
{
    public class AuthorizationTestingController : Controller
    {
        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            return View();
        }

        [Authorize(Roles = "Manager")]
        public IActionResult Managment()
        {
            return View();
        }

        [Authorize(Policy = "EmployeeOnly")]
        public IActionResult EmployeeOnly()
        {
            return View();
        }

        [Authorize(Policy = "Founders")]
        public IActionResult Founders()
        {
            return View();
        }
    }
}