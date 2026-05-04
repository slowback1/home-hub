import os
from contextlib import asynccontextmanager
from fastapi import FastAPI, Depends
from app.database import init_db, list_jobs
from app.auth import require_api_key


def get_db_path() -> str:
    return os.getenv("JOBS_DIR", "/data/jobs") + "/jobs.db"


@asynccontextmanager
async def lifespan(app: FastAPI):
    db_path = get_db_path()
    os.makedirs(os.path.dirname(db_path), exist_ok=True)
    init_db(db_path)
    yield


app = FastAPI(title="Epub-to-Audiobook GPU Service", lifespan=lifespan)


@app.get("/health")
def health():
    return {"status": "ok"}


@app.get("/jobs", dependencies=[Depends(require_api_key)])
def list_jobs_route():
    return list_jobs(get_db_path())
