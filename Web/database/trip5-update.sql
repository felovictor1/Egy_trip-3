USE [EgTripDb]; -- Change this to your actual database name
GO
UPDATE dbo.ReadyPlans
SET Description = N'Enjoy this Cairo City BreakÂ for 5 days in CairoEgypt, a fantastic weekend holiday to Cairo ancient and modern sites like the greatPyramidsand the GrandÂ Egyptian Museum.
',

Inclusions = N'● Meet and greet service by our representatives at airports
● Assistance of our guest relations during your stay
● All transfers by a private air-conditioned vehicle
● Accommodation in Cairo for 4 nights including bed and breakfast
● All sightseeing tours are strictly private tours
● Private English speaking guide
● Entrance fees to all sites as indicated on the itinerary
● Meals as indicated in the above itinerary
● Bottled water during the trips
● Portage when needed
● All service charges and taxes',

Exclusions = N'● Entry visa to Egypt
● Personal spendings
● Optional activities
● Tipping',

Itinerary = N'● Day 1: Arrival Cairo: Welcome to Cairo, Egypt. Your tour manager will meet and assist you at Cairo International Airport. Then he will escort you to the hotel by exclusive air-conditioned deluxe vehicle. Overnight in Cairo.
● Day 2: Pyramids & Sakkara Tour: After enjoying a delicious wholesome Egyptian breakfast, your personal guide will take you to the Giza Plateau to marvel at the Great Pyramids of Giza, Sphinx and Valley Temple. Visit the Sakkara Complex and the first pyramid ever built, the Djoser Pyramid. Overnight in Cairo. Meals: Breakfast, Lunch.
● Day 3: The Grand Egyptian Museum: After breakfast, visit The New Grand Egyptian Museum, showcasing over 100,000 artifacts. After that, lunch at an authentic Egyptian restaurant. Overnight in Cairo. Meals: Breakfast, Lunch.
● Day 4: Optional Tour to Alexandria: Breakfast at your hotel. Free day in Cairo or join an optional tour to Alexandria visiting Qaitbay Citadel, Alexandria Library, and the Catacombs. Return to your hotel in Cairo. Meals: Breakfast.
● Day 5: Final Departure: After breakfast in your hotel, transfer to Cairo International Airport for departure. Meals: Breakfast'
,Title=N'5-Day Cairo Short Break'
WHERE Id = 5;