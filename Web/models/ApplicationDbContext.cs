using Microsoft.EntityFrameworkCore;

namespace eg_travil.models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<ReadyPlans> ReadyPlans { get; set; }

        public DbSet<UserResponse> UserResponses { get; set; }

        public DbSet<SavedPlan> SavedPlans { get; set; }

        public DbSet<SavedTripActivity> SavedTripActivities { get; set; }
    }
}