using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;

namespace ASP.NET_Core_Check.Models
{
    public class AccountLoginViewModel
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
    }
}