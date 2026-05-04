import pytest
import os
from fastapi.testclient import TestClient
from app.database import init_db


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
    # seed a voice sample
    with open(os.path.join(voices_dir, "narrator.wav"), "wb") as f:
        f.write(b"RIFF" + b"\x00" * 40)


@pytest.fixture
def client():
    from app.main import app
    return TestClient(app)


@pytest.fixture
def auth():
    return {"Authorization": "Bearer test-secret"}


@pytest.fixture
def epub_file():
    return ("book.epub", b"PK\x03\x04" + b"\x00" * 100, "application/epub+zip")


def test_submit_job_returns_201_with_job_id(client, auth, epub_file):
    response = client.post(
        "/jobs",
        headers=auth,
        files={"epub_file": epub_file},
        data={"voice_sample_name": "narrator"},
    )
    assert response.status_code == 201
    body = response.json()
    assert "id" in body
    assert body["status"] == "queued"
    assert body["epub_filename"] == "book.epub"
    assert body["voice_sample_name"] == "narrator"


def test_submit_job_saves_epub_to_disk(client, auth, epub_file, tmp_path):
    response = client.post(
        "/jobs",
        headers=auth,
        files={"epub_file": epub_file},
        data={"voice_sample_name": "narrator"},
    )
    job_id = response.json()["id"]
    expected_path = os.path.join(str(tmp_path / "jobs"), job_id, "input.epub")
    assert os.path.exists(expected_path)


def test_submit_job_with_missing_voice_sample_returns_404(client, auth):
    response = client.post(
        "/jobs",
        headers=auth,
        files={"epub_file": ("book.epub", b"PK" + b"\x00" * 50, "application/epub+zip")},
        data={"voice_sample_name": "nonexistent"},
    )
    assert response.status_code == 404


def test_submit_non_epub_returns_400(client, auth):
    response = client.post(
        "/jobs",
        headers=auth,
        files={"epub_file": ("book.pdf", b"%PDF", "application/pdf")},
        data={"voice_sample_name": "narrator"},
    )
    assert response.status_code == 400


def test_list_jobs_returns_all_jobs(client, auth, epub_file):
    client.post("/jobs", headers=auth, files={"epub_file": epub_file}, data={"voice_sample_name": "narrator"})
    client.post("/jobs", headers=auth, files={"epub_file": epub_file}, data={"voice_sample_name": "narrator"})
    response = client.get("/jobs", headers=auth)
    assert response.status_code == 200
    assert len(response.json()) == 2


def test_get_job_by_id(client, auth, epub_file):
    job_id = client.post(
        "/jobs", headers=auth, files={"epub_file": epub_file}, data={"voice_sample_name": "narrator"}
    ).json()["id"]
    response = client.get(f"/jobs/{job_id}", headers=auth)
    assert response.status_code == 200
    assert response.json()["id"] == job_id


def test_get_nonexistent_job_returns_404(client, auth):
    assert client.get("/jobs/no-such-id", headers=auth).status_code == 404


def test_job_endpoints_require_auth(client, epub_file):
    assert client.get("/jobs").status_code == 401
    assert client.post("/jobs", files={"epub_file": epub_file}, data={"voice_sample_name": "narrator"}).status_code == 401
    assert client.get("/jobs/some-id").status_code == 401
