using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ASP.NET_Core_Check.Constraint
{
    public class MyCustomConstraint : IRouteConstraint
    {
        private readonly Regex _regex;


        public MyCustomConstraint()
        {
            _regex = new Regex(@"^[1-9]*$",
                RegexOptions.CultureInvariant | RegexOptions.IgnoreCase,
                TimeSpan.FromMilliseconds(100));
        }


        public bool Match(HttpContext httpContext, IRouter route, string routeKey,
            RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (!values.TryGetValue(routeKey, out object value))
            {
                return false;
            }
            var parameterValueString = Convert.ToString(value,
                CultureInfo.InvariantCulture);

            return parameterValueString != null && _regex.IsMatch(parameterValueString);
        }
    }
}
