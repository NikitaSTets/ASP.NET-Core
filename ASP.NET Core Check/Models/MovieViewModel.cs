using ASP.NET_Core_Check.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASP.NET_Core_Check.Models
{
    [CustomPageFilter]
    public class MovieViewModel : PageModel
    {
        public string Name { get; set; }

        public int Year { get; set; }

        public void OnGet(string name, int year)
        {
            Name = name;
            Year = year;
        }
    }
}
