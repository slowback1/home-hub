import pytest
import os
from fastapi.testclient import TestClient
from app.database import init_db, create_job, update_job_status


@pytest.fixture(autouse=True)
def configure_env(monkeypatch, tmp_path):
    monkeypatch.setenv("API_KEY", "test-secret")
    jobs_dir = str(tmp_path / "jobs")
    voices_dir = str(tmp_path / "voice-samples")
    monkeypatch.setenv("JOBS_DIR", jobs_dir)
    monkeypatch.setenv("VOICE_SAMPLES_DIR", voices_dir)
    os.makedirs(jobs_dir, exist_ok=True)
    os.makedirs(voices_dir, exist_ok=True)
    init_db(jobs_dir + "/jobs.db")


@pytest.fixture
def client():
    from app.main import app
    return TestClient(app)


@pytest.fixture
def auth():
    return {"Authorization": "Bearer test-secret"}


def _make_completed_job(tmp_path, with_file=True):
    jobs_dir = str(tmp_path / "jobs")
    db_path = jobs_dir + "/jobs.db"
    job = create_job(db_path, epub_filename="book.epub", voice_sample_name="narrator")
    update_job_status(db_path, job["id"], "completed")
    job_dir = os.path.join(jobs_dir, job["id"])
    os.makedirs(job_dir, exist_ok=True)
    m4b_path = None
    if with_file:
        m4b_path = os.path.join(job_dir, "book.m4b")
        with open(m4b_path, "wb") as f:
            f.write(b"fake-m4b-content")
    return job, db_path, job_dir, m4b_path


def test_download_completed_file_returns_200(client, auth, tmp_path):
    job, _, _, _ = _make_completed_job(tmp_path)
    response = client.get(f"/jobs/{job['id']}/file", headers=auth)
    assert response.status_code == 200


def test_download_returns_correct_content_type(client, auth, tmp_path):
    job, _, _, _ = _make_completed_job(tmp_path)
    response = client.get(f"/jobs/{job['id']}/file", headers=auth)
    assert "audio/mp4" in response.headers["content-type"]


def test_download_returns_correct_filename_header(client, auth, tmp_path):
    job, _, _, _ = _make_completed_job(tmp_path)
    response = client.get(f"/jobs/{job['id']}/file", headers=auth)
    assert "book.m4b" in response.headers.get("content-disposition", "")


def test_download_streams_file_content(client, auth, tmp_path):
    job, _, _, _ = _make_completed_job(tmp_path)
    response = client.get(f"/jobs/{job['id']}/file", headers=auth)
    assert response.content == b"fake-m4b-content"


def test_download_nonexistent_job_returns_404(client, auth):
    response = client.get("/jobs/no-such-id/file", headers=auth)
    assert response.status_code == 404


def test_download_incomplete_job_returns_409(client, auth, tmp_path):
    jobs_dir = str(tmp_path / "jobs")
    db_path = jobs_dir + "/jobs.db"
    job = create_job(db_path, epub_filename="book.epub", voice_sample_name="narrator")
    # status is "queued" by default
    response = client.get(f"/jobs/{job['id']}/file", headers=auth)
    assert response.status_code == 409


def test_download_after_file_deleted_returns_410(client, auth, tmp_path):
    job, _, _, m4b_path = _make_completed_job(tmp_path)
    os.remove(m4b_path)
    response = client.get(f"/jobs/{job['id']}/file", headers=auth)
    assert response.status_code == 410


def test_delete_file_returns_204(client, auth, tmp_path):
    job, _, _, _ = _make_completed_job(tmp_path)
    response = client.delete(f"/jobs/{job['id']}/file", headers=auth)
    assert response.status_code == 204


def test_delete_file_removes_m4b_from_disk(client, auth, tmp_path):
    job, _, _, m4b_path = _make_completed_job(tmp_path)
    client.delete(f"/jobs/{job['id']}/file", headers=auth)
    assert not os.path.exists(m4b_path)


def test_delete_file_nonexistent_job_returns_404(client, auth):
    assert client.delete("/jobs/no-such-id/file", headers=auth).status_code == 404


def test_delete_file_already_gone_returns_404(client, auth, tmp_path):
    job, _, _, m4b_path = _make_completed_job(tmp_path)
    os.remove(m4b_path)
    assert client.delete(f"/jobs/{job['id']}/file", headers=auth).status_code == 404


def test_delete_file_incomplete_job_returns_409(client, auth, tmp_path):
    jobs_dir = str(tmp_path / "jobs")
    db_path = jobs_dir + "/jobs.db"
    job = create_job(db_path, epub_filename="book.epub", voice_sample_name="narrator")
    assert client.delete(f"/jobs/{job['id']}/file", headers=auth).status_code == 409


def test_file_endpoints_require_auth(client, tmp_path):
    job, _, _, _ = _make_completed_job(tmp_path)
    assert client.get(f"/jobs/{job['id']}/file").status_code == 401
    assert client.delete(f"/jobs/{job['id']}/file").status_code == 401
