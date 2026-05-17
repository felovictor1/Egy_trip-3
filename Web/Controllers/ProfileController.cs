using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using eg_travil.models;

namespace eg_travil.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
                return Unauthorized();

            var user = await _context.Users
                .Select(u => new { u.Id, u.FullName, u.Username, u.Email, u.Age, u.PhoneNumber })
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound();

            return Ok(user);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
                return Unauthorized();

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            user.Age = request.Age;
            user.PhoneNumber = request.PhoneNumber;
            
            if (!string.IsNullOrWhiteSpace(request.Username))
                user.Username = request.Username;
                
            if (!string.IsNullOrWhiteSpace(request.FullName))
                user.FullName = request.FullName;
            
            await _context.SaveChangesAsync();

            return Ok(new { message = "Profile updated successfully" });
        }

        [HttpGet("my-plans")]
        public async Task<IActionResult> GetMyPlans()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
                return Unauthorized();

            var plans = await _context.SavedPlans
                .AsNoTracking()
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new {
                    p.Id,
                    p.Title,
                    p.CreatedAt,
                    p.PlanData,
                    p.HotelName,
                    p.HotelDetails,
                    p.HotelPricePerNight,
                    p.Nights,
                    p.IsPaid,
                    p.TripEndDate,
                    p.ReviewRating,
                    p.ReviewText,
                    p.ReviewedAt,
                    Activities = p.Activities
                        .OrderBy(a => a.DayNumber)
                        .ThenBy(a => a.SequenceOrder)
                        .Select(a => new {
                            a.Id,
                            a.DayNumber,
                            a.SequenceOrder,
                            a.TimeLabel,
                            a.Place,
                            a.Description,
                            a.TicketPrice,
                            a.ScheduledTime,
                            a.Transport
                        })
                })
                .ToListAsync();

            return Ok(plans);
        }

        /// <summary>Submit or update a review for a completed trip.</summary>
        [HttpPost("review/{planId}")]
        public async Task<IActionResult> SubmitReview(int planId, [FromBody] ReviewRequest req)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            if (req.Rating < 1 || req.Rating > 5)
                return BadRequest(new { success = false, message = "Rating must be 1–5." });

            var plan = await _context.SavedPlans
                .FirstOrDefaultAsync(p => p.Id == planId && p.UserId == userId);

            if (plan == null) return NotFound(new { success = false });

            // Only allow review on or after the trip end date (allowing 1 day for timezone differences)
            if (plan.TripEndDate.HasValue && plan.TripEndDate.Value.Date > DateTime.UtcNow.AddDays(1).Date)
                return BadRequest(new { success = false, message = "Trip has not ended yet." });

            plan.ReviewRating = req.Rating;
            plan.ReviewText   = req.ReviewText?.Trim();
            plan.ReviewedAt   = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }
    }

    public class ReviewRequest
    {
        public int Rating { get; set; }
        public string? ReviewText { get; set; }
    }

    public class UpdateProfileRequest
    {
        public int? Age { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Username { get; set; }
        public string? FullName { get; set; }
    }
}
