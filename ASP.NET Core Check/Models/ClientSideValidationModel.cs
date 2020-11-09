using System.ComponentModel.DataAnnotations;

namespace ASP.NET_Core_Check.Models
{
    public class ClientSideValidationModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter the name")]
        [StringLength(maximumLength: 25, MinimumLength = 10, ErrorMessage = "Length must be between 10 to 25")]
        public string Name { get; set; }
    }
}