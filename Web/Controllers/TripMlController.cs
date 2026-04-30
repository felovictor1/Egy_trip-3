using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text;
using eg_travil.models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace eg_travil.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripMlController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public TripMlController(ApplicationDbContext context, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpPost("Generate")]
        // [Authorize] // Uncomment if you require users to be logged in to generate trips
        public async Task<IActionResult> GenerateTrip([FromBody] UserPreferences request)
        {
            if (request == null)
            {
                return BadRequest("Invalid trip request payload.");
            }

            // 1. Data Preprocessing & Additional Context (SQL Server)
            // Save user preferences to the database for historical tracking/analytics
            string? userEmail = null;
            int? userId = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                userEmail = User.FindFirstValue(ClaimTypes.Email);
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdString, out int parsedId))
                {
                    userId = parsedId;
                    
                    var dbUser = await _context.Users.FindAsync(userId);
                    if (dbUser != null && dbUser.IsBanned)
                    {
                        return StatusCode(403, "Your account has been suspended.");
                    }
                }
            }

            var userResponse = new UserResponse
            {
                Email = userEmail,
                CreatedAt = DateTime.UtcNow,
                Destinations = request.Destinations != null ? string.Join(", ", request.Destinations) : "",
                Days = request.Days,
                Budget = request.Budget,
                PreferencesJson = JsonSerializer.Serialize(request),
                UserId = userId
            };

            _context.UserResponses.Add(userResponse);
            await _context.SaveChangesAsync();

            // Transform the data into the exact format the Python repository's model expects
            var pythonPayload = new
            {
                destinations = request.Destinations,
                budget = request.Budget,
                groupSize = (request.Travelers?.Adults ?? 1) + (request.Travelers?.Kids ?? 0),
                startDate = request.StartDate,
                endDate = request.EndDate,
                travelStyles = request.TravelStyles,
                historicalKnowledge = request.HistoricalKnowledge ?? "Beginner",
                preferredTimePeriods = request.PreferredTourismStyle,
                museumVisits = request.MuseumPreferences ?? true,
                waterActivities = request.WantsWaterActivities ?? false,
                accommodationType = request.Accommodation ?? "Medium",
                transportation = request.TransportationPreference ?? "Private Car",
                foodPreferences = request.FoodPreferences ?? "No Preference",
                tripPace = request.Intensity ?? "Moderate",
                mustVisit = request.MustVisit ?? ""
            };

            // 2. Inference & Response (Calling the Python ML Microservice)
            var mlServiceUrl = _configuration["MlServiceUrl"] ?? "http://127.0.0.1:5000";
            var httpClient = _httpClientFactory.CreateClient();
            
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(pythonPayload), Encoding.UTF8, "application/json");
                
                // POST to the Python Flask API
                var response = await httpClient.PostAsync($"{mlServiceUrl.TrimEnd('/')}/api/trip", content);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"\n🚨🚨🚨 ML Service Failed: {response.StatusCode} 🚨🚨🚨");
                    Console.WriteLine($"Error Details: {errorResponse}\n");
                    return StatusCode(500, $"ML Service Error: {errorResponse}");
                }

                // Format the result into a clean JSON response for the frontend
                var mlResponseJson = await response.Content.ReadAsStringAsync();
                var tripPlan = JsonSerializer.Deserialize<object>(mlResponseJson); // Pass through as object
                
                if (userId.HasValue)
                {
                    var destinations = request.Destinations != null && request.Destinations.Any() ? string.Join(", ", request.Destinations) : "Egypt";
                    var savedPlan = new SavedPlan
                    {
                        UserId = userId.Value,
                        Title = $"Trip to {destinations} - {request.Days} Days",
                        PlanData = mlResponseJson,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.SavedPlans.Add(savedPlan);
                    await _context.SaveChangesAsync();
                }

                return Ok(tripPlan);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(503, $"Could not connect to the ML Service. Ensure the Python API is running. Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred during ML inference: {ex.Message}");
            }
        }
    }
}
