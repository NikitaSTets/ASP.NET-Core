using System;
using ASP.NET_Core_Check.Filters;
using ASP.NET_Core_Check.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_Core_Check.Areas.Service.Controllers
{
    //[BindProperties(SupportsGet = false)]
    public class HomeController : Controller
    {
        public string ControllerProperty { get; set; }

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

        public IActionResult ModelBindingTest(ModelBindingTestModel testModel)
        {
            return View(testModel);
        }

        [HttpPost]
        public IActionResult ModelBindingTestBind([FromBody]TestBindAttributeModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception("Invalid model");
            }
            return View("Home/Test");
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ModelValidation([FromBody]ModelValidationModel model)
        {
            //model.Description = "Dsa";
            //model.Title = "dsa";
            if (!TryValidateModel(model, nameof(ModelValidationModel)))
            {
                return View("Home/Test");
            }
            if (!ModelState.IsValid)
            {
                throw new Exception("Invalid model");
            }
            return View("Home/Test");
        }
    }
}