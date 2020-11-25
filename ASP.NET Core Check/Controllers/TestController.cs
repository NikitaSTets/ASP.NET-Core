using ASP.NET_Core_Check.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_Core_Check.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult OnPost(ModelValidationModel model)
        {
            if (ModelState.IsValid)
            {
                ViewData["Message"] = "Success !!!";
            }
            return View("Index", model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Test([FromForm]ModelValidationModel model)
        {
            var isValid = ModelState.IsValid;
            var b = model.Email == "dsa";

            return View("Index", model);
        }

        public IActionResult Test()
        {
            return View("Index");
        }

        [HttpGet]
        public IActionResult CustomBindingTest(CustomModelBindingTest bindingTest)
        {
            var a = bindingTest;

            return View("Index");
        }
        //[AcceptVerbs("GET", "POST")]
        //[AllowAnonymous]
        //public IActionResult VerifyEmail(string email)
        //{
        //    return string.IsNullOrEmpty(email)
        //        ? Json($"Email {email} is already in use.")
        //        : Json(true);
        //}
    }
}