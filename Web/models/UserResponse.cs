namespace eg_travil.models
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string? Email { get; set; } // Optional: identify user if they provided it easily
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Key metrics to extract for analysis
        public string? Destinations { get; set; } // Comma separated list
        public int Days { get; set; }
        public string? Budget { get; set; }

        // The entire JSON of UserPreferences we can deserialize for detailed usage
        public string PreferencesJson { get; set; } = string.Empty;

        public int? UserId { get; set; }
        public User? User { get; set; }
    }
}
