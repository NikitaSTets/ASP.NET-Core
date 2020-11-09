using ASP.NET_Core_Check.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_Core_Check.Areas.Service.Controllers
{
    public class ModelValidationController : Controller
    {
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ClientSideValidationModel model, [FromQuery] string test)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var message = "product " + model.Name + " created successfully";

            return Content(message);
        }
    }
}
