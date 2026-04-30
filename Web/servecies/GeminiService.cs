//using System.Net.Http.Json;
//using System.Text.Json;
//using eg_travil.models;

//namespace eg_travil.servecies
//{
//    public class GeminiService
//    {
//        private readonly HttpClient _httpClient;
//        private readonly string _apiKey;

//        public GeminiService(HttpClient httpClient, IConfiguration config)
//        {
//            _httpClient = httpClient;
//            _apiKey = config["Gemini:ApiKey"];
//        }

//        public async Task<TripPlan> GenerateTripPlanAsync(UserPreferences prefs)
//        {
//            if (string.IsNullOrEmpty(_apiKey))
//            {
//                throw new Exception("Gemini API Key is missing in appsettings.json");
//            }

//            // We use the same prompt logic from your React app for consistency
//            var prompt = $@"
//                Create a detailed travel itinerary for Egypt. 
//                Destinations: {string.Join(", ", prefs.Destinations)}
//                Dates: {prefs.StartDate:yyyy-MM-dd} to {prefs.EndDate:yyyy-MM-dd}
//                Budget: {prefs.Budget}, Pace: {prefs.Intensity}.

//                Output Format: JSON ONLY. Use this schema: 
//                {{ ""tripTitle"": """", ""summary"": """", ""days"": [], ""todoList"": [] }}";

//            var requestBody = new
//            {
//                contents = new[] { new { parts = new[] { new { text = prompt } } } },
//                generationConfig = new
//                {
//                    response_mime_type = "application/json"
//                }
//            };

//            var response = await _httpClient.PostAsJsonAsync(
//                $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={_apiKey}",
//                requestBody);

//            if (!response.IsSuccessStatusCode)
//            {
//                var error = await response.Content.ReadAsStringAsync();
//                throw new Exception($"Gemini API Error: {response.StatusCode} - {error}");
//            }

//            // 1. Get the raw Google response
//            var googleRawResponse = await response.Content.ReadFromJsonAsync<JsonElement>();

//            // 2. Extract the actual JSON string from Google's wrapper: 
//            // candidates[0].content.parts[0].text
//            var cleanJson = googleRawResponse
//                .GetProperty("candidates")[0]
//                .GetProperty("content")
//                .GetProperty("parts")[0]
//                .GetProperty("text")
//                .GetString();

//            // 3. Deserialize into your TripPlan model
//            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
//            return JsonSerializer.Deserialize<TripPlan>(cleanJson, options);
//        }
//    }
//}

using System.Net.Http.Json;
using System.Text.Json;
using eg_travil.models;

namespace eg_travil.servecies
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["Gemini:ApiKey"];
        }

        // ADD THIS METHOD HERE
        private (decimal min, decimal max) ParseBudgetRange(string budgetString)
        {
            // Parse budget strings like "EGP 15000 – EGP 60000" or "EGP 15000+"
            if (string.IsNullOrEmpty(budgetString))
                return (0, 0);

            // Remove "EGP", strip thousand-separator commas, then split by "–" or "+"
            // Budget labels are formatted with commas (e.g. "EGP 15,000 – EGP 60,000"),
            // which decimal.TryParse cannot handle with the default (invariant) culture.
            var cleaned = budgetString.Replace("EGP", "").Replace(",", "").Trim();

            if (cleaned.Contains("–"))
            {
                var parts = cleaned.Split('–');
                if (parts.Length == 2)
                {
                    if (decimal.TryParse(parts[0].Trim(), System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture, out var min) &&
                        decimal.TryParse(parts[1].Trim(), System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture, out var max))
                    {
                        return (min, max);
                    }
                }
            }
            else if (cleaned.Contains("+"))
            {
                var parts = cleaned.Split('+');
                if (decimal.TryParse(parts[0].Trim(), System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var min))
                {
                    return (min, min * 2); // Assume max is 2x the minimum for open-ended budgets
                }
            }
            else if (decimal.TryParse(cleaned, System.Globalization.NumberStyles.Any,
                         System.Globalization.CultureInfo.InvariantCulture, out var single))
            {
                return (single, single * 1.5m);
            }

            return (0, 0);
        }

        public string BuildPrompt(UserPreferences prefs)
        {
            var isoStartDate = DateTime.TryParse(prefs.StartDate, out var sDate) ? sDate.ToString("yyyy-MM-ddTHH:mm:ssZ") : prefs.StartDate;
            var isoEndDate = DateTime.TryParse(prefs.EndDate, out var eDate) ? eDate.ToString("yyyy-MM-ddTHH:mm:ssZ") : prefs.EndDate;

            // Parse budget range
            var (budgetMin, budgetMax) = ParseBudgetRange(prefs.Budget);

            var payload = new
            {
                core_constraints = new
                {
                    destinations = prefs.Destinations ?? new List<string>(),
                    total_budget_egp = prefs.Budget,
                    budget_min_egp = budgetMin,
                    budget_max_egp = budgetMax,
                    group_size = (prefs.Travelers?.Adults ?? 0) + (prefs.Travelers?.Kids ?? 0),
                    travel_dates = new
                    {
                        start_date = isoStartDate,
                        end_date = isoEndDate
                    }
                },
                travel_styles = prefs.TravelStyles ?? new List<string>(),
                dynamic_preferences = new
                {
                    historical_knowledge = string.IsNullOrEmpty(prefs.HistoricalKnowledge) ? null : prefs.HistoricalKnowledge,
                    preferred_time_periods = (prefs.PreferredTourismStyle != null && prefs.PreferredTourismStyle.Any()) ? prefs.PreferredTourismStyle : null,
                    museum_visits = prefs.MuseumPreferences,
                    water_activities = prefs.WantsWaterActivities
                },
                logistics = new
                {
                    accommodation_type = prefs.Accommodation,
                    transportation_preference = prefs.TransportationPreference,
                    food_preferences = string.IsNullOrEmpty(prefs.FoodPreferences) ? null : prefs.FoodPreferences,
                    trip_pace = prefs.Intensity,
                    must_visit_places = string.IsNullOrEmpty(prefs.MustVisit) ? null : prefs.MustVisit
                }
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Serialize(payload, options);
        }

        public async Task<TripPlan> GenerateTripPlanAsync(UserPreferences request)
        {
            var mlServiceUrl = "http://127.0.0.1:8585";

            // Parse the budget range
            var (budgetMin, budgetMax) = ParseBudgetRange(request.Budget);

            var pythonPayload = new
            {
                destinations = request.Destinations,
                budgetMin = budgetMin,
                budgetMax = budgetMax,
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

            var content = new StringContent(JsonSerializer.Serialize(pythonPayload), System.Text.Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync($"{mlServiceUrl.TrimEnd('/')}/api/trip", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    throw new Exception($"ML Service Error: {errorResponse}");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();

                // Deserialize the clean JSON from the Python service
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var plan = JsonSerializer.Deserialize<TripPlan>(jsonResponse, options) ?? new TripPlan();

                // Always store the raw JSON so the UI can render it if needed
                plan.Content = jsonResponse;

                // Ensure Activities list is initialised
                plan.Activities ??= new List<SavedTripActivity>();

                // Auto-derive TimeLabel from sequenceOrder (so the UI badge colours work)
                foreach (var act in plan.Activities)
                {
                    if (string.IsNullOrEmpty(act.TimeLabel))
                    {
                        act.TimeLabel = act.SequenceOrder switch
                        {
                            1 => "morning",
                            2 => "lunch",
                            3 => "afternoon",
                            4 => "dinner",
                            _ => ""
                        };
                    }
                }

                Console.WriteLine($"✅ [GeminiService] Deserialized {plan.Activities.Count} activities. Hotel: {plan.HotelName ?? "(none)"}");
                return plan;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to ML Service: {ex.Message}");
                throw;
            }
        }

        public async Task<string> ChatWithGeminiAsync(ChatRequest request)
        {
            // Try to get OpenRouter API Key from appsettings first, then fallback to ML_Service_Config/.env
            string apiKey = "";
            string modelId = "google/gemini-1.5-flash"; // fallback if not found in .env
            try {
                string envPath = Path.Combine(Directory.GetCurrentDirectory(), "ML_Service_Config", ".env");
                if (File.Exists(envPath))
                {
                    var lines = File.ReadAllLines(envPath);
                    foreach(var line in lines)
                    {
                        if (line.StartsWith("OPENROUTER_API_KEY=") && !line.StartsWith("#"))
                        {
                            apiKey = line.Substring("OPENROUTER_API_KEY=".Length).Trim();
                        }
                        else if (line.StartsWith("OPENROUTER_MODEL=") && !line.StartsWith("#"))
                        {
                            modelId = line.Substring("OPENROUTER_MODEL=".Length).Trim();
                        }
                    }
                }
            } catch { }

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("OpenRouter API Key is missing. Please add it to ML_Service_Config/.env as OPENROUTER_API_KEY.");
            }

            var messages = new List<object>
            {
                new { role = "system", content = "You are an expert travel assistant specializing in Egypt. Provide concise, helpful, friendly, and practical advice about traveling to Egypt. Use emojis occasionally." }
            };

            foreach (var msg in request.Messages)
            {
                // Map frontend roles ('user'/'model') to OpenAI roles ('user'/'assistant')
                string mappedRole = msg.Role == "model" ? "assistant" : "user";
                messages.Add(new
                {
                    role = mappedRole,
                    content = msg.Text
                });
            }

            var requestBody = new
            {
                model = modelId,
                messages = messages,
                temperature = 0.7
            };

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://openrouter.ai/api/v1/chat/completions");
            requestMessage.Headers.Add("Authorization", $"Bearer {apiKey}");
            requestMessage.Content = JsonContent.Create(requestBody);

            var response = await _httpClient.SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"OpenRouter API Error: {response.StatusCode} - {error}");
            }

            var rawResponse = await response.Content.ReadFromJsonAsync<JsonElement>();

            try
            {
                var responseText = rawResponse
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();
                return responseText ?? "No response generated.";
            }
            catch
            {
                return "Failed to parse OpenRouter response.";
            }
        }
    }
}