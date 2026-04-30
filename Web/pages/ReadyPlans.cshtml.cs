using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using eg_travil.models;

namespace eg_travil.pages
{
    public class ReadyPlansModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ReadyPlansModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ReadyPlans> ReadyPlans { get; set; } = new();

        public async Task OnGetAsync()
        {
            // This fetches the data from the TravelyDb database
            ReadyPlans = await _context.ReadyPlans.ToListAsync();
        }
    }
}
