using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppConstants = ASP.NET_Core_Check.Constants.Constants;

namespace ASP.NET_Core_Check.Areas.Service.Controllers
{
    public class AuthorizationTestingController : Controller
    {
        [Authorize(AuthenticationSchemes = "Google")]
        public IActionResult Admin()
        {
            return View();
        }

        [Authorize(Roles = "Manager", AuthenticationSchemes = "Google")]
        public IActionResult Management()
        {
            return View();
        }

        [Authorize(Policy = AppConstants.Policies.EmployeeOnly)]
        public IActionResult EmployeeOnly()
        {
            return View();
        }

        [Authorize(Policy = AppConstants.Policies.Founders)]
        public IActionResult Founders()
        {
            return View();
        }

        [Authorize(Policy = AppConstants.Policies.MinimumAge21)]
        public IActionResult MinimumAgeTest()
        {
            return View();
        }
    }
}