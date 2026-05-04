import pytest
import os
from app.database import init_db, create_job, get_job, update_job_status, update_job_pid
from app.worker import build_command, detect_resume_chapter, reset_stale_jobs


@pytest.fixture
def db_path(tmp_path):
    path = str(tmp_path / "jobs.db")
    init_db(path)
    return path


@pytest.fixture
def dirs(tmp_path):
    jobs_dir = str(tmp_path / "jobs")
    voices_dir = str(tmp_path / "voice-samples")
    os.makedirs(jobs_dir)
    os.makedirs(voices_dir)
    with open(os.path.join(voices_dir, "narrator.wav"), "wb") as f:
        f.write(b"RIFF")
    return jobs_dir, voices_dir


def make_job_dir_with_wavs(jobs_dir, job_id, n_wavs):
    job_dir = os.path.join(jobs_dir, job_id)
    os.makedirs(job_dir, exist_ok=True)
    for i in range(1, n_wavs + 1):
        open(os.path.join(job_dir, f"chapter_{i:03d}.wav"), "w").close()
    return job_dir


def test_detect_resume_chapter_empty_dir_returns_zero(tmp_path):
    job_dir = str(tmp_path / "job")
    os.makedirs(job_dir)
    assert detect_resume_chapter(job_dir) == 0


def test_detect_resume_chapter_counts_existing_wavs(tmp_path):
    job_dir = str(tmp_path / "job")
    os.makedirs(job_dir)
    for i in range(1, 6):
        open(os.path.join(job_dir, f"chapter_{i:03d}.wav"), "w").close()
    assert detect_resume_chapter(job_dir) == 5


def test_detect_resume_chapter_ignores_non_chapter_files(tmp_path):
    job_dir = str(tmp_path / "job")
    os.makedirs(job_dir)
    open(os.path.join(job_dir, "chapter_001.wav"), "w").close()
    open(os.path.join(job_dir, "input.epub"), "w").close()
    open(os.path.join(job_dir, "_temp_chunk.wav"), "w").close()
    assert detect_resume_chapter(job_dir) == 1


def test_reset_stale_jobs_requeues_in_progress_jobs(db_path, dirs):
    jobs_dir, voices_dir = dirs
    job = create_job(db_path, epub_filename="book.epub", voice_sample_name="narrator")
    update_job_status(db_path, job["id"], "in_progress")
    update_job_pid(db_path, job["id"], 99999)
    make_job_dir_with_wavs(jobs_dir, job["id"], 3)

    reset_stale_jobs(db_path, jobs_dir)

    updated = get_job(db_path, job["id"])
    assert updated["status"] == "queued"
    assert updated["pid"] is None


def test_reset_stale_jobs_leaves_completed_jobs_alone(db_path, dirs):
    jobs_dir, _ = dirs
    job = create_job(db_path, epub_filename="book.epub", voice_sample_name="narrator")
    update_job_status(db_path, job["id"], "completed")
    reset_stale_jobs(db_path, jobs_dir)
    assert get_job(db_path, job["id"])["status"] == "completed"


def test_reset_stale_jobs_leaves_queued_jobs_alone(db_path, dirs):
    jobs_dir, _ = dirs
    job = create_job(db_path, epub_filename="book.epub", voice_sample_name="narrator")
    reset_stale_jobs(db_path, jobs_dir)
    assert get_job(db_path, job["id"])["status"] == "queued"


def test_build_command_includes_start_chapter_when_nonzero(dirs):
    jobs_dir, voices_dir = dirs
    job = {"id": "abc123", "voice_sample_name": "narrator"}
    cmd = build_command(job, jobs_dir, voices_dir, start_chapter=5)
    cmd_str = " ".join(cmd)
    assert "--start-chapter" in cmd_str
    assert "5" in cmd_str


def test_build_command_omits_start_chapter_when_zero(dirs):
    jobs_dir, voices_dir = dirs
    job = {"id": "abc123", "voice_sample_name": "narrator"}
    cmd = build_command(job, jobs_dir, voices_dir, start_chapter=0)
    assert "--start-chapter" not in " ".join(cmd)
