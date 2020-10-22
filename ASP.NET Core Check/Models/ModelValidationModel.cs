using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_Core_Check.Models
{
    public class ModelValidationModel
    {
        [Required]
        public string ClientName { get; set; }

        [DataType(DataType.Date)]
        //[FutureDate(ErrorMessage = "Please enter a date in the future")]
        [Remote("ValidateDate", "ModelValidation")]
        public DateTime Date { get; set; }

        public int Id { get; set; }

        //[Required]
        [StringLength(100, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 6)]
        public string Title { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Release Date")]
        public DateTime ReleaseDate { get; set; }

        //[Required]
        [StringLength(1000)]
        public string Description { get; set; }

        [Range(0, 999.99)]
        public decimal Price { get; set; }

        public bool Preorder { get; set; }

        [Required]
        [Remote(action: "VerifyEmail", controller: "Validation", ErrorMessage = "Please enter a valid email.")]
        public string Email { get; set; }
    }
}