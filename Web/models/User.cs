namespace eg_travil.models
{
    public class User
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int? Age { get; set; }
        public string? PhoneNumber { get; set; }
        public ICollection<SavedPlan> SavedPlans { get; set; } = new List<SavedPlan>();
        public bool IsBanned { get; set; } = false;
    }
}
