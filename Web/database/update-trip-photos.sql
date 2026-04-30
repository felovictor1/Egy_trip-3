/* This script updates the PhotoPath column in the ReadyPlans table for specific records based on their Id.*/
USE [EgTripDb]; -- Change this to your actual database name
GO
UPDATE dbo.ReadyPlans 
SET PhotoPath = 'https://images.memphistours.com/thumbs1/382870437_egypt-tours-cairo-luxor-and-aswan-26.heic'
WHERE id = 1;

UPDATE dbo.ReadyPlans 
SET PhotoPath = 'https://images.memphistours.com/thumbs1/1076837289_Female-in-vacation-standing-in-front-of-the-great-pyramids.jpg'
WHERE id = 2;

UPDATE dbo.ReadyPlans 
SET PhotoPath = 'https://images.memphistours.com/thumbs1/1579186944_4-day%20Salacia%20Nile%20Cruise.jpg'
WHERE id = 3;

UPDATE dbo.ReadyPlans 
SET PhotoPath = 'https://images.memphistours.com/thumbs1/1325058248_websitesize-15.jpg'
WHERE id = 4;

UPDATE dbo.ReadyPlans 
SET PhotoPath = 'https://images.memphistours.com/thumbs1/444773801_Stanley-Bridge-Alexandria-min.jpg'
WHERE id = 5;

UPDATE dbo.ReadyPlans 
SET PhotoPath = 'https://images.memphistours.com/thumbs1/289147859_websitesize-03.jpg'
WHERE id = 6;

UPDATE dbo.ReadyPlans 
SET PhotoPath = 'https://images.memphistours.com/thumbs1/1590679074_5-day-Luxury-Nile-Cruise-During-Easter.jpg'
WHERE id = 7;

UPDATE dbo.ReadyPlans 
SET PhotoPath = 'https://images.memphistours.com/thumbs1/1710654641_egypt-tours-cairo-luxor-and-aswan-39.jpg'
WHERE id = 8;

UPDATE dbo.ReadyPlans 
SET PhotoPath = 'https://images.memphistours.com/thumbs1/33603824_dwa.jpg'
WHERE id = 9;


UPDATE dbo.ReadyPlans 
SET PhotoPath = 'https://images.memphistours.com/thumbs1/1525675619_MS-Salacia-Nile-Cruise%20%282%29.jpg'
WHERE id = 10;


UPDATE dbo.ReadyPlans 
SET PhotoPath = 'https://images.memphistours.com/thumbs1/707002998_95.jpg'
WHERE id = 11;

