import os
from contextlib import asynccontextmanager
from fastapi import FastAPI
from app.database import init_db

DB_PATH = os.getenv("JOBS_DIR", "/data/jobs") + "/jobs.db"


@asynccontextmanager
async def lifespan(app: FastAPI):
    os.makedirs(os.path.dirname(DB_PATH), exist_ok=True)
    init_db(DB_PATH)
    yield


app = FastAPI(title="Epub-to-Audiobook GPU Service", lifespan=lifespan)
