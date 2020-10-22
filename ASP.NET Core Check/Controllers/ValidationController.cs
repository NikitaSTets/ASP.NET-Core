using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_Core_Check.Controllers
{
    public class ValidationController : Controller
    {
        [AcceptVerbs("GET", "POST")]
        [AllowAnonymous]
        public IActionResult VerifyEmail(string email)
        {
            return string.IsNullOrEmpty(email)
                ? Json($"Email {email} is already in use.")
                : Json(true);
        }
    }
}