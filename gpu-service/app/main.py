import os
from contextlib import asynccontextmanager
from fastapi import FastAPI
from app.database import init_db
from app.routers import voice_samples
from app.routers import jobs


def get_db_path() -> str:
    return os.getenv("JOBS_DIR", "/data/jobs") + "/jobs.db"


@asynccontextmanager
async def lifespan(app: FastAPI):
    db_path = get_db_path()
    os.makedirs(os.path.dirname(db_path), exist_ok=True)
    init_db(db_path)
    yield


app = FastAPI(title="Epub-to-Audiobook GPU Service", lifespan=lifespan)

app.include_router(voice_samples.router)
app.include_router(jobs.router)


@app.get("/health")
def health():
    return {"status": "ok"}
