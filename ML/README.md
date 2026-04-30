# Egypt Trip Planner

A smart trip planner for Egypt that uses ChromaDB vector database with semantic search and metadata filtering to help users discover restaurants, tourist attractions, and hotels across Egyptian cities.

## Architecture

```
┌─────────────────────────────────────────────────────────┐
│                    Vercel (Frontend)                     │
│                                                         │
│  Next.js 14 App Router (React + TypeScript)             │
│  ┌───────────────────────────────────────────────────┐  │
│  │  UI Components → api.ts → POST /api/trip          │  │
│  └──────────────────────┬────────────────────────────┘  │
│                         │                               │
│                         │ HTTP POST (JSON)              │
└─────────────────────────┼───────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────┐
│               Modal Cloud (Backend)                      │
│                                                         │
│  Flask API Server (api_server.py)                       │
│  ┌───────────────────────────────────────────────────┐  │
│  │  POST /api/trip                                    │  │
│  │    ├── Vector Search (ChromaDB + Volume)           │  │
│  │    ├── AI Orchestrator (Groq LLaMA 3.1-8B)        │  │
│  │    └── Tavily Real-Time Search                     │  │
│  └───────────────────────────────────────────────────┘  │
│                                                         │
│  ChromaDB ← persisted on Modal Volume                   │
│  Embedding Model: paraphrase-multilingual-MiniLM-L12-v2 │
└─────────────────────────────────────────────────────────┘
```

## Requirements

- **Python**: 3.11+
- **Node.js**: 20+
- **npm** or **yarn**

## Overview

This project builds a vector database from tourism data to enable intelligent trip planning through:
- **Semantic Search**: Find places, restaurants, and hotels using natural language queries
- **Metadata Filtering**: Filter results by city, price range, ratings, cuisine type, and more
- **Multi-Entity Support**: Covers restaurants, tourist attractions, and hotels
- **AI-Powered Itineraries**: LLaMA 3.1-8B generates full day-by-day trip plans with real-time Tavily search

## Data Sources

| File | Rows | Description |
|------|------|-------------|
| `restaurants.csv` | 1,225 | Restaurant listings from Talabat with menu items and pricing |
| `places.csv` | 391 | Tourist attractions with descriptions, ratings, and ticket prices |
| `hotels.csv` | 2,550 | Hotel listings from Booking.com with reviews and geolocation |

### Coverage by City

| City | Restaurants | Places | Hotels |
|------|-------------|--------|--------|
| Alexandria | 762 | 58 | 730 |
| Cairo | 463 | 158 | 776 |
| Luxor | - | 66 | 474 |
| Sharm El Sheikh | - | 41 | 570 |
| Aswan | - | 38 | - |

## Embedding Model

This project uses **sentence-transformers/paraphrase-multilingual-MiniLM-L12-v2** — a multilingual embedding model that supports both Arabic and English with high-quality semantic search. It handles mixed Arabic-English queries natively.

## Project Structure

```
grad project/
├── data/                    # CSV data files
│   ├── restaurants.csv
│   ├── places.csv
│   └── hotels.csv
├── src/                     # Core Python modules
│   ├── build_vector_db.py   # Build ChromaDB with embeddings
│   └── vector_search.py     # Search with metadata filtering
├── ai/                      # AI orchestrator (LLM + tools)
│   └── orchestrator.py
├── frontend/                # Next.js frontend
│   ├── src/
│   │   ├── app/
│   │   ├── components/
│   │   ├── lib/
│   │   ├── store/
│   │   └── types/
│   ├── .env.example
│   └── package.json
├── chroma_db/               # Generated ChromaDB (gitignored)
├── .venv/                   # Virtual environment (gitignored)
├── .gitignore
├── requirements.txt
├── app.py                   # Streamlit web app (legacy)
├── api_server.py            # Flask API server
├── modal_app.py             # Modal cloud deployment config
└── README.md
```

## Features

### Bilingual Semantic Search (Arabic + English)
- Queries in Arabic: "مطعم رخيص قريب من البحر"
- Queries in English: "cheap seafood restaurant near beach"
- Mixed queries work seamlessly

### ChromaDB Native Metadata Filtering
- **City**: `city="alexandria"`
- **Entity Type**: `entity_type="hotel" | "restaurant" | "place"`
- **Price Range**: `min_price=100`, `max_price=500`
- **Ratings**: `min_rating=8.0`
- **Cuisine Type**: `cuisines="arabic"` (substring match)
- **Ticket Price**: `ticket_price=50` (max ticket price for places)

Filters are applied at the database level using ChromaDB's `where` clause — no manual post-filtering needed.

---

## Environment Variables

### Frontend (`frontend/.env.local`)

| Variable | Description | Example |
|----------|-------------|---------|
| `NEXT_PUBLIC_BACKEND_URL` | Backend API URL | `http://localhost:5000` (dev) or your Modal URL (prod) |

### Backend (Modal Secrets)

| Variable | Description | Required |
|----------|-------------|----------|
| `GROQ_API_KEY` | Groq API key for LLaMA 3.1-8B | Yes |
| `TAVILY_API_KEY` | Tavily API key for real-time search | Yes |
| `ALLOWED_ORIGINS` | Comma-separated CORS origins | `http://localhost:3000,https://your-app.vercel.app` |

---

## Running the App

### Option 1: Local Development

#### Backend (Flask)

1. **Create virtual environment**
   ```bash
   python -m venv .venv
   ```

2. **Activate it**
   ```bash
   .venv\Scripts\activate    # Windows (PowerShell)
   # or
   source .venv/bin/activate # Mac/Linux
   ```

3. **Install dependencies**
   ```bash
   pip install -r requirements.txt
   ```

4. **Build the vector database**
   ```bash
   python src/build_vector_db.py
   ```
   This downloads the embedding model (~2.2GB) and builds the ChromaDB. Takes 5-15 minutes.

5. **Run the Flask API server**
   ```bash
   python api_server.py
   ```
   API runs on **http://localhost:5000**

#### Frontend (Next.js)

1. **Navigate to the frontend directory**
   ```bash
   cd frontend
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Create `.env.local`**
   ```bash
   cp .env.example .env.local
   ```
   Set `NEXT_PUBLIC_BACKEND_URL=http://localhost:5000`

4. **Run the development server**
   ```bash
   npm run dev
   ```
   Open **http://localhost:3000** in your browser.

---

### Option 2: Production Deploy (Modal Backend + Vercel Frontend)

#### Step 1: Deploy Backend to Modal

**Prerequisites**
```bash
pip install modal
modal setup
```

**Create Modal secrets**
```bash
modal secret create egypt-trip-planner-secrets GROQ_API_KEY TAVILY_API_KEY ALLOWED_ORIGINS
```
Set `ALLOWED_ORIGINS` to your Vercel domain: `https://your-app.vercel.app`

**Build ChromaDB on Modal (one-time, ~5-10 min)**
```bash
modal run modal_app.py::build_chroma_db
```
This runs on a T4 GPU and persists the database to a Modal Volume.

**Deploy the Flask API**
```bash
modal deploy modal_app.py
```

Modal will print a URL like:
```
https://<your-modal-account>--egypt-trip-planner-run.modal.run
```
This is your backend API URL.

#### Step 2: Deploy Frontend to Vercel

1. Push your code to GitHub
2. Connect your repo at [vercel.com](https://vercel.com)
3. Set the environment variable in Vercel:
   - `NEXT_PUBLIC_BACKEND_URL` = your Modal URL from Step 1
4. Deploy!

#### Development mode (watch for changes)
```bash
modal serve modal_app.py
```
Gives you a temporary URL that auto-updates when you edit files.

---

## Usage (Python API)

```python
from src.vector_search import search

# Arabic query - cheap hotel in Alexandria
results = search(
    query="فندق رخيص قريب من البحر",
    entity_type="hotel",
    city="alexandria",
    max_price=500,
    k=3,
)

# English query - luxury hotel in Sharm
results = search(
    query="luxury beachfront resort with spa",
    entity_type="hotel",
    city="sharm",
    min_rating=8.5,
    k=3,
)

# Filter by cuisine
results = search(
    query="مطعم عربي أصيل",
    entity_type="restaurant",
    city="cairo",
    cuisines="arabic",
    k=3,
)

# Places with cheap tickets
results = search(
    query="أماكن تاريخية وأثرية",
    entity_type="place",
    city="cairo",
    ticket_price=50,
    k=3,
)
```

### Search Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `query` | str | Search query (Arabic, English, or mixed) |
| `k` | int | Number of results (default: 5) |
| `entity_type` | str | "hotel", "restaurant", or "place" |
| `city` | str | City name (e.g., "alexandria", "cairo", "sharm") |
| `min_price` | float | Minimum price filter |
| `max_price` | float | Maximum price filter |
| `min_rating` | float | Minimum rating filter |
| `cuisines` | str | Cuisine substring match (e.g., "arabic", "italian") |
| `ticket_price` | float | Maximum ticket price for places |

## Data Schema

### Restaurants
- `id`, `city`, `Restaurant`, `Location`, `Cuisines`, `URL`
- `Total_Items`, `Prices_List`, `Min_Price`, `Max_Price`, `Avg_Price`

### Places
- `id`, `City`, `Title`, `Rating`, `Reviews`
- `Description`, `Tips`, `Address`, `Timings`, `Ticket Price`

### Hotels
- `id`, `city`, `Hotel Name`, `Price`, `Review Score (/10)`
- `Hotel URL`, `Description`, `Latitude`, `Longitude`, `Distance (km)`

## Notes

- **Model**: The multilingual embedding model supports 100+ languages including Arabic and English. First run downloads ~2.2GB model weights.
- **Missing Data**: Some entries have null values (e.g., 85% of places have no ticket price, 28% of hotels have no review score). Null metadata values are stored as `None` and excluded from filtering.
- **Price Outliers**: Extreme values exist in restaurant and hotel prices; use `max_price` filter to exclude them.
- **City Coverage**: Only Cairo and Alexandria appear across all three datasets.
- **ChromaDB**: Uses cosine similarity for better semantic matching. Database is stored persistently in `chroma_db/travel_chroma_db/`.

## License

This project is for educational/graduation purposes.
