USE [EgTripDb]; -- Change this to your actual database name
GO

-- Table Creation (if not already created)
IF OBJECT_ID('dbo.ReadyPlans', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ReadyPlans (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Title NVARCHAR(255),
        Rating DECIMAL(3, 2),
        Reviews NVARCHAR(MAX),
        Description NVARCHAR(MAX),
        Inclusions NVARCHAR(MAX),
        Exclusions NVARCHAR(MAX),
        Itinerary NVARCHAR(MAX),
        PhotoPath NVARCHAR(500)
    );
END
GO

-- Trip 1: 5 Days Trip to Egypt: Cairo and Luxor
INSERT INTO [dbo].[ReadyPlans] (
[Title],
[Rating],
[Reviews], 
[Description],
[Inclusions], 
[Exclusions],
[Itinerary],
[PhotoPath])
VALUES (
N'5 Days Trip to Egypt: Cairo and Luxor',
4.96,
N'1737 Reviews',
N'Discover Egypt’s essentials in five days on this private Cairo & Luxor tour... [Text truncated for brevity, full data in your CSV]',
N'Meet and greet service... All transfers in private vehicle... Domestic flights...',
N'International airfare... Entry visa... Optional tours...',
N'Day 1: Arrival... Day 2: Pyramids... Day 3: Luxor... Day 4: West Bank... Day 5: Departure.',
NULL);
GO

-- Trip 2: 4-Day Cairo Short Break
INSERT INTO [dbo].[ReadyPlans] ([Title], [Rating], [Reviews], [Description], [Inclusions], [Exclusions], [Itinerary], [PhotoPath])
VALUES (N'4-Day Cairo Short Break', 4.96, N'1737 Reviews', N'Experience the grandness of Cairo, discover the Great Pyramids, and the New Grand Egyptian Museum...', N'Airport assistance... Private AC vehicle... Professional guide...', N'Visas... Personal expenses... Drinks... Optional Sound & Light show...', N'Day 1: Welcome to Cairo... Day 2: Pyramids & GEM... Day 3: Old Cairo & Citadel... Day 4: Final Departure.', NULL);
GO

-- Trip 3: 8 Days Cairo and Nile Cruise by Flight
INSERT INTO [dbo].[ReadyPlans] ([Title], [Rating], [Reviews], [Description], [Inclusions], [Exclusions], [Itinerary], [PhotoPath])
VALUES (N'8 Days Cairo and Nile Cruise by Flight', 4.96, N'1737 Reviews', N'A comprehensive journey covering the capital and a luxury cruise between Luxor and Aswan...', N'Domestic flights... 4 nights on 5-star Nile Cruise... 3 nights in Cairo hotel...', N'International flights... Tipping... Laundry... Optional Abu Simbel tour...', N'Day 1-2: Cairo & Pyramids... Day 3-6: Nile Cruise... Day 7: Return to Cairo... Day 8: Departure.', NULL);
GO

-- Trip 4: 6 Days Cairo and Alexandria Tour
INSERT INTO [dbo].[ReadyPlans] ([Title], [Rating], [Reviews], [Description], [Inclusions], [Exclusions], [Itinerary], [PhotoPath])
VALUES (N'6 Days Cairo and Alexandria Tour', 4.96, N'1737 Reviews', N'Explore the wonders of Cairo and the Mediterranean beauty of Alexandria...', N'3 nights in Cairo... 2 nights in Alexandria... All transfers...', N'Visa... Tipping... Personal items...', N'Day 1: Arrival... Day 2: Pyramids... Day 3: Drive to Alexandria... Day 4: Alexandria sites... Day 5: Return to Cairo... Day 6: Departure.', NULL);
GO

-- Trip 5: 10 Days Egypt & Jordan tour: Cairo, Petra & Nile Cruise
INSERT INTO [dbo].[ReadyPlans] ([Title], [Rating], [Reviews], [Description], [Inclusions], [Exclusions], [Itinerary], [PhotoPath])
VALUES (N'10 Days Egypt & Jordan tour: Cairo, Petra & Nile Cruise', 4.96, N'1737 Reviews', N'An unusual trip through history to explore the mysterious greatness of the pharaohs and Petra...', N'Internal flights... Hotels... Cruise... Petra entry fees...', N'International airfare... Jordan Visa... Tipping...', N'Day 1-4: Cairo & Cruise... Day 5: Flight to Amman... Day 6: Petra tour... Day 10: Departure.', NULL);
GO

-- Trip 6: 12 Days Egypt & Morocco Tour: Cairo, Nile Cruise & Marrakesh
INSERT INTO [dbo].[ReadyPlans] ([Title], [Rating], [Reviews], [Description], [Inclusions], [Exclusions], [Itinerary], [PhotoPath])
VALUES (N'12 Days Egypt & Morocco Tour: Cairo, Nile Cruise & Marrakesh', 4.96, N'1737 Reviews', N'Start with the ancient life in Cairo and Luxor, then fly to Morocco to enjoy the top four cities...', N'All accommodation... Expert guides in both countries... Private transfers...', N'Visas... International flights... Personal expenses...', N'Day 1-7: Egypt itinerary... Day 8: Flight to Casablanca... Day 9-12: Morocco tours.', NULL);
GO

-- Trip 7: 15 Days Egypt, Dubai & Jordan: Pyramids, Petra & Burj Khalifa
INSERT INTO [dbo].[ReadyPlans] ([Title], [Rating], [Reviews], [Description], [Inclusions], [Exclusions], [Itinerary], [PhotoPath])
VALUES (N'15 Days Egypt, Dubai & Jordan: Pyramids, Petra & Burj Khalifa', 4.96, N'1737 Reviews', N'Visit the most important spots in Egypt, Jordan, and the modern wonders of Dubai...', N'All transfers... Guided tours in 3 countries... Breakfast daily...', N'Airfare... Visas... Optional activities...', N'Day 1-5: Egypt... Day 6-10: Jordan... Day 11-15: Dubai.', NULL);
GO

-- Trip 8: 19 Days Turkey, Greece & Egypt Tour
INSERT INTO [dbo].[ReadyPlans] ([Title], [Rating], [Reviews], [Description], [Inclusions], [Exclusions], [Itinerary], [PhotoPath])
VALUES (N'19 Days Turkey, Greece & Egypt Tour', 4.96, N'1737 Reviews', N'Discover the splendors and history. Explore the culture of Turkey, Greece and Egypt closely...', N'Hotels... Domestic flights... Ferries... Professional guides...', N'International flights... Personal expenses... Visas...', N'Day 1-6: Turkey... Day 7-12: Greece... Day 13-19: Egypt.', NULL);
GO

-- Trip 9: 4 Days Nile Cruise: Aswan to Luxor
INSERT INTO [dbo].[ReadyPlans] ([Title], [Rating], [Reviews], [Description], [Inclusions], [Exclusions], [Itinerary], [PhotoPath])
VALUES (N'4 Days Nile Cruise: Aswan to Luxor', 4.96, N'1737 Reviews', N'Experience the classic Nile sailing between Aswan and Luxor on a 5-star ship...', N'Full board on cruise... Sightseeing... AC transfers...', N'Drinks... Tipping... Optional Abu Simbel...', N'Day 1: Aswan arrival... Day 2: Kom Ombo/Edfu... Day 3: Luxor East Bank... Day 4: West Bank/Departure.', NULL);
GO

-- Trip 10: 5 Days Nile Cruise: Luxor to Aswan
INSERT INTO [dbo].[ReadyPlans] ([Title], [Rating], [Reviews], [Description], [Inclusions], [Exclusions], [Itinerary], [PhotoPath])
VALUES (N'5 Days Nile Cruise: Luxor to Aswan', 4.96, N'1737 Reviews', N'Sail south from Luxor to Aswan, visiting the great temples along the river banks...', N'Accommodation... All meals... Egyptologist guide...', N'Tipping... Personal items... Extra tours...', N'Day 1: Luxor arrival... Day 2: West Bank... Day 3: Esna/Edfu... Day 4: Kom Ombo/Aswan... Day 5: Departure.', NULL);
GO

-- Trip 11: 8 Days Egypt Family Holiday with Nile Cruise
INSERT INTO [dbo].[ReadyPlans] ([Title], [Rating], [Reviews], [Description], [Inclusions], [Exclusions], [Itinerary], [PhotoPath])
VALUES (N'8 Days Egypt Family Holiday with Nile Cruise', 4.96, N'1737 Reviews', N'The perfect family getaway exploring the wonders of Cairo and the Nile River...', N'Hotels... Cruise ship... Child-friendly activities... All transfers...', N'Visa... International flights... Soft drinks...', N'Day 1-2: Cairo... Day 3-6: Nile Cruise... Day 7: Cairo return... Day 8: Departure.', NULL);
GO

-- Final check of inserted rows
SELECT * FROM dbo.ReadyPlans;



