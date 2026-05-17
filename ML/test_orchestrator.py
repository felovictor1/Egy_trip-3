import sys
import os
from dotenv import load_dotenv

BASE_DIR = os.path.dirname(os.path.abspath(__file__))
env_path = os.path.join(BASE_DIR, "..", "Web", "ML_Service_Config", ".env")
load_dotenv(env_path, override=True)

sys.path.insert(0, BASE_DIR)

from ai.orchestrator import TripOrchestrator

orchestrator = TripOrchestrator()

print("Generating test trip plan...")
from datetime import datetime
try:
    result = orchestrator.generate_trip_plan(
        destinations=["Cairo & Giza"],
        budget="5000 EGP",
        group_size=2,
        start_date=datetime.fromisoformat("2026-04-22T00:00:00Z".replace("Z", "+00:00")),
        end_date=datetime.fromisoformat("2026-04-26T00:00:00Z".replace("Z", "+00:00")),
        travel_styles=["Historical", "Food & Dining"],
        historical_knowledge="Beginner",
        preferred_time_periods=["Pharaonic", "Islamic"],
        museum_visits=True,
        water_activities=False,
        accommodation_type="Luxury",
        transportation="Private Car",
        food_preferences="Vegetarian",
        trip_pace="Moderate",
        must_visit=None,
        places=[],
        restaurants=[],
        hotels=[],
        model=None,
        provider="groq"
    )
    print("FINAL RESULT TEXT:")
    print(result)
except Exception as e:
    import traceback
    traceback.print_exc()
    print("FAILED TO GENERATE:", e)
