import os
import chromadb
import pandas as pd
from sentence_transformers import SentenceTransformer

print("Initializing DB Path...")
CHROMA_PATH = os.path.join(os.path.dirname(__file__), "chroma_db", "travel_chroma_db")
client = chromadb.PersistentClient(path=CHROMA_PATH)

print("Getting or creating collection 'travel'...")
collection = client.get_or_create_collection(name="travel")

print("Loading Embedding Model...")
EMBEDDING_MODEL = "sentence-transformers/paraphrase-multilingual-MiniLM-L12-v2"
model = SentenceTransformer(EMBEDDING_MODEL, trust_remote_code=True)

print("Reading Dataset...")
DATA_PATH = os.path.join(os.path.dirname(__file__), "data", "places.csv")
df = pd.read_csv(DATA_PATH)

documents = []
metadatas = []
ids = []

for idx, row in df.iterrows():
    # Use id from dataset if available, otherwise index
    doc_id = f"place_{row.get('id', idx)}"
    text_parts = [f"Place: {row.get('Title', '')}", f"City: {row.get('City', '')}"]
    
    if 'Description' in row and pd.notna(row['Description']):
         text_parts.append(f"Description: {row['Description']}")
    if 'Ticket Price' in row and pd.notna(row['Ticket Price']):
         text_parts.append(f"Ticket Price: {row['Ticket Price']} EGP")
         
    doc_text = "\n".join(text_parts)
    
    metadata = {
        "type": "place",
        "name": str(row.get('Title', '')),
        "city": str(row.get('City', '')).lower().strip()
    }
    
    # Optional ticket_price to avoid errors when filtering by it
    if 'Ticket Price' in row and pd.notna(row['Ticket Price']):
        try:
            metadata["ticket_price"] = float(row['Ticket Price'])
        except ValueError:
            pass
            
    documents.append(doc_text)
    metadatas.append(metadata)
    ids.append(doc_id)

print(f"Generating embeddings for {len(documents)} documents...")
# Batch the insertions if it's large, but since it's just one dataset, we'll encode at once
batch_size = 64
for i in range(0, len(documents), batch_size):
    batch_docs = documents[i:i+batch_size]
    batch_embeddings = model.encode(batch_docs, show_progress_bar=False).tolist()
    batch_meta = metadatas[i:i+batch_size]
    batch_ids = ids[i:i+batch_size]
    
    collection.add(
        documents=batch_docs,
        embeddings=batch_embeddings,
        metadatas=batch_meta,
        ids=batch_ids
    )
    print(f"Inserted batch {i//batch_size + 1}")

print(f"Done! Successfully inserted {len(documents)} records into collection 'travel'.")
