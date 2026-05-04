import pytest
import tempfile
import os
from app.database import init_db, create_job, get_job, list_jobs, update_job_status, update_job_pid, update_job_error


@pytest.fixture
def db_path(tmp_path):
    path = str(tmp_path / "test.db")
    init_db(path)
    return path


def test_create_job_returns_job_with_id(db_path):
    job = create_job(db_path, epub_filename="book.epub", voice_sample_name="narrator")
    assert job["id"] is not None
    assert job["epub_filename"] == "book.epub"
    assert job["voice_sample_name"] == "narrator"
    assert job["status"] == "queued"


def test_get_job_returns_created_job(db_path):
    job = create_job(db_path, epub_filename="book.epub", voice_sample_name="narrator")
    fetched = get_job(db_path, job["id"])
    assert fetched["id"] == job["id"]
    assert fetched["epub_filename"] == "book.epub"


def test_get_job_returns_none_for_unknown_id(db_path):
    assert get_job(db_path, "nonexistent-id") is None


def test_list_jobs_returns_all_jobs(db_path):
    create_job(db_path, epub_filename="a.epub", voice_sample_name="v1")
    create_job(db_path, epub_filename="b.epub", voice_sample_name="v2")
    jobs = list_jobs(db_path)
    assert len(jobs) == 2


def test_list_jobs_empty(db_path):
    assert list_jobs(db_path) == []


def test_update_job_status(db_path):
    job = create_job(db_path, epub_filename="book.epub", voice_sample_name="narrator")
    update_job_status(db_path, job["id"], "in_progress")
    updated = get_job(db_path, job["id"])
    assert updated["status"] == "in_progress"


def test_update_job_status_invalid_raises(db_path):
    job = create_job(db_path, epub_filename="book.epub", voice_sample_name="narrator")
    with pytest.raises(ValueError):
        update_job_status(db_path, job["id"], "invalid_status")


def test_update_job_pid(db_path):
    job = create_job(db_path, epub_filename="book.epub", voice_sample_name="narrator")
    update_job_pid(db_path, job["id"], 12345)
    updated = get_job(db_path, job["id"])
    assert updated["pid"] == 12345


def test_update_job_error(db_path):
    job = create_job(db_path, epub_filename="book.epub", voice_sample_name="narrator")
    update_job_error(db_path, job["id"], "something went wrong")
    updated = get_job(db_path, job["id"])
    assert updated["error_message"] == "something went wrong"


def test_init_db_is_idempotent(db_path):
    init_db(db_path)  # second call should not error or drop data
    create_job(db_path, epub_filename="book.epub", voice_sample_name="narrator")
    init_db(db_path)
    assert len(list_jobs(db_path)) == 1


def test_job_has_timestamps(db_path):
    job = create_job(db_path, epub_filename="book.epub", voice_sample_name="narrator")
    assert job["created_at"] is not None
    assert job["updated_at"] is not None


def test_valid_statuses_accepted(db_path):
    for status in ("queued", "in_progress", "completed", "failed", "cancelled"):
        job = create_job(db_path, epub_filename="book.epub", voice_sample_name="narrator")
        update_job_status(db_path, job["id"], status)
        assert get_job(db_path, job["id"])["status"] == status
