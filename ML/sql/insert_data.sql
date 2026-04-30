-- =============================================
-- STEP 1: Insert into dim_places
-- =============================================
BULK INSERT dim_places
FROM 'places.csv'
WITH (
    FORMAT          = 'CSV',
    FIRSTROW        = 2,          -- skip header row
    FIELDTERMINATOR = ',',
    ROWTERMINATOR   = '\n',
    CODEPAGE        = '65001'     -- UTF-8
);


-- =============================================
-- STEP 2: Insert into dim_restaurants
-- =============================================
BULK INSERT dim_restaurants
FROM 'restaurants.csv'
WITH (
    FORMAT          = 'CSV',
    FIRSTROW        = 2,
    FIELDTERMINATOR = ',',
    ROWTERMINATOR   = '\n',
    CODEPAGE        = '65001'
);


-- =============================================
-- STEP 3: Insert into dim_hotels
-- =============================================
BULK INSERT dim_hotels
FROM 'hotels.csv'
WITH (
    FORMAT          = 'CSV',
    FIRSTROW        = 2,
    FIELDTERMINATOR = ',',
    ROWTERMINATOR   = '\n',
    CODEPAGE        = '65001'
);

