USE [EgTripDb]; -- Change this to your actual database name
GO
UPDATE dbo.ReadyPlans
SET Description = N'Cairo short breaks tours for 4 days in Cairo Egypt.
take a break to Cairo and experience the grandness of Cairo, discover the Great Pyramids of Giza, the Grand Egyptian Museum, and more.',

Inclusions = N'● Meet and greet service by our representatives at airports
● Assistance of our guest relations during your stay
● All transfers by a private air-conditioned vehicle
● Accommodation in Cairo for 3 nights including bed and breakfast
● All sightseeing tours are strictly private tours
● Private English speaking guide
● Entrance fees to all sites as indicated on the itinerary
● Meals as indicated in the above itinerary
● Bottled water during the trips
● Portage when needed
● All service charges and taxes',

Exclusions = N'● Entry visa to Egypt
● Personal spending
● Optional activities
● Tipping',

Itinerary = N'Day 1: Arrival Cairo - Welcome to land of the Pharaohs. Your tour manager will meet and assist you at Cairo International Airport.

Day 2: Pyramids & GEM. Visit the Giza Plateau, the iconic Sphinx, and the New Grand Egyptian Museum.

Day 3: Museum of Egyptian Civilization, Old Cairo & Salah El-Din Citadel. 

Day 4: Cairo - Fly Back Home.'
, title = N'4-Day Cairo Short Break'
WHERE Id = 2;