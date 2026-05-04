import pytest
import os
import threading
import time
from unittest.mock import patch, MagicMock
from app.database import init_db, create_job, get_job, list_jobs, update_job_status
from app.worker import build_command, process_job, start_worker


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
        f.write(b"RIFF" + b"\x00" * 40)
    return jobs_dir, voices_dir


def make_job(db_path, dirs):
    jobs_dir, voices_dir = dirs
    job = create_job(db_path, epub_filename="book.epub", voice_sample_name="narrator")
    job_dir = os.path.join(jobs_dir, job["id"])
    os.makedirs(job_dir)
    with open(os.path.join(job_dir, "input.epub"), "wb") as f:
        f.write(b"PK" + b"\x00" * 50)
    return job


def test_build_command_includes_required_args(dirs, monkeypatch):
    jobs_dir, voices_dir = dirs
    monkeypatch.setenv("TTS_SILENCE", "2.0")
    monkeypatch.setenv("TTS_SAMPLE_RATE", "22050")
    monkeypatch.setenv("TTS_MIN_CHARS", "300")
    monkeypatch.setenv("TTS_MAX_CHARS", "600")
    monkeypatch.setenv("TTS_MODEL", "tts_models/multilingual/multi-dataset/xtts_v2")
    job = {"id": "abc123", "voice_sample_name": "narrator"}
    cmd = build_command(job, jobs_dir, voices_dir)
    cmd_str = " ".join(cmd)
    assert "--voice-sample" in cmd_str
    assert "narrator.wav" in cmd_str
    assert "--silence" in cmd_str
    assert "2.0" in cmd_str
    assert "--output-dir" in cmd_str
    assert "abc123" in cmd_str


def test_process_job_marks_in_progress_and_stores_pid(db_path, dirs):
    job = make_job(db_path, dirs)
    jobs_dir, voices_dir = dirs

    mock_proc = MagicMock()
    mock_proc.pid = 99999
    mock_proc.wait.return_value = 0
    mock_proc.stderr = None

    with patch("app.worker.subprocess.Popen", return_value=mock_proc):
        process_job(job, db_path, jobs_dir, voices_dir)

    updated = get_job(db_path, job["id"])
    assert updated["pid"] == 99999


def test_process_job_marks_completed_on_success(db_path, dirs):
    job = make_job(db_path, dirs)
    jobs_dir, voices_dir = dirs

    mock_proc = MagicMock()
    mock_proc.pid = 12345
    mock_proc.wait.return_value = 0
    mock_proc.stderr = None

    with patch("app.worker.subprocess.Popen", return_value=mock_proc):
        process_job(job, db_path, jobs_dir, voices_dir)

    assert get_job(db_path, job["id"])["status"] == "completed"


def test_process_job_marks_failed_on_nonzero_exit(db_path, dirs):
    job = make_job(db_path, dirs)
    jobs_dir, voices_dir = dirs

    mock_proc = MagicMock()
    mock_proc.pid = 12345
    mock_proc.wait.return_value = 1
    mock_proc.stderr = MagicMock()
    mock_proc.stderr.read.return_value = b"TTS model failed"

    with patch("app.worker.subprocess.Popen", return_value=mock_proc):
        process_job(job, db_path, jobs_dir, voices_dir)

    updated = get_job(db_path, job["id"])
    assert updated["status"] == "failed"
    assert "TTS model failed" in updated["error_message"]


def test_worker_processes_jobs_in_order(db_path, dirs):
    jobs_dir, voices_dir = dirs
    job1 = make_job(db_path, dirs)
    job2 = make_job(db_path, dirs)
    processed = []

    def fake_popen(cmd, **kwargs):
        # identify which job by checking the command for the job dir path
        job_id = job1["id"] if job1["id"] in " ".join(cmd) else job2["id"]
        processed.append(job_id)
        m = MagicMock()
        m.pid = 1
        m.wait.return_value = 0
        m.stderr = None
        return m

    with patch("app.worker.subprocess.Popen", side_effect=fake_popen):
        stop = threading.Event()

        def run():
            from app.worker import run_worker_loop
            run_worker_loop(db_path, jobs_dir, voices_dir, stop)

        t = threading.Thread(target=run, daemon=True)
        t.start()
        # wait until both jobs are processed
        for _ in range(50):
            if len(processed) >= 2:
                break
            time.sleep(0.1)
        stop.set()
        t.join(timeout=2)

    assert processed == [job1["id"], job2["id"]]


def test_start_worker_returns_running_thread(db_path, dirs, monkeypatch):
    jobs_dir, voices_dir = dirs
    monkeypatch.setenv("JOBS_DIR", jobs_dir)
    monkeypatch.setenv("VOICE_SAMPLES_DIR", voices_dir)

    with patch("app.worker.subprocess.Popen"):
        thread, stop_event = start_worker(db_path, jobs_dir, voices_dir)
        assert thread.is_alive()
        stop_event.set()
        thread.join(timeout=2)
