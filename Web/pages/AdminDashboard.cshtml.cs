using Microsoft.AspNetCore.Mvc.RazorPages;
using eg_travil.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace eg_travil.Pages
{
    public class AdminDashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<User> RegisteredUsers { get; set; } = new List<User>();
        public List<UserResponse> RecentResponses { get; set; } = new List<UserResponse>();

        public async Task OnGetAsync()
        {
            RegisteredUsers = await _context.Users.ToListAsync();
            RecentResponses = await _context.UserResponses
                .Include(t => t.User)
                .OrderByDescending(t => t.CreatedAt)
                .Take(10)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteTripAsync(int id)
        {
            var trip = await _context.UserResponses.FindAsync(id);
            if (trip != null)
            {
                _context.UserResponses.Remove(trip);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostExportCsvAsync()
        {
            var responses = await _context.UserResponses
                .Include(u => u.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
            
            var csv = new StringBuilder();
            csv.AppendLine("Email,Destinations,Days,Budget,Created At");

            foreach (var r in responses)
            {
                var email = (r.User?.Email ?? r.Email ?? "Anonymous").Replace(",", " ");
                var dests = (r.Destinations ?? "").Replace(",", " ");
                var days = r.Days;
                var budget = (r.Budget ?? "").Replace(",", " ");
                var createdAt = r.CreatedAt.ToLocalTime().ToString("g").Replace(",", " ");

                csv.AppendLine($"{email},{dests},{days},{budget},{createdAt}");
            }

            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", "TripResponses.csv");
        }

        public async Task<IActionResult> OnPostClearAllTripsAsync()
        {
            var allResponses = await _context.UserResponses.ToListAsync();
            _context.UserResponses.RemoveRange(allResponses);

            var allSavedPlans = await _context.SavedPlans.ToListAsync();
            _context.SavedPlans.RemoveRange(allSavedPlans);

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
