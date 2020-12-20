using System.Security.Claims;
using System.Threading.Tasks;
using ASP.NET_Core_Check.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ASP.NET_Core_Check.Infrastructure.Authorization
{
    public class MyUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
    {
        public MyUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            if (user.ContactName != null)
            {
                identity.AddClaim(new Claim("ContactName", user.ContactName));
            }

            return identity;
        }
    }
}