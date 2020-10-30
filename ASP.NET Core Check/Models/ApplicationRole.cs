using Microsoft.AspNetCore.Identity;

namespace ASP.NET_Core_Check.Models
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole()
        {

        }

        public ApplicationRole(string roleName)
            : base(roleName)
        {

        }
    }
}