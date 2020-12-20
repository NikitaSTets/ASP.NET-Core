using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ASP.NET_Core_Check.Infrastructure.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace ASP.NET_Core_Check.Infrastructure.Authorization.Handlers
{
    public class MinimumAgeRequirementHandler : AuthorizationHandler<MinimumAgeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == ClaimTypes.DateOfBirth))
            {
                return Task.CompletedTask;
            }

            var dateOfBirthString = context.User.FindFirst(c => c.Type == ClaimTypes.DateOfBirth).Value;
            var dateOfBirth = Convert.ToDateTime(dateOfBirthString);

            var calculatedAge = DateTime.Today.Year - dateOfBirth.Year;
            if (dateOfBirth > DateTime.Today.AddYears(-calculatedAge))
            {
                calculatedAge--;
            }

            if (calculatedAge >= requirement.MinimumAge)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}