using System.Threading.Tasks;
using ASP.NET_Core_Check.Infrastructure.Authorization.Requirements;
using ASP.NET_Core_Check.Models;
using Microsoft.AspNetCore.Authorization;

namespace ASP.NET_Core_Check.Infrastructure.Authorization.Handlers
{
    public class RoomAccessEmployeeAuthorizationHandler : AuthorizationHandler<RoomAccessRequirement, Room>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoomAccessRequirement requirement, Room lounge)
        {
            if (context.User.HasClaim(claim => claim.Type == "Employee"))
            {
                context.Succeed(requirement);
            }

            return Task.FromResult(0);
        }
    }
}