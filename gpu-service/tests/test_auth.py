import pytest
from fastapi.testclient import TestClient
from app.database import init_db


@pytest.fixture(autouse=True)
def configure_env(monkeypatch, tmp_path):
    monkeypatch.setenv("API_KEY", "test-secret")
    db_dir = str(tmp_path)
    monkeypatch.setenv("JOBS_DIR", db_dir)
    init_db(db_dir + "/jobs.db")


@pytest.fixture
def client():
    from app.main import app
    return TestClient(app)


def test_health_returns_200_without_auth(client):
    response = client.get("/health")
    assert response.status_code == 200


def test_protected_route_returns_401_without_auth(client):
    response = client.get("/jobs")
    assert response.status_code == 401


def test_protected_route_returns_401_with_wrong_key(client):
    response = client.get("/jobs", headers={"Authorization": "Bearer wrong-key"})
    assert response.status_code == 401


def test_protected_route_returns_200_with_correct_key(client):
    response = client.get("/jobs", headers={"Authorization": "Bearer test-secret"})
    assert response.status_code == 200
