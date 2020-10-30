using System.Security.Claims;
using System.Threading.Tasks;
using ASP.NET_Core_Check.Models;
using Microsoft.AspNetCore.Authentication;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;


        public AccountController(
            IOptions<CookieAuthenticationOptions> options,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _options = options;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PerformLogin(string username,
            string password, string returnUrl)
        {
            var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }

            if (!result.IsLockedOut)
            {
                //TODO what is it???
                return Redirect(_options.Value.AccessDeniedPath);
            }
            ModelState.AddModelError("User", "User is locked out");

            return View("Login");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToRoute("Default");
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            //the current user properties
            return await _userManager.GetUserAsync(HttpContext.User);
        }

        private async Task<ApplicationRole> GetUserRoleAsync(string id)
        {
            //the role for the given user
            return await _roleManager.FindByIdAsync(id);
        }
    }
}