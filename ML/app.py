import streamlit as st
import os
from dotenv import load_dotenv
from datetime import datetime
import sys

sys.path.insert(0, os.path.join(os.path.dirname(__file__), "src"))
from vector_search import search
from ai.orchestrator import TripOrchestrator

load_dotenv()

orchestrator = TripOrchestrator()

st.set_page_config(page_title="Egypt Trip Planner", page_icon="🇪🇬", layout="wide")

st.title("🇪🇬 Egypt Trip Planner")
st.caption("Answer a few questions and we'll build your perfect itinerary")

# ==============================
# SIDEBAR - Travel Styles
# ==============================
st.sidebar.header("🎯 Travel Styles")
travel_styles = st.sidebar.multiselect(
    "What are your travel styles?",
    options=["Historical", "Food & Dining", "Adventure", "Relaxation", "Shopping", "Nature & Wildlife", "Cultural", "Nightlife"],
    default=["Historical", "Food & Dining"],
)

# ==============================
# TABS
# ==============================
tab1, tab2, tab3 = st.tabs(["📍 Core Constraints", "🧭 Dynamic Preferences", "🚗 Logistics"])

# --- Tab 1: Core Constraints ---
with tab1:
    st.header("Core Constraints")

    col1, col2 = st.columns(2)

    with col1:
        destinations = st.multiselect(
            "Destinations",
            options=["Cairo & Giza", "Alexandria", "Luxor", "Aswan", "Sharm El Sheikh", "Hurghada", "Dahab"],
            default=["Cairo & Giza"],
        )

        budget = st.selectbox(
            "Total Budget (EGP)",
            options=["500-1000 EGP", "1000-2000 EGP", "2000-5000 EGP", "5000-10000 EGP", "10000+ EGP"],
            index=1,
        )

    with col2:
        group_size = st.number_input("Group Size", min_value=1, max_value=20, value=2)

        date_cols = st.columns(2)
        with date_cols[0]:
            start_date = st.date_input("Start Date", value=datetime(2026, 3, 16))
        with date_cols[1]:
            end_date = st.date_input("End Date", value=datetime(2026, 3, 17))

# --- Tab 2: Dynamic Preferences ---
with tab2:
    st.header("Dynamic Preferences")

    col1, col2 = st.columns(2)

    with col1:
        historical_knowledge = st.selectbox(
            "Historical Knowledge Level",
            options=["Beginner", "Intermediate", "Expert"],
            index=0,
        )

        preferred_time_periods = st.multiselect(
            "Preferred Time Periods",
            options=["Pharaonic", "Islamic", "Coptic", "Roman", "Modern"],
            default=["Pharaonic", "Islamic"],
        )

    with col2:
        museum_visits = st.checkbox("Include Museum Visits", value=True)
        water_activities = st.checkbox("Include Water Activities", value=False)

# --- Tab 3: Logistics ---
with tab3:
    st.header("Logistics")

    col1, col2 = st.columns(2)

    with col1:
        accommodation_type = st.selectbox(
            "Accommodation Type",
            options=["Budget", "Medium", "Luxury"],
            index=1,
        )

        transportation = st.selectbox(
            "Transportation Preference",
            options=["Private Car", "Public Transport", "Walking", "Taxi/Rideshare"],
            index=0,
        )

    with col2:
        food_preferences = st.selectbox(
            "Food Preferences",
            options=["Vegetarian", "Non-Vegetarian", "Vegan", "Halal", "No Preference"],
            index=0,
        )

        trip_pace = st.selectbox(
            "Trip Pace",
            options=["Relaxed", "Moderate", "Fast"],
            index=1,
        )

    must_visit = st.text_input("Must Visit Places (comma-separated)", value="Pyramids")

# ==============================
# GENERATE PLAN
# ==============================
st.divider()

if st.button("🗺️ Generate My Trip Plan", type="primary", use_container_width=True):
    budget_min, budget_max = map(int, budget.replace(" EGP", "").replace("+", "-99999").split("-"))

    city_map = {
        "Cairo & Giza": "cairo",
        "Alexandria": "alexandria",
        "Luxor": "luxor",
        "Aswan": "aswan",
        "Sharm El Sheikh": "sharm",
    }

    query_parts = []

    if "Historical" in travel_styles:
        if "Pharaonic" in preferred_time_periods:
            query_parts.append("pharaonic ancient pyramids temple historical site")
        if "Islamic" in preferred_time_periods:
            query_parts.append("islamic mosque old cairo historic")
        if "Coptic" in preferred_time_periods:
            query_parts.append("coptic church ancient christian")
        if museum_visits:
            query_parts.append("museum exhibition artifacts")
        if historical_knowledge == "Beginner":
            query_parts.append("introductory guided tour overview")

    if "Food & Dining" in travel_styles:
        if food_preferences == "Vegetarian":
            query_parts.append("vegetarian restaurant plant-based food")
        elif food_preferences == "Vegan":
            query_parts.append("vegan restaurant plant-based food")
        else:
            query_parts.append("restaurant local cuisine dining food")

    if "Water Activities" in travel_styles or water_activities:
        query_parts.append("water activities beach diving snorkeling")

    if accommodation_type == "Budget":
        query_parts.append("budget cheap affordable hotel")
    elif accommodation_type == "Medium":
        query_parts.append("mid-range moderate hotel comfortable")
    elif accommodation_type == "Luxury":
        query_parts.append("luxury premium 5-star resort spa")

    if must_visit:
        query_parts.append(must_visit)

    query = " ".join(query_parts) if query_parts else "tourist attractions restaurants hotels"

    results_places = []
    results_restaurants = []
    results_hotels = []

    for city_label in destinations:
        city = city_map.get(city_label, city_label.lower())

        places = search(query=query, entity_type="place", city=city, k=5)
        results_places.extend(places)

        restaurants = search(query=query, entity_type="restaurant", city=city, max_price=budget_max, k=5)
        results_restaurants.extend(restaurants)

        hotels = search(query=query, entity_type="hotel", city=city, max_price=budget_max, k=5)
        results_hotels.extend(hotels)

    st.subheader(f"📅 Your Trip: {start_date.strftime('%b %d')} - {end_date.strftime('%b %d, %Y')}")
    st.caption(f"Destinations: {', '.join(destinations)} | Budget: {budget} | Group: {group_size} | Pace: {trip_pace}")

    # LLM-generated summary
    if results_places or results_restaurants or results_hotels:
        with st.spinner("AI is generating your trip summary..."):
            try:
                summary = orchestrator.generate_summary(results_places, results_restaurants, results_hotels, destinations)
                st.info(summary)
                if orchestrator.last_summary_tokens:
                    t = orchestrator.last_summary_tokens
                    st.caption(f"Tokens — Input: {t.prompt_tokens} | Output: {t.completion_tokens} | Total: {t.total_tokens}")
            except Exception as e:
                st.warning(f"Could not generate summary: {e}")

    if results_places:
        st.divider()
        st.subheader("🏛️ Places to Visit")
        for r in results_places:
            meta = r["metadata"]
            cols = st.columns([3, 1, 1])
            cols[0].markdown(f"**{r['name']}**")
            cols[1].caption(f"⭐ {meta.get('rating', 'N/A')}")
            ticket = meta.get('ticket_price')
            cols[2].caption(f"🎫 {ticket} EGP" if ticket else "🎫 Free/Unknown")
            if meta.get("address"):
                st.caption(f"📍 {meta['address']}")

    if results_restaurants:
        st.divider()
        st.subheader("🍽️ Restaurants")
        for r in results_restaurants:
            meta = r["metadata"]
            cols = st.columns([3, 1, 1])
            cols[0].markdown(f"**{r['name']}**")
            cuisines = meta.get("cuisines", "")
            cols[1].caption(f"🍴 {cuisines}" if cuisines else "")
            avg_price = meta.get("avg_price")
            cols[2].caption(f"💰 ~{avg_price} EGP" if avg_price else "")

    if results_hotels:
        st.divider()
        st.subheader("🏨 Hotels")
        for r in results_hotels:
            meta = r["metadata"]
            cols = st.columns([3, 1, 1])
            cols[0].markdown(f"**{r['name']}**")
            cols[1].caption(f"⭐ {meta.get('rating', 'N/A')}/10")
            price = meta.get("price")
            cols[2].caption(f"💰 {price} EGP" if price else "")

    # ==============================
    # AI-GENERATED FULL TRIP PLAN
    # ==============================
    st.divider()
    st.subheader("🤖 AI-Generated Full Trip Plan")
    st.caption("Powered by LLaMA 3.1-8B + Tavily Real-Time Search")

    with st.spinner("Generating your personalized trip plan... This may take a moment."):
        try:
            trip_plan = orchestrator.generate_trip_plan(
                destinations=destinations,
                budget=budget,
                group_size=group_size,
                start_date=start_date,
                end_date=end_date,
                travel_styles=travel_styles,
                historical_knowledge=historical_knowledge,
                preferred_time_periods=preferred_time_periods,
                museum_visits=museum_visits,
                water_activities=water_activities,
                accommodation_type=accommodation_type,
                transportation=transportation,
                food_preferences=food_preferences,
                trip_pace=trip_pace,
                must_visit=must_visit,
                places=results_places,
                restaurants=results_restaurants,
                hotels=results_hotels,
            )
            st.markdown(trip_plan)
            if orchestrator.last_trip_plan_tokens:
                t = orchestrator.last_trip_plan_tokens
                st.caption(f"Tokens — Input: {t.prompt_tokens} | Output: {t.completion_tokens} | Total: {t.total_tokens}")
        except Exception as e:
            st.error(f"Failed to generate trip plan: {e}")
            st.info("Please check your GROQ_API_KEY and try again.")

    # ==============================
    # SUMMARY JSON
    # ==============================
    st.divider()
    st.subheader("📋 Your Preferences Summary")
    summary = {
        "core_constraints": {
            "destinations": destinations,
            "total_budget_egp": budget,
            "group_size": group_size,
            "travel_dates": {
                "start_date": start_date.isoformat(),
                "end_date": end_date.isoformat(),
            },
        },
        "travel_styles": travel_styles,
        "dynamic_preferences": {
            "historical_knowledge": historical_knowledge,
            "preferred_time_periods": preferred_time_periods,
            "museum_visits": museum_visits,
            "water_activities": water_activities if water_activities else None,
        },
        "logistics": {
            "accommodation_type": accommodation_type,
            "transportation_preference": transportation,
            "food_preferences": food_preferences,
            "trip_pace": trip_pace,
            "must_visit_places": must_visit,
        },
    }
    st.json(summary)
