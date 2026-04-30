CREATE DATABASE eg_trip_db;
USE eg_trip_db;


-- =============================================
-- 1. dim_hotels
-- =============================================
CREATE TABLE dim_hotels (
    hotel_id      INT            IDENTITY(1,1)  PRIMARY KEY,
    city          NVARCHAR(100)  NOT NULL,
    hotel_name    NVARCHAR(255)  NOT NULL,
    price         FLOAT          NULL,
    review_score  DECIMAL(3,1)   NULL,
    hotel_url     NVARCHAR(MAX)  NULL,
    description   NVARCHAR(MAX)  NULL,
    latitude      FLOAT          NULL,
    longitude     FLOAT          NULL,
    distance_km   DECIMAL(6,2)   NULL,
);

-- =============================================
-- 2. dim_places
-- =============================================
CREATE TABLE dim_places (
    place_id      INT            IDENTITY(1,1)  PRIMARY KEY,
    city          NVARCHAR(100)  NOT NULL,
    title         NVARCHAR(255)  NOT NULL,
    rating        DECIMAL(3,1)   NULL,
    reviews       FLOAT          NULL,
    description   NVARCHAR(MAX)  NULL,
    tips          NVARCHAR(MAX)  NULL,
    address       NVARCHAR(500)  NULL,
    timings       NVARCHAR(300)  NULL,
    ticket_price  FLOAT          NULL
);


-- =============================================
-- 3. dim_restaurants
-- =============================================
CREATE TABLE dim_restaurants (
    restaurant_id   INT            IDENTITY(1,1)  PRIMARY KEY,
    city            NVARCHAR(100)  NOT NULL,
    restaurant_name NVARCHAR(255)  NOT NULL,
    location        NVARCHAR(500)  NULL,
    cuisines        NVARCHAR(500)  NULL,
    url             NVARCHAR(MAX)  NULL,
    total_items     INT            NULL,
    prices_list     NVARCHAR(MAX)  NULL,
    min_price       FLOAT          NULL,
    max_price       FLOAT          NULL,
    avg_price       FLOAT          NULL
);


-- =============================================
-- 4. fact_trip
-- =============================================
CREATE TABLE fact_trip (
    trip_id        BIGINT  IDENTITY(1,1)  PRIMARY KEY,
    hotel_id       INT     NULL,
    restaurant_id  INT     NULL,
    place_id       INT     NULL,

    CONSTRAINT FK_trip_hotel      FOREIGN KEY (hotel_id)
        REFERENCES dim_hotels      (hotel_id)      ON DELETE SET NULL, 

    CONSTRAINT FK_trip_restaurant FOREIGN KEY (restaurant_id)
        REFERENCES dim_restaurants (restaurant_id) ON DELETE SET NULL,

    CONSTRAINT FK_trip_place      FOREIGN KEY (place_id)
        REFERENCES dim_places      (place_id)      ON DELETE SET NULL
);