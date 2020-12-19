using System.Threading.Tasks;
using ASP.NET_Core_Check.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ASP.NET_Core_Check.Controllers
{
    public class AccountController : Controller
    {
        private readonly IOptions<CookieAuthenticationOptions> _options;
        private readonly SignInManager<ApplicationUser> _signInManager;


        public AccountController(
            IOptions<CookieAuthenticationOptions> options,
            SignInManager<ApplicationUser> signInManager)
        {
            _options = options;
            _signInManager = signInManager;
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PerformLogin(string username, string password, string returnUrl)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }

            if (!result.IsLockedOut)
            {
                return Redirect(_options.Value.AccessDeniedPath);
            }
            ModelState.AddModelError("User", "User is locked out");

            return View("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToRoute("Default");
        }
    }
}