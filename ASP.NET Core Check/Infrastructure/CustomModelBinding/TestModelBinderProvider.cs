using ASP.NET_Core_Check.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ASP.NET_Core_Check.Infrastructure.CustomModelBinding
{
    public class TestModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(typeof(TestModelBinder));
            var a = logger as ILogger<TestModelBinder>;

            IModelBinder binder = new TestModelBinder(a);

            return context.Metadata.ModelType == typeof(CustomModelBindingTest) ? binder : null;
         }
    }
}