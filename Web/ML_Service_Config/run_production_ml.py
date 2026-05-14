import sys
import os

# Add the cloned ML repo path here so Python can find the modules
# Replace this path with the actual path where you clone 'https://github.com/seifkhalled/not_final_project'
ML_REPO_PATH = r"c:\Users\Felo\Documents\GitHub\not_final_project"
sys.path.insert(0, ML_REPO_PATH)


from dotenv import load_dotenv
env_file = os.path.join(os.path.dirname(__file__), ".env")
load_dotenv(env_file)
from waitress import serve
from api_server import app


if __name__ == '__main__':
    print("=========================================================")
    print("  Starting Egypt Trip Planner ML Service (Waitress)  ")
    print("=========================================================")
    print("Initializing Models... (This only happens once!)")
    print("- Loading ChromaDB Vector Search Database...")
    print("- Loading sentence-transformers/paraphrase-multilingual-MiniLM-L12-v2...")
    print("=========================================================")
    print("Server running on http://127.0.0.1:8585")
    
    # Waitress is a production WSGI server for Windows/Linux
    # It ensures the Flask app (and its loaded ML models) stay in memory efficiently.
    serve(app, host='127.0.0.1', port=8585)
