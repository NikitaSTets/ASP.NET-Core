using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace ASP.NET_Core_Check.Filters
{
    public class LogFilterAttribute : IAsyncActionFilter
    {
        private readonly ILoggerFactory _loggerFactory;


        public LogFilterAttribute(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }


        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var logger = _loggerFactory.CreateLogger(context.Controller.GetType());
            logger.LogTrace($"{context.ActionDescriptor.DisplayName}action called");

            //OnActionExecutionExecuting

            await next(); //need to pass the execution to next

            //OnActionExecutionExecuted
        }
    }
}