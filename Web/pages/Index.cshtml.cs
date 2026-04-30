using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eg_travil.views
{
    public class IndexModel : PageModel
    {
        public string Message { get; set; } = "";
        public void OnGet()
        {
            Message = "Welcome to EG Travil! Your frontend is officially working.";
        }
    }
}
