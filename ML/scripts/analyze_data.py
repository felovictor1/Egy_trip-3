import pandas as pd
import numpy as np

print("=" * 100)
print("RESTAURANTS.CSV ANALYSIS")
print("=" * 100)

df_rest = pd.read_csv(r"C:\Users\DELL\Downloads\grad project\grad project\Data\restaurants.csv")

print("\n--- SHAPE ---")
print(df_rest.shape)

print("\n--- COLUMNS ---")
print(df_rest.columns.tolist())

print("\n--- DTYPES ---")
print(df_rest.dtypes)

print("\n--- NULL COUNTS ---")
print(df_rest.isnull().sum())

print("\n--- NULL PERCENTAGES ---")
print((df_rest.isnull().sum() / len(df_rest) * 100).round(2))

print("\n--- FIRST 5 ROWS ---")
print(df_rest.head())

print("\n--- UNIQUE CITIES ---")
print(df_rest['city'].value_counts())

print("\n--- UNIQUE CUISINES (TOP 20) ---")
all_cuisines = df_rest['Cuisines'].dropna().str.split(',').explode().str.strip()
print(all_cuisines.value_counts().head(20))

print("\n--- DISTRIBUTION OF Total_Items ---")
print(df_rest['Total_Items'].describe())
print("\nValue counts:")
print(df_rest['Total_Items'].value_counts().sort_index())

print("\n--- Min_Price / Max_Price / Avg_Price ---")
for col in ['Min_Price', 'Max_Price', 'Avg_Price']:
    print(f"\n{col}:")
    print(df_rest[col].describe())

print("\n--- Sample URLs ---")
print(df_rest['URL'].head(10))

print("\n--- Sample Prices_List ---")
print(df_rest['Prices_List'].head(5))

print("\n\n" + "=" * 100)
print("PLACES.CSV ANALYSIS")
print("=" * 100)

df_places = pd.read_csv(r"C:\Users\DELL\Downloads\grad project\grad project\Data\places.csv")

print("\n--- SHAPE ---")
print(df_places.shape)

print("\n--- COLUMNS ---")
print(df_places.columns.tolist())

print("\n--- DTYPES ---")
print(df_places.dtypes)

print("\n--- NULL COUNTS ---")
print(df_places.isnull().sum())

print("\n--- NULL PERCENTAGES ---")
print((df_places.isnull().sum() / len(df_places) * 100).round(2))

print("\n--- FIRST 5 ROWS ---")
print(df_places.head())

print("\n--- UNIQUE CITIES ---")
print(df_places['City'].value_counts())

print("\n--- RATING DISTRIBUTION ---")
print(df_places['Rating'].describe())
print("\nRating bins:")
print(pd.cut(df_places['Rating'].dropna(), bins=[0, 2, 3, 4, 5]).value_counts().sort_index())

print("\n--- REVIEWS DISTRIBUTION ---")
print(df_places['Reviews'].describe())

print("\n--- DESCRIPTION LENGTH STATS ---")
desc_lengths = df_places['Description'].dropna().str.len()
print(f"Count: {len(desc_lengths)}")
print(f"Min: {desc_lengths.min()}")
print(f"Max: {desc_lengths.max()}")
print(f"Mean: {desc_lengths.mean():.1f}")
print(f"Median: {desc_lengths.median():.1f}")

print("\n--- TIPS LENGTH STATS ---")
tips_lengths = df_places['Tips'].dropna().str.len()
print(f"Count: {len(tips_lengths)}")
print(f"Min: {tips_lengths.min()}")
print(f"Max: {tips_lengths.max()}")
print(f"Mean: {tips_lengths.mean():.1f}")

print("\n--- TICKET PRICE ---")
print(df_places['Ticket Price'].value_counts().head(20))

print("\n--- TIMINGS SAMPLE ---")
print(df_places['Timings'].dropna().head(10))

print("\n--- ADDRESS SAMPLE ---")
print(df_places['Address'].dropna().head(5))


print("\n\n" + "=" * 100)
print("HOTELS.CSV ANALYSIS")
print("=" * 100)

df_hotels = pd.read_csv(r"C:\Users\DELL\Downloads\grad project\grad project\Data\hotels.csv")

print("\n--- SHAPE ---")
print(df_hotels.shape)

print("\n--- COLUMNS ---")
print(df_hotels.columns.tolist())

print("\n--- DTYPES ---")
print(df_hotels.dtypes)

print("\n--- NULL COUNTS ---")
print(df_hotels.isnull().sum())

print("\n--- NULL PERCENTAGES ---")
print((df_hotels.isnull().sum() / len(df_hotels) * 100).round(2))

print("\n--- FIRST 5 ROWS ---")
print(df_hotels.head())

print("\n--- UNIQUE CITIES ---")
print(df_hotels['city'].value_counts())

print("\n--- PRICE DISTRIBUTION ---")
print(df_hotels['Price'].describe())

print("\n--- REVIEW SCORE DISTRIBUTION ---")
print(df_hotels['Review Score (/10)'].describe())
print("\nReview score bins:")
print(pd.cut(df_hotels['Review Score (/10)'].dropna(), bins=[0, 5, 6, 7, 8, 9, 10]).value_counts().sort_index())

print("\n--- LATITUDE / LONGITUDE ---")
print(f"Latitude: min={df_hotels['Latitude'].min()}, max={df_hotels['Latitude'].max()}, nulls={df_hotels['Latitude'].isnull().sum()}")
print(f"Longitude: min={df_hotels['Longitude'].min()}, max={df_hotels['Longitude'].max()}, nulls={df_hotels['Longitude'].isnull().sum()}")

print("\n--- DISTANCE (km) ---")
print(df_hotels['Distance (km)'].describe())

print("\n--- SAMPLE DESCRIPTIONS ---")
print(df_hotels['Description'].dropna().head(3))

print("\n--- Hotel URL SAMPLE ---")
print(df_hotels['Hotel URL'].head(5))

print("\n\n" + "=" * 100)
print("CROSS-FILE ANALYSIS: Common Cities")
print("=" * 100)

rest_cities = set(df_rest['city'].dropna().str.strip().str.lower())
places_cities = set(df_places['City'].dropna().str.strip().str.lower())
hotels_cities = set(df_hotels['city'].dropna().str.strip().str.lower())

print(f"\nRestaurants cities: {len(rest_cities)}")
print(f"Places cities: {len(places_cities)}")
print(f"Hotels cities: {len(hotels_cities)}")

print(f"\nCities in ALL 3 files: {len(rest_cities & places_cities & hotels_cities)}")
print(rest_cities & places_cities & hotels_cities)

print(f"\nCities in Restaurants but NOT in Places: {rest_cities - places_cities}")
print(f"\nCities in Places but NOT in Restaurants: {places_cities - rest_cities}")
print(f"\nCities in Hotels but NOT in Restaurants: {hotels_cities - rest_cities}")
