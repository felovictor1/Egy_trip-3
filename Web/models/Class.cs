using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace eg_travil.models
{


    // Holds the user's choices from the 8-step form
    public class UserPreferences
    {
        public List<string> Destinations { get; set; } = new();
        public string StartDate { get; set; } = "";
        public string EndDate { get; set; } = "";
        public int Days { get; set; } = 3;
        public TravelerCount Travelers { get; set; } = new();
        public string? Budget { get; set; }
        public List<string> TravelStyles { get; set; } = new();
        public bool? WantsWaterActivities { get; set; }
        public string? HistoricalKnowledge { get; set; }
        public List<string> PreferredTourismStyle { get; set; } = new();
        public bool? MuseumPreferences { get; set; }
        public string? TransportationPreference { get; set; }

        public string? Accommodation { get; set; }
        public List<string> Facilities { get; set; } = new();
        public string? Intensity { get; set; }
        public string? FoodPreferences { get; set; }
        public string? MustVisit { get; set; }
    }

    public class TravelerCount
    {
        public int Adults { get; set; } = 2;
        public int Kids { get; set; } = 0;
    }

    public class Travelers
        {
            public int Adults { get; set; } = 2;
            public int Kids { get; set; } = 0;
        }

        // This is what we show on the landing page (from your constants.ts)
        public class ReadyMadePlan
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Location { get; set; }
            public string Duration { get; set; }
            public string ImageUrl { get; set; }
            public string Description { get; set; }
            public double Rating { get; set; }
            public int Reviews { get; set; }
            public List<string> Highlights { get; set; } = new();
        }
    // The main result returned by the AI
    public class TripPlan
    {
        public int? SavedPlanId { get; set; }
        public bool IsPaid { get; set; } = false;
        public string TripTitle { get; set; }
        public string Summary { get; set; }
        public string TotalEstimatedCost { get; set; }
        public List<DayPlan> Days { get; set; } = new();
        public List<string> TodoList { get; set; } = new();
        public string Content { get; set; }

        // Used by the weather widget in _TripResult.cshtml
        public string StartDate { get; set; } = "";          // "2026-05-01"
        public string PrimaryDestination { get; set; } = ""; // first destination, e.g. "Cairo & Giza"
        public int    TripDays { get; set; } = 3;             // total number of days

        [JsonPropertyName("activities")]
        public List<SavedTripActivity> Activities { get; set; } = new();

        [JsonPropertyName("hotelName")]
        public string HotelName { get; set; }

        [JsonPropertyName("hotelDetails")]
        public string HotelDetails { get; set; }
    }

    public class SavedTripActivity
    {
        public int Id { get; set; }
        public int SavedPlanId { get; set; }
        public SavedPlan SavedPlan { get; set; }

        [JsonPropertyName("dayNumber")]
        public int DayNumber { get; set; }

        [JsonPropertyName("sequenceOrder")]
        public int SequenceOrder { get; set; } // 1: morning, 2: lunch, 3: afternoon, 4: dinner

        [JsonPropertyName("timeLabel")]
        public string TimeLabel { get; set; } // "morning", "lunch", etc.

        [JsonPropertyName("place")]
        public string Place { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("ticketPrice")]
        public string TicketPrice { get; set; }

        [Column("Timings")]
        [JsonPropertyName("scheduledTime")]
        public string ScheduledTime { get; set; }

        [JsonPropertyName("transport")]
        public string? Transport { get; set; }

        [NotMapped]
        public string OperatingHours { get; set; }

        [NotMapped]
        [JsonPropertyName("imageUrl")]
        public string ImageUrl { get; set; }
    }

    public class TripUpdateDto
    {
        public int ActivityId { get; set; }
        public int NewDay { get; set; }
        public int NewOrder { get; set; }
    }

    public class DayPlan
    {
        public int DayNumber { get; set; }
        public string Date { get; set; }
        public List<ActivityItem> Schedule { get; set; } = new();
        public FoodOptions Food { get; set; } = new();
        public string Tips { get; set; }
    }

    public class ActivityItem
    {
        public string Time { get; set; }
        public string Activity { get; set; }
        public string Description { get; set; }
        public string Cost { get; set; }
        public string Transport { get; set; }
        public string ImageKeyword { get; set; }
    }

    public class FoodOptions
    {
        public string Lunch { get; set; }
        public string Dinner { get; set; }
    }

    public class ChatMessage
    {
        public string Role { get; set; } = "user";
        public string Text { get; set; } = "";
    }

    public class ChatRequest
    {
        public List<ChatMessage> Messages { get; set; } = new();
    }
}
