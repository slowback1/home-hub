import os
from fastapi import APIRouter, Depends, HTTPException, UploadFile, Form, status
from app.auth import require_api_key
from app.database import create_job, get_job, list_jobs

router = APIRouter(prefix="/jobs", tags=["jobs"])


def get_db_path() -> str:
    return os.getenv("JOBS_DIR", "/data/jobs") + "/jobs.db"


def get_voice_samples_dir() -> str:
    return os.getenv("VOICE_SAMPLES_DIR", "/data/voice-samples")


@router.get("", dependencies=[Depends(require_api_key)])
def list_jobs_route() -> list[dict]:
    return list_jobs(get_db_path())


@router.post("", status_code=status.HTTP_201_CREATED, dependencies=[Depends(require_api_key)])
async def submit_job(
    epub_file: UploadFile,
    voice_sample_name: str = Form(...),
) -> dict:
    if not epub_file.filename or not epub_file.filename.lower().endswith(".epub"):
        raise HTTPException(status_code=400, detail="Only .epub files are accepted")

    voice_path = os.path.join(get_voice_samples_dir(), f"{voice_sample_name}.wav")
    if not os.path.exists(voice_path):
        raise HTTPException(status_code=404, detail=f"Voice sample '{voice_sample_name}' not found")

    job = create_job(get_db_path(), epub_filename=epub_file.filename, voice_sample_name=voice_sample_name)

    job_dir = os.path.join(os.getenv("JOBS_DIR", "/data/jobs"), job["id"])
    os.makedirs(job_dir, exist_ok=True)
    content = await epub_file.read()
    with open(os.path.join(job_dir, "input.epub"), "wb") as f:
        f.write(content)

    return job


@router.get("/{job_id}", dependencies=[Depends(require_api_key)])
def get_job_route(job_id: str) -> dict:
    job = get_job(get_db_path(), job_id)
    if job is None:
        raise HTTPException(status_code=404, detail="Job not found")
    return job
