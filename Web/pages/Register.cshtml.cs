using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace eg_travil.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public string GoogleClientId { get; private set; }

        public RegisterModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
            GoogleClientId = _configuration["Authentication:Google:ClientId"];
        }
    }
}