from fastapi.testclient import TestClient
from app.main import app


def test_app_boots():
    client = TestClient(app, raise_server_exceptions=False)
    response = client.get("/nonexistent")
    assert response.status_code == 404
