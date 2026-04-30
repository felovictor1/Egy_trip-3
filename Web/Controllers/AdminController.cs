using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using eg_travil.models;
using System.Linq;

namespace eg_travil.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard-stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            // Verify admin email from JWT claim
            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (userEmail != "felopater@gmail.com")
            {
                return Forbid(); // Return 403 if not admin
            }

            var responses = await _context.UserResponses.ToListAsync();
            var totalResponses = responses.Count;
            
            var popularDestinations = responses
                .Where(r => !string.IsNullOrEmpty(r.Destinations))
                .SelectMany(r => r.Destinations.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                .GroupBy(d => d)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => new { Destination = g.Key, Count = g.Count() })
                .ToList();

            var avgDays = totalResponses > 0 ? responses.Average(r => r.Days) : 0;

            var popularBudgets = responses
                .Where(r => !string.IsNullOrEmpty(r.Budget))
                .GroupBy(r => r.Budget)
                .OrderByDescending(g => g.Count())
                .Select(g => new { Budget = g.Key, Count = g.Count() })
                .ToList();

            var recentResponses = responses.OrderByDescending(r => r.CreatedAt).Take(10).ToList();

            return Ok(new 
            {
                TotalResponses = totalResponses,
                PopularDestinations = popularDestinations,
                AverageDays = Math.Round(avgDays, 1),
                PopularBudgets = popularBudgets,
                RecentResponses = recentResponses
            });
        }

        public class BanRequest
        {
            public int? UserId { get; set; }
            public string? Email { get; set; }
        }

        [HttpPost("toggle-ban")]
        public async Task<IActionResult> ToggleBan([FromBody] BanRequest request)
        {
            // Verify admin email from JWT claim
            var adminEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (adminEmail != "felopater@gmail.com")
            {
                return Forbid(); // Return 403 if not admin
            }

            eg_travil.models.User? targetUser = null;

            if (request.UserId.HasValue)
            {
                targetUser = await _context.Users.FindAsync(request.UserId.Value);
            }
            else if (!string.IsNullOrEmpty(request.Email))
            {
                targetUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            }

            if (targetUser == null)
            {
                return NotFound(new { message = "User not found." });
            }

            targetUser.IsBanned = !targetUser.IsBanned;
            await _context.SaveChangesAsync();

            return Ok(new 
            { 
                message = $"User {(targetUser.IsBanned ? "banned" : "unbanned")} successfully.", 
                isBanned = targetUser.IsBanned 
            });
        }
    }
}
