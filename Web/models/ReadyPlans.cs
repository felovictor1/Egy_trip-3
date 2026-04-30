using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eg_travil.models
{
    [Table("ReadyPlans")] // This ensures EF looks for the correct table name
    public class ReadyPlans
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Rating { get; set; }
        public string Reviews { get; set; }
        public string Description { get; set; }
        public string Inclusions { get; set; }
        public string Exclusions { get; set; }
        public string Itinerary { get; set; }
        public string? PhotoPath { get; set; }
    }
}