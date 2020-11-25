using System;
using System.Threading.Tasks;
using ASP.NET_Core_Check.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace ASP.NET_Core_Check.Infrastructure.CustomModelBinding
{
    public class TestModelBinder : IModelBinder
    {
        private readonly ILogger<TestModelBinder> _logger;


        public TestModelBinder(ILogger<TestModelBinder> logger)
        {
            _logger = logger;
        }
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var idValue = bindingContext.ValueProvider.GetValue("Id");
            var nameValue = bindingContext.ValueProvider.GetValue("Name");


            int.TryParse(idValue.FirstValue, out int id);

            var result = new CustomModelBindingTest
            {
                Id = id,
                Name = "nameValue.FirstValue"
            };

            bindingContext.Result = ModelBindingResult.Success(result);

            return Task.CompletedTask;
        }
    }
}