import pytest
import os
from fastapi.testclient import TestClient
from app.database import init_db


@pytest.fixture(autouse=True)
def configure_env(monkeypatch, tmp_path):
    monkeypatch.setenv("API_KEY", "test-secret")
    monkeypatch.setenv("JOBS_DIR", str(tmp_path / "jobs"))
    monkeypatch.setenv("VOICE_SAMPLES_DIR", str(tmp_path / "voice-samples"))
    os.makedirs(str(tmp_path / "jobs"), exist_ok=True)
    os.makedirs(str(tmp_path / "voice-samples"), exist_ok=True)
    init_db(str(tmp_path / "jobs" / "jobs.db"))


@pytest.fixture
def client():
    from app.main import app
    return TestClient(app)


@pytest.fixture
def auth_headers():
    return {"Authorization": "Bearer test-secret"}


def test_list_voice_samples_empty(client, auth_headers):
    response = client.get("/voice-samples", headers=auth_headers)
    assert response.status_code == 200
    assert response.json() == []


def test_upload_voice_sample(client, auth_headers):
    wav_content = b"RIFF" + b"\x00" * 40  # minimal WAV-like bytes
    response = client.post(
        "/voice-samples",
        headers=auth_headers,
        files={"file": ("narrator.wav", wav_content, "audio/wav")},
    )
    assert response.status_code == 201
    assert response.json()["name"] == "narrator"


def test_list_voice_samples_after_upload(client, auth_headers):
    wav_content = b"RIFF" + b"\x00" * 40
    client.post(
        "/voice-samples",
        headers=auth_headers,
        files={"file": ("narrator.wav", wav_content, "audio/wav")},
    )
    response = client.get("/voice-samples", headers=auth_headers)
    assert response.json() == ["narrator"]


def test_upload_non_wav_returns_400(client, auth_headers):
    response = client.post(
        "/voice-samples",
        headers=auth_headers,
        files={"file": ("narrator.mp3", b"ID3", "audio/mpeg")},
    )
    assert response.status_code == 400


def test_delete_voice_sample(client, auth_headers):
    wav_content = b"RIFF" + b"\x00" * 40
    client.post(
        "/voice-samples",
        headers=auth_headers,
        files={"file": ("narrator.wav", wav_content, "audio/wav")},
    )
    response = client.delete("/voice-samples/narrator", headers=auth_headers)
    assert response.status_code == 204
    assert client.get("/voice-samples", headers=auth_headers).json() == []


def test_delete_nonexistent_voice_sample_returns_404(client, auth_headers):
    response = client.delete("/voice-samples/ghost", headers=auth_headers)
    assert response.status_code == 404


def test_voice_sample_endpoints_require_auth(client):
    assert client.get("/voice-samples").status_code == 401
    wav_bytes = b"RIFF" + b"\x00" * 40
    assert client.post("/voice-samples", files={"file": ("x.wav", wav_bytes, "audio/wav")}).status_code == 401
    assert client.delete("/voice-samples/narrator").status_code == 401
