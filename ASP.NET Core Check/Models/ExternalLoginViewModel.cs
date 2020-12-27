using System.ComponentModel.DataAnnotations;

namespace ASP.NET_Core_Check.Models
{
    public class ExternalLoginViewModel
    {
        public string ProviderDisplayName { get; set; }

        public string ReturnUrl { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}