using eg_travil.models;
using eg_travil.servecies;
using eg_travil.services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Security.Claims;

namespace eg_travil.Pages
{
    public class PlannerModel : PageModel
    {
        private readonly GeminiService _geminiService;
        private readonly ApplicationDbContext _context;
        private readonly BudgetCalculatorService _budgetService;
        private readonly eg_travil.services.WeatherService _weatherService;

        public PlannerModel(GeminiService geminiService, ApplicationDbContext context, BudgetCalculatorService budgetService, eg_travil.services.WeatherService weatherService)
        {
            _geminiService  = geminiService;
            _context        = context;
            _budgetService  = budgetService;
            _weatherService = weatherService;
        }

        [BindProperty]
        public UserPreferences Preferences { get; set; } = new();

        // This holds the result from the AI
        public TripPlan GeneratedPlan { get; set; }

        public void OnGet()
        {
            // Initial load
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // 1. Validate Form Data
            if (!ModelState.IsValid)
            {
                Console.WriteLine("--- MODEL VALIDATION FAILED ---");
                foreach (var modelState in ModelState)
                {
                    foreach (var error in modelState.Value.Errors)
                    {
                        Console.WriteLine($"Property: {modelState.Key}, Error: {error.ErrorMessage}");
                    }
                }
                return Page();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
            Console.WriteLine($"\n🚨 FRONTEND SENDING USER ID: {currentUserId ?? "NULL!"} 🚨\n");

            if (DateTime.TryParse(Preferences.StartDate, out DateTime start) && DateTime.TryParse(Preferences.EndDate, out DateTime end))
            {
                Preferences.Days = Math.Max(1, (int)(end - start).TotalDays + 1);
            }

            try
            {
                // Save user response to database before calling API
                try 
                {
                    var userResponse = new UserResponse
                    {
                        Email = User.FindFirstValue(ClaimTypes.Email) ?? User.Identity?.Name,
                        Destinations = Preferences.Destinations != null ? string.Join(", ", Preferences.Destinations) : "",
                        Days = Preferences.Days,
                        Budget = Preferences.Budget,
                        PreferencesJson = JsonSerializer.Serialize(Preferences)
                    };

                    if (int.TryParse(currentUserId, out int parsedId))
                    {
                        userResponse.UserId = parsedId;
                    }

                    _context.UserResponses.Add(userResponse);
                    await _context.SaveChangesAsync();
                }
                catch (Exception dbEx)
                {
                    Console.WriteLine("Error saving response to DB: " + dbEx.Message);
                }

                // 2. Build the prompt for debugging
                string promptToAIs = _geminiService.BuildPrompt(Preferences);
                Console.WriteLine("\n================== DEBUG: AI PROMPT ==================");
                Console.WriteLine(promptToAIs);
                Console.WriteLine("======================================================\n");

                // 3. CALL THE API: This is the missing piece
                // We pass the Preferences object to your service to get the JSON result
                GeneratedPlan = await _geminiService.GenerateTripPlanAsync(Preferences);

                if (GeneratedPlan == null)
                {
                    ModelState.AddModelError(string.Empty, "The AI failed to generate a plan. Please try again.");
                    return Page();
                }

                Console.WriteLine($"Plan Successfully Generated: {GeneratedPlan.TripTitle}");

                // ── GeminiService already populated GeneratedPlan.Activities from JSON ──
                // Nothing to re-parse. Just set a friendly title if the AI didn't supply one.
                if (string.IsNullOrEmpty(GeneratedPlan.TripTitle))
                {
                    var destinations = Preferences?.Destinations != null
                        ? string.Join(", ", Preferences.Destinations)
                        : "Egypt";
                    GeneratedPlan.TripTitle = $"{destinations} Trip";
                }

                Console.WriteLine($"✅ Activities ready: {GeneratedPlan.Activities.Count} | Hotel: {GeneratedPlan.HotelName ?? "(none)"}");

                // ── Populate weather context so _TripResult.cshtml can request forecasts ──
                GeneratedPlan.StartDate          = Preferences.StartDate ?? "";
                GeneratedPlan.PrimaryDestination = Preferences.Destinations?.FirstOrDefault() ?? "Cairo & Giza";
                GeneratedPlan.TripDays           = Preferences.Days;

                if (User.Identity?.IsAuthenticated == true)
                {
                    if (int.TryParse(currentUserId, out int userId))
                    {
                        Console.WriteLine($"\n🚨 BACKEND SAVING FOR USER ID: {userId} 🚨\n");

                        // Parse hotel price from hotelDetails e.g. "1329.56 EGP/night, Luxury hotel..."
                        decimal hotelPricePerNight = 0;
                        if (!string.IsNullOrEmpty(GeneratedPlan.HotelDetails))
                        {
                            var priceMatch = System.Text.RegularExpressions.Regex.Match(
                                GeneratedPlan.HotelDetails, @"[\d,]+\.?\d*");
                            if (priceMatch.Success)
                                decimal.TryParse(priceMatch.Value.Replace(",", ""),
                                    System.Globalization.NumberStyles.Any,
                                    System.Globalization.CultureInfo.InvariantCulture,
                                    out hotelPricePerNight);
                        }

                        int nights = Math.Max(Preferences?.Days ?? 1, 1);

                        DateTime tripEndDate = DateTime.UtcNow.AddDays(nights);
                        if (!string.IsNullOrEmpty(Preferences?.EndDate) && DateTime.TryParse(Preferences.EndDate, out DateTime parsedEndDate))
                        {
                            tripEndDate = parsedEndDate;
                        }

                        var savedPlan = new SavedPlan
                        {
                            UserId = userId,
                            Title = GeneratedPlan.TripTitle ?? "My Custom Trip",
                            PlanData = GeneratedPlan.Content,
                            CreatedAt = DateTime.UtcNow,
                            HotelName = GeneratedPlan.HotelName,
                            HotelDetails = GeneratedPlan.HotelDetails,
                            HotelPricePerNight = hotelPricePerNight,
                            Nights = nights,
                            TripEndDate = tripEndDate,
                            IsPaid = false
                        };
                        _context.SavedPlans.Add(savedPlan);
                        await _context.SaveChangesAsync();
                        
                        foreach (var activity in GeneratedPlan.Activities)
                        {
                            activity.SavedPlanId = savedPlan.Id;
                            _context.SavedTripActivities.Add(activity);
                        }
                        await _context.SaveChangesAsync();
                        
                        // Pass back the SavedPlanId so the UI knows it
                        GeneratedPlan.SavedPlanId = savedPlan.Id;
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during generation: " + ex.Message);
                ModelState.AddModelError(string.Empty, "An error occurred while connecting to the AI service.");
            }

            // 4. Return the same page. 
            // Because GeneratedPlan is now NOT null, the HTML will switch to the results view.
            return Page();
        }

        public async Task<IActionResult> OnPostSaveTripEditAsync([FromBody] List<TripUpdateDto> updates)
        {
            if (updates == null || !updates.Any())
            {
                return new JsonResult(new { success = false, message = "No updates provided" });
            }

            try
            {
                foreach (var update in updates)
                {
                    var activity = await _context.SavedTripActivities.FindAsync(update.ActivityId);
                    if (activity != null)
                    {
                        activity.DayNumber = update.NewDay;
                        activity.SequenceOrder = update.NewOrder;
                    }
                }
                await _context.SaveChangesAsync();
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        private static decimal RoundToNearest(decimal value, decimal nearest) =>
            Math.Round(value / nearest, 0) * nearest;

        public IActionResult OnGetCalculateBudgetRange([FromQuery] string destinations, [FromQuery] int days, [FromQuery] int adults, [FromQuery] int kids, [FromQuery] string startDate)
        {
            try
            {
                if (string.IsNullOrEmpty(destinations) || days < 1 || adults < 1)
                {
                    return new JsonResult(new { success = false, message = "Invalid parameters" });
                }

                var destList = destinations.Split(',').Select(d => d.Trim()).Where(d => !string.IsNullOrEmpty(d)).ToList();
                if (destList.Count == 0)
                {
                    return new JsonResult(new { success = false, message = "At least one destination is required" });
                }

                bool isHighSeason = false;
                if (DateTime.TryParse(startDate, out DateTime start))
                {
                    isHighSeason = BudgetCalculatorService.IsHighSeason(start);
                }

                var estimate = _budgetService.Calculate(destList, days, adults, kids, isHighSeason);

                // Build 4 logical tiers from the three real data points:
                //   Budget     → min to halfway between min and mid
                //   Mid-Range  → halfway between min and mid, to mid
                //   Comfortable→ mid to halfway between mid and max
                //   Luxury     → halfway between mid and max, to max (open-ended)
                decimal midLow  = RoundToNearest(estimate.MinTotalEGP + (estimate.MidTotalEGP - estimate.MinTotalEGP) / 2, 1000);
                decimal midHigh = RoundToNearest(estimate.MidTotalEGP + (estimate.MaxTotalEGP - estimate.MidTotalEGP) / 2, 1000);

                var budgetRanges = new List<object>
                {
                    new { min = estimate.MinTotalEGP, max = midLow,              label = $"EGP {(int)estimate.MinTotalEGP} – EGP {(int)midLow}",              tier = "Budget" },
                    new { min = midLow,               max = estimate.MidTotalEGP, label = $"EGP {(int)midLow} – EGP {(int)estimate.MidTotalEGP}",              tier = "Mid-Range" },
                    new { min = estimate.MidTotalEGP, max = midHigh,             label = $"EGP {(int)estimate.MidTotalEGP} – EGP {(int)midHigh}",             tier = "Comfortable" },
                    new { min = midHigh,              max = estimate.MaxTotalEGP, label = $"EGP {(int)midHigh} – EGP {(int)estimate.MaxTotalEGP}",             tier = "Luxury" },
                };

                return new JsonResult(new
                {
                    success = true,
                    budgetRanges = budgetRanges,
                    estimate = new
                    {
                        minTotal = estimate.MinTotalEGP,
                        maxTotal = estimate.MaxTotalEGP,
                        minAccommodation = estimate.MinAccommodationTotal,
                        maxAccommodation = estimate.MaxAccommodationTotal,
                        minFood = estimate.MinFoodActivitiesTotal,
                        maxFood = estimate.MaxFoodActivitiesTotal,
                        minTransport = estimate.MinTransportationTotal,
                        maxTransport = estimate.MaxTransportationTotal,
                        groupMultiplier = estimate.GroupEconomyMultiplier
                    }
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        // ── Weather forecast endpoint ─────────────────────────────────────────────
        // Called by JS: GET /Planner?handler=Weather&destination=Cairo+%26+Giza&startDate=2026-05-01&days=5
        public async Task<IActionResult> OnGetWeatherAsync(
            [FromQuery] string destination,
            [FromQuery] string startDate,
            [FromQuery] int    days)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(destination) || days < 1)
                    return new JsonResult(new List<object>());

                if (!DateTime.TryParse(startDate, out DateTime start))
                    start = DateTime.Today;

                var forecast = await _weatherService.GetForecastAsync(destination, start, days);
                return new JsonResult(forecast);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Weather Handler] Error: {ex.Message}");
                return new JsonResult(new List<object>());
            }
        }
    }
}