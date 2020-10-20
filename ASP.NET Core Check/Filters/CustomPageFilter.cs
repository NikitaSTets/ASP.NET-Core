using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace ASP.NET_Core_Check.Filters
{
    public class CustomPageFilter : Attribute, IPageFilter //, IAsyncPageFilter
    {
        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
            context.RouteData.Values.Add("Name", "test");
            context.RouteData.Values.Add("Year", 1999);

            //throw new System.NotImplementedException();
        }

        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            //context.RouteData.Values.Add("Year", 1999);
            //throw new System.NotImplementedException();
        }

        public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
           
        }

        //public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        //{
        //    await next();
        //    //throw new NotImplementedException();
        //}

        //public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        //{
        //    return Task.CompletedTask;
        //    //throw new NotImplementedException();
        //}
    }
}