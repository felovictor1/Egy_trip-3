using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace eg_travil.services
{
    // ── DTOs returned to the frontend ────────────────────────────────────────────
    public class DailyWeatherDto
    {
        public int    DayNumber  { get; set; }   // 1-based trip day (Day 1, Day 2 ...)
        public string Date       { get; set; } = "";
        public double MaxTempC   { get; set; }
        public double MinTempC   { get; set; }
        public double AvgTempC   { get; set; }
        public string Condition  { get; set; } = "";
        public string IconUrl    { get; set; } = "";
        public int    RainChance { get; set; }
    }

    // ── Internal WeatherAPI.com response model ────────────────────────────────────
    internal class WeatherApiResponse
    {
        public WeatherForecast? forecast { get; set; }
    }
    internal class WeatherForecast
    {
        public List<ForecastDay>? forecastday { get; set; }
    }
    internal class ForecastDay
    {
        public DateTime date { get; set; }
        public ForecastDayInfo day { get; set; } = new();
    }
    internal class ForecastDayInfo
    {
        public double maxtemp_c          { get; set; }
        public double mintemp_c          { get; set; }
        public double avgtemp_c          { get; set; }
        public int    daily_chance_of_rain { get; set; }
        public WeatherCondition condition { get; set; } = new();
    }
    internal class WeatherCondition
    {
        public string text { get; set; } = "";
        public string icon { get; set; } = "";    // "//cdn.weatherapi.com/..."
    }

    // ── Service ───────────────────────────────────────────────────────────────────
    public class WeatherService
    {
        private readonly HttpClient _http;
        private readonly string     _apiKey;

        // City name aliases: our destination labels → WeatherAPI query strings
        private static readonly Dictionary<string, string> _cityMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Cairo & Giza"]   = "Cairo",
            ["Alexandria"]     = "Alexandria",
            ["Luxor"]          = "Luxor",
            ["Aswan"]          = "Aswan",
            ["Sharm El Sheikh"]= "Sharm El Sheikh",
            ["Hurghada"]       = "Hurghada",
            ["Dahab"]          = "Dahab",
            ["Marsa Alam"]     = "Marsa Alam",
            ["Siwa"]           = "Siwa",
        };

        public WeatherService(HttpClient http, IConfiguration config)
        {
            _http   = http;
            _apiKey = config["WeatherApi:ApiKey"] ?? throw new InvalidOperationException("WeatherApi:ApiKey is not configured.");
        }

        /// <summary>
        /// Returns up to <paramref name="days"/> days of forecast for <paramref name="destination"/>.
        /// Capped at 14 days (WeatherAPI free-plan limit).
        /// </summary>
        public async Task<List<DailyWeatherDto>> GetForecastAsync(string destination, DateTime startDate, int days)
        {
            // How many days from today until the trip starts?
            int daysUntilStart = Math.Max(0, (startDate.Date - DateTime.Today).Days);

            // Request enough days to cover: gap to trip start + full trip duration
            int apiDays = Math.Clamp(daysUntilStart + days, 1, 14);

            // Resolve city alias
            string city = _cityMap.TryGetValue(destination, out var mapped) ? mapped : destination;

            var url = $"https://api.weatherapi.com/v1/forecast.json" +
                      $"?key={_apiKey}" +
                      $"&q={Uri.EscapeDataString(city)}" +
                      $"&days={apiDays}" +
                      $"&aqi=no&alerts=no";

            try
            {
                var response = await _http.GetFromJsonAsync<WeatherApiResponse>(url);
                if (response?.forecast?.forecastday == null)
                    return new();

                // Keep only days that fall within the trip window
                var availableDays = response.forecast.forecastday
                    .Where(fd => fd.date.Date >= startDate.Date)
                    .ToList();

                if (!availableDays.Any() && response.forecast.forecastday.Any())
                {
                    // Fallback to the last available day from the response if the trip is too far in the future
                    availableDays.Add(response.forecast.forecastday.Last());
                }

                var results = new List<DailyWeatherDto>();

                for (int i = 0; i < days; i++)
                {
                    var targetDate = startDate.Date.AddDays(i);
                    var fd = availableDays.FirstOrDefault(d => d.date.Date == targetDate);
                    
                    // If the API didn't return data for this day (e.g. free tier limit), use the last available day's data
                    if (fd == null && availableDays.Any())
                    {
                        fd = availableDays.Last();
                    }

                    if (fd != null)
                    {
                        results.Add(new DailyWeatherDto
                        {
                            DayNumber  = i + 1,
                            Date       = targetDate.ToString("yyyy-MM-dd"),
                            MaxTempC   = Math.Round(fd.day.maxtemp_c, 1),
                            MinTempC   = Math.Round(fd.day.mintemp_c, 1),
                            AvgTempC   = Math.Round(fd.day.avgtemp_c, 1),
                            Condition  = fd.day.condition.text,
                            IconUrl    = fd.day.condition.icon.StartsWith("//")
                                            ? "https:" + fd.day.condition.icon
                                            : fd.day.condition.icon,
                            RainChance = fd.day.daily_chance_of_rain,
                        });
                    }
                }

                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WeatherService] Error: {ex.Message}");
                return new();
            }
        }
    }
}
