using eg_travil.models;

namespace eg_travil.services
{
    /// <summary>
    /// Calculates a minimum-to-maximum budget range for an Egypt trip.
    ///
    /// ══════════════════════════════════════════════════════════════
    ///  BUDGET DISTRIBUTION MODEL
    /// ══════════════════════════════════════════════════════════════
    ///
    ///  The total trip cost is decomposed into three buckets:
    ///
    ///  1. ACCOMMODATION  (per person × nights)
    ///     Each governorate stores three nightly rates: budget / mid / luxury.
    ///     Min estimate uses the budget rate; Max uses the luxury rate.
    ///     Days - 1 is used for nights when visiting multiple destinations
    ///     (last night is usually travel home).
    ///
    ///  2. FOOD & DAILY ACTIVITIES  (per person × days)
    ///     Covers meals, entrance fees, guided tours, leisure activities,
    ///     and incidental spending. Each governorate stores three daily rates
    ///     that reflect its local cost-of-living and tourism intensity.
    ///
    ///  3. TRANSPORTATION  (intra-city daily + one-way inter-city fare)
    ///     - Intra-city: per person × days, at budget / premium tiers.
    ///     - Inter-city: one-way fare from Cairo hub to each destination;
    ///       for a multi-city trip this is summed across all destinations
    ///       (each leg counted once, simulating a round-trip by doubling).
    ///
    ///  GROUP ECONOMY-OF-SCALE MULTIPLIER
    ///     Larger groups share fixed costs (one Uber instead of four,
    ///     room-sharing, group discounts at attractions).
    ///     The multiplier is applied to the raw group total AFTER summing
    ///     per-person costs. Scale: solo = 1.0, couples ≈ 0.95, family ≈ 0.85.
    ///
    ///  SEASONAL UPLIFT
    ///     If the caller signals high season (July–Aug or Dec–Jan for Red Sea),
    ///     a per-governorate PeakSeasonMultiplier is applied to accommodation.
    ///
    /// </summary>
    public class BudgetCalculatorService
    {
        // ════════════════════════════════════════════════════════════════════════
        //  GOVERNORATE COST DATABASE
        //  All values are in Egyptian Pounds (EGP).
        //  Sources: approximate 2024-2025 market research for Egypt tourism.
        // ════════════════════════════════════════════════════════════════════════
        private static readonly Dictionary<string, GovernorateCostProfile> _profiles =
            new(StringComparer.OrdinalIgnoreCase)
        {
            // ── Cairo & Giza ─────────────────────────────────────────────────────
            ["Cairo"] = new()
            {
                Name                       = "Cairo",
                // Budget hostel dorm to 5-star hotel
                AccommodationMinPerNight   = 400,
                AccommodationMidPerNight   = 1_200,
                AccommodationMaxPerNight   = 4_000,
                // Street food / koshary to high-end restaurant + museum entries
                DailyActivitiesMinPerPerson = 300,
                DailyActivitiesMidPerPerson = 800,
                DailyActivitiesMaxPerPerson = 2_000,
                // Metro + microbus vs. Uber/private car
                LocalTransportMinPerDay    = 50,
                LocalTransportMidPerDay    = 200,
                LocalTransportMaxPerDay    = 600,
                // Cairo is the hub — no inter-city travel cost
                InterCityTravelMin         = 0,
                InterCityTravelMax         = 0,
                PeakSeasonMultiplier       = 1.15,
            },

            ["Giza"] = new()
            {
                Name                       = "Giza",
                AccommodationMinPerNight   = 450,
                AccommodationMidPerNight   = 1_400,
                AccommodationMaxPerNight   = 5_000,
                DailyActivitiesMinPerPerson = 400,   // Pyramids ticket alone ≈ EGP 360
                DailyActivitiesMidPerPerson = 1_000,
                DailyActivitiesMaxPerPerson = 2_500,
                LocalTransportMinPerDay    = 60,
                LocalTransportMidPerDay    = 250,
                LocalTransportMaxPerDay    = 700,
                InterCityTravelMin         = 0,      // Adjacent to Cairo
                InterCityTravelMax         = 0,
                PeakSeasonMultiplier       = 1.20,
            },

            // ── Alexandria ───────────────────────────────────────────────────────
            ["Alexandria"] = new()
            {
                Name                       = "Alexandria",
                AccommodationMinPerNight   = 350,
                AccommodationMidPerNight   = 1_000,
                AccommodationMaxPerNight   = 3_200,
                DailyActivitiesMinPerPerson = 200,
                DailyActivitiesMidPerPerson = 600,
                DailyActivitiesMaxPerPerson = 1_500,
                LocalTransportMinPerDay    = 40,
                LocalTransportMidPerDay    = 150,
                LocalTransportMaxPerDay    = 400,
                // Train from Cairo ≈ EGP 200–700 one-way
                InterCityTravelMin         = 200,
                InterCityTravelMax         = 700,
                PeakSeasonMultiplier       = 1.30,   // Very busy in summer
            },

            // ── Luxor ────────────────────────────────────────────────────────────
            ["Luxor"] = new()
            {
                Name                       = "Luxor",
                AccommodationMinPerNight   = 350,
                AccommodationMidPerNight   = 1_100,
                AccommodationMaxPerNight   = 4_500,
                // High ticket prices (Valley of the Kings, Karnak, etc.)
                DailyActivitiesMinPerPerson = 500,
                DailyActivitiesMidPerPerson = 1_200,
                DailyActivitiesMaxPerPerson = 3_000,
                LocalTransportMinPerDay    = 80,    // Calèche / local taxi
                LocalTransportMidPerDay    = 300,
                LocalTransportMaxPerDay    = 800,
                // Train ≈ EGP 300 / flight ≈ EGP 1,500 one-way
                InterCityTravelMin         = 300,
                InterCityTravelMax         = 1_500,
                PeakSeasonMultiplier       = 1.25,
            },

            // ── Aswan ────────────────────────────────────────────────────────────
            ["Aswan"] = new()
            {
                Name                       = "Aswan",
                AccommodationMinPerNight   = 300,
                AccommodationMidPerNight   = 1_000,
                AccommodationMaxPerNight   = 4_000,
                // Abu Simbel excursion, Nubian village, Felucca
                DailyActivitiesMinPerPerson = 400,
                DailyActivitiesMidPerPerson = 1_000,
                DailyActivitiesMaxPerPerson = 2_800,
                LocalTransportMinPerDay    = 70,
                LocalTransportMidPerDay    = 250,
                LocalTransportMaxPerDay    = 700,
                InterCityTravelMin         = 400,
                InterCityTravelMax         = 1_800,
                PeakSeasonMultiplier       = 1.20,
            },

            // ── Hurghada ─────────────────────────────────────────────────────────
            ["Hurghada"] = new()
            {
                Name                       = "Hurghada",
                AccommodationMinPerNight   = 500,
                AccommodationMidPerNight   = 1_500,
                AccommodationMaxPerNight   = 6_000,
                // Snorkeling, diving, boat trips inflate daily costs
                DailyActivitiesMinPerPerson = 400,
                DailyActivitiesMidPerPerson = 1_200,
                DailyActivitiesMaxPerPerson = 3_500,
                LocalTransportMinPerDay    = 60,
                LocalTransportMidPerDay    = 200,
                LocalTransportMaxPerDay    = 500,
                // Bus ≈ EGP 350 / flight ≈ EGP 1,200 one-way
                InterCityTravelMin         = 350,
                InterCityTravelMax         = 1_200,
                // Red Sea resorts spike in winter (European holiday escape)
                PeakSeasonMultiplier       = 1.40,
            },

            // ── Sharm El-Sheikh ───────────────────────────────────────────────────
            ["Sharm El-Sheikh"] = new()
            {
                Name                       = "Sharm El-Sheikh",
                AccommodationMinPerNight   = 700,
                AccommodationMidPerNight   = 2_000,
                AccommodationMaxPerNight   = 8_000,
                DailyActivitiesMinPerPerson = 500,
                DailyActivitiesMidPerPerson = 1_500,
                DailyActivitiesMaxPerPerson = 4_000,
                LocalTransportMinPerDay    = 80,
                LocalTransportMidPerDay    = 300,
                LocalTransportMaxPerDay    = 700,
                // Ferry from Hurghada or flight from Cairo
                InterCityTravelMin         = 600,
                InterCityTravelMax         = 1_500,
                PeakSeasonMultiplier       = 1.45,
            },

            // ── Dahab ────────────────────────────────────────────────────────────
            ["Dahab"] = new()
            {
                Name                       = "Dahab",
                AccommodationMinPerNight   = 250,
                AccommodationMidPerNight   = 700,
                AccommodationMaxPerNight   = 2_500,
                DailyActivitiesMinPerPerson = 300,
                DailyActivitiesMidPerPerson = 800,
                DailyActivitiesMaxPerPerson = 2_000,
                LocalTransportMinPerDay    = 50,
                LocalTransportMidPerDay    = 150,
                LocalTransportMaxPerDay    = 400,
                InterCityTravelMin         = 700,
                InterCityTravelMax         = 1_700,
                PeakSeasonMultiplier       = 1.25,
            },

            // ── Siwa Oasis ───────────────────────────────────────────────────────
            ["Siwa"] = new()
            {
                Name                       = "Siwa",
                AccommodationMinPerNight   = 200,
                AccommodationMidPerNight   = 600,
                AccommodationMaxPerNight   = 1_800,
                DailyActivitiesMinPerPerson = 200,
                DailyActivitiesMidPerPerson = 600,
                DailyActivitiesMaxPerPerson = 1_500,
                LocalTransportMinPerDay    = 40,
                LocalTransportMidPerDay    = 150,
                LocalTransportMaxPerDay    = 400,
                // Long bus journey ≈ EGP 400 one-way
                InterCityTravelMin         = 400,
                InterCityTravelMax         = 900,
                PeakSeasonMultiplier       = 1.15,
            },

            // ── Marsa Matrouh ────────────────────────────────────────────────────
            ["Marsa Matrouh"] = new()
            {
                Name                       = "Marsa Matrouh",
                AccommodationMinPerNight   = 350,
                AccommodationMidPerNight   = 900,
                AccommodationMaxPerNight   = 3_000,
                DailyActivitiesMinPerPerson = 200,
                DailyActivitiesMidPerPerson = 600,
                DailyActivitiesMaxPerPerson = 1_500,
                LocalTransportMinPerDay    = 40,
                LocalTransportMidPerDay    = 150,
                LocalTransportMaxPerDay    = 400,
                InterCityTravelMin         = 400,
                InterCityTravelMax         = 1_200,
                // Extremely busy in Egyptian summer
                PeakSeasonMultiplier       = 1.50,
            },

            // ── Fayoum ───────────────────────────────────────────────────────────
            ["Fayoum"] = new()
            {
                Name                       = "Fayoum",
                AccommodationMinPerNight   = 250,
                AccommodationMidPerNight   = 700,
                AccommodationMaxPerNight   = 2_000,
                DailyActivitiesMinPerPerson = 150,
                DailyActivitiesMidPerPerson = 500,
                DailyActivitiesMaxPerPerson = 1_200,
                LocalTransportMinPerDay    = 40,
                LocalTransportMidPerDay    = 150,
                LocalTransportMaxPerDay    = 400,
                InterCityTravelMin         = 100,
                InterCityTravelMax         = 300,
                PeakSeasonMultiplier       = 1.10,
            },

            // ── Minya ─────────────────────────────────────────────────────────────
            ["Minya"] = new()
            {
                Name                       = "Minya",
                AccommodationMinPerNight   = 200,
                AccommodationMidPerNight   = 600,
                AccommodationMaxPerNight   = 1_500,
                DailyActivitiesMinPerPerson = 150,
                DailyActivitiesMidPerPerson = 450,
                DailyActivitiesMaxPerPerson = 1_000,
                LocalTransportMinPerDay    = 40,
                LocalTransportMidPerDay    = 150,
                LocalTransportMaxPerDay    = 350,
                InterCityTravelMin         = 150,
                InterCityTravelMax         = 600,
                PeakSeasonMultiplier       = 1.05,
            },

            // ── Sohag & Qena ─────────────────────────────────────────────────────
            ["Sohag"] = new()
            {
                Name                       = "Sohag",
                AccommodationMinPerNight   = 180,
                AccommodationMidPerNight   = 500,
                AccommodationMaxPerNight   = 1_200,
                DailyActivitiesMinPerPerson = 120,
                DailyActivitiesMidPerPerson = 400,
                DailyActivitiesMaxPerPerson = 900,
                LocalTransportMinPerDay    = 30,
                LocalTransportMidPerDay    = 120,
                LocalTransportMaxPerDay    = 300,
                InterCityTravelMin         = 200,
                InterCityTravelMax         = 800,
                PeakSeasonMultiplier       = 1.05,
            },

            // ── Ismailia ──────────────────────────────────────────────────────────
            ["Ismailia"] = new()
            {
                Name                       = "Ismailia",
                AccommodationMinPerNight   = 250,
                AccommodationMidPerNight   = 700,
                AccommodationMaxPerNight   = 1_800,
                DailyActivitiesMinPerPerson = 150,
                DailyActivitiesMidPerPerson = 450,
                DailyActivitiesMaxPerPerson = 1_000,
                LocalTransportMinPerDay    = 40,
                LocalTransportMidPerDay    = 150,
                LocalTransportMaxPerDay    = 350,
                InterCityTravelMin         = 150,
                InterCityTravelMax         = 500,
                PeakSeasonMultiplier       = 1.10,
            },

            // ── Port Said ─────────────────────────────────────────────────────────
            ["Port Said"] = new()
            {
                Name                       = "Port Said",
                AccommodationMinPerNight   = 300,
                AccommodationMidPerNight   = 800,
                AccommodationMaxPerNight   = 2_200,
                DailyActivitiesMinPerPerson = 180,
                DailyActivitiesMidPerPerson = 500,
                DailyActivitiesMaxPerPerson = 1_200,
                LocalTransportMinPerDay    = 40,
                LocalTransportMidPerDay    = 150,
                LocalTransportMaxPerDay    = 350,
                InterCityTravelMin         = 200,
                InterCityTravelMax         = 600,
                PeakSeasonMultiplier       = 1.15,
            },
        };

        // ════════════════════════════════════════════════════════════════════════
        //  GROUP ECONOMY-OF-SCALE TABLE
        //
        //  Logic: A large group shares rides, hotel rooms (twin/triple
        //  occupancy), and often qualifies for group discounts at attractions.
        //  The multiplier is applied to the RAW GROUP TOTAL (not per-person),
        //  so it shrinks proportionally as the group grows.
        //
        //  Example: 6 people with raw total EGP 12,000 → 12,000 × 0.83 = EGP 9,960
        // ════════════════════════════════════════════════════════════════════════
        private static double GetGroupEconomyMultiplier(int totalPeople) => totalPeople switch
        {
            1          => 1.00,   // Solo: full price, no sharing
            2          => 0.95,   // Couple: minor saving (shared room)
            3          => 0.90,   // Small group: split taxi, shared room
            4 or 5     => 0.87,   // Family / quad: room sharing + group tickets
            6 or 7     => 0.83,   // Medium group: meaningful volume discounts
            >= 8 and
            <= 12      => 0.80,   // Large group: significant shared costs
            _          => 0.75,   // Tour group (13+): maximum economy
        };

        private static decimal RoundToNearest1000(decimal value)
        {
            return Math.Round(value / 1000, 0) * 1000;
        }

        // ════════════════════════════════════════════════════════════════════════
        //  MAIN ENTRY POINT
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Calculates the minimum and maximum trip budget for a group.
        /// </summary>
        /// <param name="destinations">
        ///   List of governorate names selected by the user (must match keys in
        ///   the <see cref="_profiles"/> dictionary, case-insensitive).
        /// </param>
        /// <param name="totalDays">Total length of the trip in days.</param>
        /// <param name="adults">Number of adult travelers.</param>
        /// <param name="kids">
        ///   Number of child travelers (children 0–11 are counted at 0.6 of
        ///   the adult per-person cost — half-price tickets, kid's meals, etc.).
        /// </param>
        /// <param name="isHighSeason">
        ///   Pass <c>true</c> when the trip falls in a peak month so that
        ///   accommodation surcharges are applied per governorate.
        /// </param>
        /// <returns>A <see cref="TripBudgetEstimate"/> with full breakdown.</returns>
        public TripBudgetEstimate Calculate(
            List<string> destinations,
            int          totalDays,
            int          adults,
            int          kids          = 0,
            bool         isHighSeason  = false)
        {
            // ── 0. Guard & resolve profiles ──────────────────────────────────────
            if (destinations == null || destinations.Count == 0)
                throw new ArgumentException("At least one destination is required.", nameof(destinations));
            if (totalDays < 1)
                throw new ArgumentException("Trip must be at least 1 day.", nameof(totalDays));
            if (adults < 1)
                throw new ArgumentException("At least one adult traveler is required.", nameof(adults));

            // Resolve each destination name to its cost profile.
            // Unknown names fall back to a generic mid-Egypt baseline.
            var resolvedProfiles = destinations
                .Select(d => _profiles.TryGetValue(d, out var p) ? p : _getFallbackProfile(d))
                .ToList();

            // Children cost 60 % of the adult per-person price.
            // This "effective people count" is used for per-person buckets.
            double effectivePersonCount = adults + (kids * 0.6);
            int    totalPeople          = adults + kids;

            // ── 1. Distribute days across destinations ────────────────────────────
            //
            // If the user visits N destinations, we divide the total days equally.
            // The last slice absorbs any remainder (e.g., 7 days / 3 cities → 3, 2, 2).
            // This gives us a per-destination "nights" allocation to price accommodation.
            int destinationCount = resolvedProfiles.Count;
            int baseDaysPerDest  = totalDays / destinationCount;
            int remainderDays    = totalDays % destinationCount;

            // ── 2. Accumulate costs per bucket ───────────────────────────────────
            decimal minAccommodation   = 0;
            decimal midAccommodation   = 0;
            decimal maxAccommodation   = 0;
            decimal minFoodActivities  = 0;
            decimal midFoodActivities  = 0;
            decimal maxFoodActivities  = 0;
            decimal minLocalTransport  = 0;
            decimal midLocalTransport  = 0;
            decimal maxLocalTransport  = 0;
            decimal minInterCity       = 0;
            decimal midInterCity       = 0;
            decimal maxInterCity       = 0;

            for (int i = 0; i < destinationCount; i++)
            {
                var profile = resolvedProfiles[i];

                // This destination gets baseDays + 1 extra if it's in the "remainder" slots.
                int daysHere = baseDaysPerDest + (i < remainderDays ? 1 : 0);

                // ACCOMMODATION
                // Apply peak-season multiplier to accommodation only.
                double seasonFactor = isHighSeason ? profile.PeakSeasonMultiplier : 1.0;

                minAccommodation += (decimal)(
                    (double)(profile.AccommodationMinPerNight * daysHere) * seasonFactor
                    * effectivePersonCount
                );
                midAccommodation += (decimal)(
                    (double)(profile.AccommodationMidPerNight * daysHere) * seasonFactor
                    * effectivePersonCount
                );
                maxAccommodation += (decimal)(
                    (double)(profile.AccommodationMaxPerNight * daysHere) * seasonFactor
                    * effectivePersonCount
                );

                // FOOD & DAILY ACTIVITIES
                minFoodActivities += profile.DailyActivitiesMinPerPerson * daysHere
                                     * (decimal)effectivePersonCount;
                midFoodActivities += profile.DailyActivitiesMidPerPerson * daysHere
                                     * (decimal)effectivePersonCount;
                maxFoodActivities += profile.DailyActivitiesMaxPerPerson * daysHere
                                     * (decimal)effectivePersonCount;

                // LOCAL TRANSPORT (within the governorate)
                minLocalTransport += profile.LocalTransportMinPerDay * daysHere
                                     * (decimal)effectivePersonCount;
                midLocalTransport += profile.LocalTransportMidPerDay * daysHere
                                     * (decimal)effectivePersonCount;
                maxLocalTransport += profile.LocalTransportMaxPerDay * daysHere
                                     * (decimal)effectivePersonCount;

                // INTER-CITY TRAVEL (one-way fare × 2 for return, per person)
                if (i > 0 || profile.InterCityTravelMin > 0)
                {
                    decimal interCityMid = (profile.InterCityTravelMin + profile.InterCityTravelMax) / 2;
                    minInterCity += profile.InterCityTravelMin * 2 * (decimal)effectivePersonCount;
                    midInterCity += interCityMid              * 2 * (decimal)effectivePersonCount;
                    maxInterCity += profile.InterCityTravelMax * 2 * (decimal)effectivePersonCount;
                }
            }

            // ── 3. Combine transport sub-buckets ─────────────────────────────────
            decimal minTransport = minLocalTransport + minInterCity;
            decimal midTransport = midLocalTransport + midInterCity;
            decimal maxTransport = maxLocalTransport + maxInterCity;

            // ── 4. Sum raw totals (before group economy) ─────────────────────────
            decimal rawMin = minAccommodation + minFoodActivities + minTransport;
            decimal rawMid = midAccommodation + midFoodActivities + midTransport;
            decimal rawMax = maxAccommodation + maxFoodActivities + maxTransport;

            // ── 5. Apply group economy-of-scale multiplier ───────────────────────
            double groupMultiplier = GetGroupEconomyMultiplier(totalPeople);

            decimal finalMin = RoundToNearest1000(rawMin * (decimal)groupMultiplier);
            decimal finalMid = RoundToNearest1000(rawMid * (decimal)groupMultiplier);
            decimal finalMax = RoundToNearest1000(rawMax * (decimal)groupMultiplier);

            // ── 6. Return structured result ──────────────────────────────────────
            return new TripBudgetEstimate
            {
                MinTotalEGP              = finalMin,
                MidTotalEGP              = finalMid,
                MaxTotalEGP              = finalMax,

                // Breakdown (also scaled by group multiplier for consistency)
                MinAccommodationTotal    = RoundToNearest1000(minAccommodation * (decimal)groupMultiplier),
                MidAccommodationTotal    = RoundToNearest1000(midAccommodation * (decimal)groupMultiplier),
                MaxAccommodationTotal    = RoundToNearest1000(maxAccommodation * (decimal)groupMultiplier),
                MinFoodActivitiesTotal   = RoundToNearest1000(minFoodActivities * (decimal)groupMultiplier),
                MidFoodActivitiesTotal   = RoundToNearest1000(midFoodActivities * (decimal)groupMultiplier),
                MaxFoodActivitiesTotal   = RoundToNearest1000(maxFoodActivities * (decimal)groupMultiplier),
                MinTransportationTotal   = RoundToNearest1000(minTransport * (decimal)groupMultiplier),
                MidTransportationTotal   = RoundToNearest1000(midTransport * (decimal)groupMultiplier),
                MaxTransportationTotal   = RoundToNearest1000(maxTransport * (decimal)groupMultiplier),

                TotalPeople              = totalPeople,
                TotalDays                = totalDays,
                Destinations             = destinations,
                GroupEconomyMultiplier   = groupMultiplier,
            };
        }

        // ════════════════════════════════════════════════════════════════════════
        //  HELPER: DETECT HIGH SEASON FROM TRIP DATE
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Convenience helper: returns <c>true</c> if the trip's start month
        /// falls in a known Egyptian peak-tourism window.
        /// </summary>
        /// <remarks>
        ///   Peak windows:
        ///   - December / January  → Red Sea &amp; Nile Valley international tourists
        ///   - July / August       → Domestic coastal summer rush (Alex, Matrouh)
        ///   - March / April       → Nile cruise season + Easter holidays
        /// </remarks>
        public static bool IsHighSeason(DateTime startDate)
        {
            return startDate.Month is 12 or 1 or 7 or 8 or 3 or 4;
        }

        // ════════════════════════════════════════════════════════════════════════
        //  HELPER: FALLBACK PROFILE FOR UNKNOWN GOVERNORATES
        //  Uses conservative mid-Egypt averages so the estimate never crashes.
        // ════════════════════════════════════════════════════════════════════════
        private static GovernorateCostProfile _getFallbackProfile(string name) =>
            new()
            {
                Name                        = name,
                AccommodationMinPerNight    = 300,
                AccommodationMidPerNight    = 800,
                AccommodationMaxPerNight    = 2_500,
                DailyActivitiesMinPerPerson = 200,
                DailyActivitiesMidPerPerson = 600,
                DailyActivitiesMaxPerPerson = 1_500,
                LocalTransportMinPerDay     = 50,
                LocalTransportMidPerDay     = 180,
                LocalTransportMaxPerDay     = 450,
                InterCityTravelMin          = 200,
                InterCityTravelMax          = 800,
                PeakSeasonMultiplier        = 1.10,
            };
    }
}
