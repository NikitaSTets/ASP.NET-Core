using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ASP.NET_Core_Check.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {

        }

        public ApplicationUser(string userName)
            : base(userName)
        {

        }


        [PersonalData]
        [MaxLength(50)]
        public string FullName { get; set; }


        [PersonalData]
        public DateTime? Birthday { get; set; }
    }
}