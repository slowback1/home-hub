import pytest
import os
import shutil
from fastapi.testclient import TestClient
from app.database import init_db, create_job, update_job_status, update_job_pid


@pytest.fixture(autouse=True)
def configure_env(monkeypatch, tmp_path):
    monkeypatch.setenv("API_KEY", "test-secret")
    jobs_dir = str(tmp_path / "jobs")
    voices_dir = str(tmp_path / "voice-samples")
    monkeypatch.setenv("JOBS_DIR", jobs_dir)
    monkeypatch.setenv("VOICE_SAMPLES_DIR", voices_dir)
    os.makedirs(jobs_dir, exist_ok=True)
    os.makedirs(voices_dir, exist_ok=True)
    with open(os.path.join(voices_dir, "narrator.wav"), "wb") as f:
        f.write(b"RIFF")
    init_db(jobs_dir + "/jobs.db")


@pytest.fixture
def client():
    from app.main import app
    return TestClient(app)


@pytest.fixture
def auth():
    return {"Authorization": "Bearer test-secret"}


def _make_queued_job(tmp_path):
    jobs_dir = str(tmp_path / "jobs")
    db_path = jobs_dir + "/jobs.db"
    job = create_job(db_path, epub_filename="book.epub", voice_sample_name="narrator")
    job_dir = os.path.join(jobs_dir, job["id"])
    os.makedirs(job_dir)
    with open(os.path.join(job_dir, "input.epub"), "wb") as f:
        f.write(b"PK")
    return job, db_path, job_dir


def test_cancel_queued_job_returns_204(client, auth, tmp_path):
    job, _, _ = _make_queued_job(tmp_path)
    response = client.delete(f"/jobs/{job['id']}", headers=auth)
    assert response.status_code == 204


def test_cancel_queued_job_sets_status_cancelled(client, auth, tmp_path):
    job, db_path, _ = _make_queued_job(tmp_path)
    client.delete(f"/jobs/{job['id']}", headers=auth)
    from app.database import get_job
    assert get_job(db_path, job["id"])["status"] == "cancelled"


def test_cancel_in_progress_job_sends_sigterm(client, auth, tmp_path, monkeypatch):
    job, db_path, job_dir = _make_queued_job(tmp_path)
    update_job_status(db_path, job["id"], "in_progress")
    update_job_pid(db_path, job["id"], 99999)

    killed_pids = []

    def fake_kill(pid, sig):
        killed_pids.append(pid)

    import signal
    monkeypatch.setattr("os.kill", fake_kill)
    client.delete(f"/jobs/{job['id']}", headers=auth)
    assert 99999 in killed_pids


def test_cancel_in_progress_job_handles_dead_process(client, auth, tmp_path, monkeypatch):
    job, db_path, _ = _make_queued_job(tmp_path)
    update_job_status(db_path, job["id"], "in_progress")
    update_job_pid(db_path, job["id"], 99999)

    def fake_kill(pid, sig):
        raise ProcessLookupError(f"No process {pid}")

    monkeypatch.setattr("os.kill", fake_kill)
    response = client.delete(f"/jobs/{job['id']}", headers=auth)
    assert response.status_code == 204  # still succeeds


def test_cancel_unknown_job_returns_404(client, auth):
    assert client.delete("/jobs/no-such-id", headers=auth).status_code == 404


def test_cancel_completed_job_returns_409(client, auth, tmp_path):
    job, db_path, _ = _make_queued_job(tmp_path)
    update_job_status(db_path, job["id"], "completed")
    assert client.delete(f"/jobs/{job['id']}", headers=auth).status_code == 409


def test_cancel_already_cancelled_job_returns_409(client, auth, tmp_path):
    job, db_path, _ = _make_queued_job(tmp_path)
    update_job_status(db_path, job["id"], "cancelled")
    assert client.delete(f"/jobs/{job['id']}", headers=auth).status_code == 409


def test_cancel_cleans_up_job_directory(client, auth, tmp_path, monkeypatch):
    job, db_path, job_dir = _make_queued_job(tmp_path)
    # create some partial chapter WAVs
    open(os.path.join(job_dir, "chapter_001.wav"), "w").close()
    open(os.path.join(job_dir, "chapter_002.wav"), "w").close()

    client.delete(f"/jobs/{job['id']}", headers=auth)
    assert not os.path.exists(job_dir)


def test_cancel_requires_auth(client, tmp_path):
    job, _, _ = _make_queued_job(tmp_path)
    assert client.delete(f"/jobs/{job['id']}").status_code == 401
