namespace eg_travil.models
{
    /// <summary>
    /// Represents the real-world cost baseline for a single Egyptian governorate.
    /// All monetary values are in Egyptian Pounds (EGP) per person per night/day,
    /// unless noted otherwise.
    /// </summary>
    public class GovernorateCostProfile
    {
        /// <summary>Canonical name matching the frontend destination list.</summary>
        public string Name { get; init; } = string.Empty;

        // ── Accommodation ────────────────────────────────────────────────────────
        /// <summary>Nightly rate for a budget hostel / cheap hotel (EGP / person).</summary>
        public decimal AccommodationMinPerNight { get; init; }

        /// <summary>Nightly rate for a mid-range hotel (EGP / person).</summary>
        public decimal AccommodationMidPerNight { get; init; }

        /// <summary>Nightly rate for a luxury / resort stay (EGP / person).</summary>
        public decimal AccommodationMaxPerNight { get; init; }

        // ── Food & Activities ────────────────────────────────────────────────────
        /// <summary>
        /// Daily spend on food + entrance fees + local activities for one person
        /// at the budget tier (street food, free sights, etc.).
        /// </summary>
        public decimal DailyActivitiesMinPerPerson { get; init; }

        /// <summary>Daily spend at mid-range tier (sit-down meals, paid attractions).</summary>
        public decimal DailyActivitiesMidPerPerson { get; init; }

        /// <summary>Daily spend at premium tier (fine dining, guided tours, etc.).</summary>
        public decimal DailyActivitiesMaxPerPerson { get; init; }

        // ── Intra-city Transportation ────────────────────────────────────────────
        /// <summary>
        /// Estimated daily intra-city transport per person (microbus / metro).
        /// This covers getting between attractions inside the governorate.
        /// </summary>
        public decimal LocalTransportMinPerDay { get; init; }

        /// <summary>Daily intra-city transport at mid tier (Uber / taxi).</summary>
        public decimal LocalTransportMidPerDay { get; init; }

        /// <summary>Daily intra-city transport at premium tier (private car hire).</summary>
        public decimal LocalTransportMaxPerDay { get; init; }

        // ── Inter-destination Travel ─────────────────────────────────────────────
        /// <summary>
        /// One-way travel cost TO this governorate from a neutral hub (Cairo).
        /// Used once when the user includes this destination in a multi-city trip.
        /// Budget = bus/minibus; Max = domestic flight.
        /// </summary>
        public decimal InterCityTravelMin { get; init; }
        public decimal InterCityTravelMax { get; init; }

        // ── Seasonal multiplier hint ─────────────────────────────────────────────
        /// <summary>
        /// Peak-season uplift factor (e.g., 1.30 = 30 % more expensive in high season).
        /// Applied by the algorithm only when IsHighSeason == true.
        /// </summary>
        public double PeakSeasonMultiplier { get; init; } = 1.0;
    }

    /// <summary>
    /// The output produced by <see cref="eg_travil.services.BudgetCalculatorService"/>.
    /// Surfaces a minimum (budget traveler) and maximum (premium traveler) total cost
    /// for the entire trip, plus a breakdown by spend category.
    /// All values are in EGP.
    /// </summary>
    public class TripBudgetEstimate
    {
        // ── Totals ───────────────────────────────────────────────────────────────
        public decimal MinTotalEGP { get; set; }   // Budget tier
        public decimal MidTotalEGP { get; set; }   // Mid-range tier
        public decimal MaxTotalEGP { get; set; }   // Luxury tier

        // ── Category breakdowns (useful for displaying a pie/bar chart) ──────────
        public decimal MinAccommodationTotal { get; set; }
        public decimal MidAccommodationTotal { get; set; }
        public decimal MaxAccommodationTotal { get; set; }

        public decimal MinFoodActivitiesTotal { get; set; }
        public decimal MidFoodActivitiesTotal { get; set; }
        public decimal MaxFoodActivitiesTotal { get; set; }

        public decimal MinTransportationTotal { get; set; }
        public decimal MidTransportationTotal { get; set; }
        public decimal MaxTransportationTotal { get; set; }

        // ── Meta ─────────────────────────────────────────────────────────────────
        public int TotalPeople { get; set; }
        public int TotalDays { get; set; }
        public List<string> Destinations { get; set; } = new();

        /// <summary>
        /// The economy-of-scale multiplier that was applied to the group total.
        /// Values < 1 represent a discount (larger groups share fixed costs).
        /// </summary>
        public double GroupEconomyMultiplier { get; set; }

        /// <summary>Formatted string for UI display, e.g. "EGP 4,500 – EGP 12,000".</summary>
        public string FormattedRange =>
            $"EGP {MinTotalEGP:N0} – EGP {MaxTotalEGP:N0}";
    }
}
