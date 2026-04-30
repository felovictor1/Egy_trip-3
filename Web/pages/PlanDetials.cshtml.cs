using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using eg_travil.models;

namespace eg_travil.pages // Ensure this is lowercase 'pages' to match your error
{
    public class PlanDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public PlanDetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public ReadyPlans Plan { get; set; }

        // Added 'Task<IActionResult>' which is the return type
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Plan = await _context.ReadyPlans.FirstOrDefaultAsync(p => p.Id == id);

            if (Plan == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}