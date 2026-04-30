/**
 * apiService.js
 * Centralized utility for managing HTTP network requests to the backend API.
 */

const API_BASE_URL = window.location.origin; // Dynamically uses the current host/port (e.g., https://localhost:7292)

/**
 * Sends the user's travel preferences to the C# ASP.NET Core backend bridge,
 * which in turn queries the Python AI Machine Learning model.
 *
 * @param {Object} tripData - The user's preferences for the trip.
 * @param {string[]} tripData.destinations - Array of destination cities (e.g., ["Cairo", "Luxor"]).
 * @param {string} tripData.budget - The budget range (e.g., "1000-2000 EGP").
 * @param {Object} tripData.travelers - Object containing travelers count (e.g., { adults: 2, kids: 0 }).
 * @param {string} tripData.startDate - ISO Date string for trip start.
 * @param {string} tripData.endDate - ISO Date string for trip end.
 * @param {string[]} tripData.travelStyles - Array of preferred travel styles (e.g., ["Historical", "Food"]).
 * @returns {Promise<Object>} The generated AI trip itinerary payload.
 */
async function generateAITrip(tripData) {
    const endpoint = `${API_BASE_URL}/api/TripMl/Generate`;

    // Construct the payload mapping to the C# controller's expected model
    const payload = {
        destinations: tripData.destinations || [],
        budget: tripData.budget || "",
        travelers: {
            adults: tripData.travelers?.adults || 1,
            kids: tripData.travelers?.kids || 0
        },
        startDate: tripData.startDate || new Date().toISOString(),
        endDate: tripData.endDate || new Date().toISOString(),
        travelStyles: tripData.travelStyles || []
    };

    try {
        console.log("Initiating AI Trip Generation with payload:", payload);

        const response = await fetch(endpoint, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Accept": "application/json"
            },
            body: JSON.stringify(payload)
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(`HTTP error! Status: ${response.status} - ${errorText}`);
        }

        const data = await response.json();
        
        // Output the resulting AI itinerary JSON payload directly to the web browser's console
        console.log("✅ AI Trip Generated Successfully! Payload received:");
        console.dir(data, { depth: null });
        
        return data;

    } catch (error) {
        console.error("❌ Failed to generate AI trip. Network or server error occurred:", error);
        throw error;
    }
}

// Export functions if using modules, otherwise they are available globally
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { generateAITrip };
}
