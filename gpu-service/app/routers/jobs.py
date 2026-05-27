import os
import signal
import shutil
from fastapi import APIRouter, Depends, HTTPException, UploadFile, Form, status
from fastapi.responses import FileResponse
from app.auth import require_api_key
from app.database import create_job, delete_job, get_job, list_jobs, update_job_status

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


_TERMINAL_STATUSES = {"completed", "failed", "cancelled"}


@router.delete("/{job_id}", status_code=status.HTTP_204_NO_CONTENT, dependencies=[Depends(require_api_key)])
def cancel_or_delete_job(job_id: str) -> None:
    db_path = get_db_path()
    job = get_job(db_path, job_id)
    if job is None:
        raise HTTPException(status_code=404, detail="Job not found")

    job_dir = os.path.join(os.getenv("JOBS_DIR", "/data/jobs"), job_id)

    if job["status"] in _TERMINAL_STATUSES:
        delete_job(db_path, job_id)
        if os.path.isdir(job_dir):
            shutil.rmtree(job_dir)
        return

    if job["status"] == "in_progress" and job["pid"]:
        try:
            os.kill(job["pid"], signal.SIGTERM)
        except ProcessLookupError:
            pass  # process already exited

    update_job_status(db_path, job_id, "cancelled")

    if os.path.isdir(job_dir):
        shutil.rmtree(job_dir)


def _find_m4b(job_dir: str) -> str | None:
    """Return the path to the M4B file in job_dir, or None if not found."""
    for name in os.listdir(job_dir) if os.path.isdir(job_dir) else []:
        if name.endswith(".m4b"):
            return os.path.join(job_dir, name)
    return None


@router.get("/{job_id}/file", dependencies=[Depends(require_api_key)])
def download_file(job_id: str) -> FileResponse:
    db_path = get_db_path()
    job = get_job(db_path, job_id)
    if job is None:
        raise HTTPException(status_code=404, detail="Job not found")
    if job["status"] != "completed":
        raise HTTPException(status_code=409, detail=f"Job is not completed (status: {job['status']})")
    job_dir = os.path.join(os.getenv("JOBS_DIR", "/data/jobs"), job_id)
    m4b_path = _find_m4b(job_dir)
    if m4b_path is None:
        raise HTTPException(status_code=410, detail="File has already been deleted")
    filename = os.path.basename(m4b_path)
    return FileResponse(
        path=m4b_path,
        media_type="audio/mp4",
        filename=filename,
    )


@router.delete("/{job_id}/file", status_code=status.HTTP_204_NO_CONTENT, dependencies=[Depends(require_api_key)])
def delete_file(job_id: str) -> None:
    db_path = get_db_path()
    job = get_job(db_path, job_id)
    if job is None:
        raise HTTPException(status_code=404, detail="Job not found")
    if job["status"] != "completed":
        raise HTTPException(status_code=409, detail=f"Job is not completed (status: {job['status']})")
    job_dir = os.path.join(os.getenv("JOBS_DIR", "/data/jobs"), job_id)
    m4b_path = _find_m4b(job_dir)
    if m4b_path is None:
        raise HTTPException(status_code=404, detail="File not found or already deleted")
    os.remove(m4b_path)
