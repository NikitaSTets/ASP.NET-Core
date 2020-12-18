using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ASP.NET_Core_Check.Middlewares
{
    public class TestTokenMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var token = context.Request.Query["token"];
            if (token == "12345678")
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Token is invalid");
            }
            else
            {
                await next.Invoke(context);
            }
        }
    }
}