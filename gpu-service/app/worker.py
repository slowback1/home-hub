import os
import subprocess
import threading
import time
from app.database import list_jobs, update_job_status, update_job_pid, update_job_error

POLL_INTERVAL = 5  # seconds between queue polls


def build_command(job: dict, jobs_dir: str, voice_samples_dir: str, start_chapter: int = 0) -> list[str]:
    job_dir = os.path.join(jobs_dir, job["id"])
    voice_sample_path = os.path.join(voice_samples_dir, f"{job['voice_sample_name']}.wav")
    cmd = [
        "python3", "-m", "app.vendor.convert",
        job_dir,
        "--output-dir", job_dir,
        "--voice-sample", voice_sample_path,
        "--silence", os.getenv("TTS_SILENCE", "1.5"),
        "--sample-rate", os.getenv("TTS_SAMPLE_RATE", "22050"),
        "--min-chars", os.getenv("TTS_MIN_CHARS", "500"),
        "--max-chunk-chars", os.getenv("TTS_MAX_CHARS", "500"),
        "--model", os.getenv("TTS_MODEL", "tts_models/multilingual/multi-dataset/xtts_v2"),
    ]
    if start_chapter > 0:
        cmd += ["--start-chapter", str(start_chapter)]
    return cmd


def detect_resume_chapter(job_dir: str) -> int:
    """Count existing chapter_NNN.wav files to determine where to resume."""
    if not os.path.isdir(job_dir):
        return 0
    return sum(
        1 for name in os.listdir(job_dir)
        if name.startswith("chapter_") and name.endswith(".wav")
    )


def reset_stale_jobs(db_path: str, jobs_dir: str) -> None:
    """On startup, re-queue any jobs that were interrupted mid-run."""
    for job in list_jobs(db_path):
        if job["status"] == "in_progress":
            update_job_pid(db_path, job["id"], None)
            update_job_status(db_path, job["id"], "queued")


def process_job(job: dict, db_path: str, jobs_dir: str, voice_samples_dir: str, start_chapter: int = 0) -> None:
    update_job_status(db_path, job["id"], "in_progress")
    cmd = build_command(job, jobs_dir, voice_samples_dir, start_chapter)
    proc = subprocess.Popen(cmd, stderr=subprocess.PIPE)
    update_job_pid(db_path, job["id"], proc.pid)
    exit_code = proc.wait()
    if exit_code == 0:
        update_job_status(db_path, job["id"], "completed")
    else:
        stderr_output = proc.stderr.read().decode("utf-8", errors="replace") if proc.stderr else ""
        update_job_error(db_path, job["id"], stderr_output)
        update_job_status(db_path, job["id"], "failed")


def run_worker_loop(db_path: str, jobs_dir: str, voice_samples_dir: str, stop_event: threading.Event) -> None:
    while not stop_event.is_set():
        jobs = list_jobs(db_path)
        queued = [j for j in jobs if j["status"] == "queued"]
        if queued:
            job = queued[0]
            job_dir = os.path.join(jobs_dir, job["id"])
            start_chapter = detect_resume_chapter(job_dir)
            process_job(job, db_path, jobs_dir, voice_samples_dir, start_chapter)
        else:
            stop_event.wait(timeout=POLL_INTERVAL)


def start_worker(db_path: str, jobs_dir: str, voice_samples_dir: str) -> tuple[threading.Thread, threading.Event]:
    stop_event = threading.Event()
    thread = threading.Thread(
        target=run_worker_loop,
        args=(db_path, jobs_dir, voice_samples_dir, stop_event),
        daemon=True,
    )
    thread.start()
    return thread, stop_event
