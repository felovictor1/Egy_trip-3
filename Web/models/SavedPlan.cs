using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eg_travil.models
{
    public class SavedPlan
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; }

        public string PlanData { get; set; } // Storing the JSON or markdown plan response

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // --- Payment fields ---
        public bool IsPaid { get; set; } = false;
        public string? HotelName { get; set; }
        public string? HotelDetails { get; set; }
        public decimal HotelPricePerNight { get; set; } = 0;
        public int Nights { get; set; } = 1;

        /// <summary>Date the trip ends (CreatedAt + Nights). Review button unlocks after this.</summary>
        public DateTime? TripEndDate { get; set; }

        // --- Review fields ---
        public int? ReviewRating { get; set; }        // 1-5 stars
        public string? ReviewText { get; set; }
        public DateTime? ReviewedAt { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public ICollection<SavedTripActivity> Activities { get; set; } = new List<SavedTripActivity>();
    }
}
