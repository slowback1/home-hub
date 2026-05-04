import sqlite3
import uuid
from datetime import datetime, timezone

VALID_STATUSES = {"queued", "in_progress", "completed", "failed", "cancelled"}


def _connect(db_path: str) -> sqlite3.Connection:
    conn = sqlite3.connect(db_path)
    conn.row_factory = sqlite3.Row
    return conn


def init_db(db_path: str) -> None:
    with _connect(db_path) as conn:
        conn.execute("""
            CREATE TABLE IF NOT EXISTS jobs (
                id TEXT PRIMARY KEY,
                status TEXT NOT NULL DEFAULT 'queued',
                epub_filename TEXT NOT NULL,
                voice_sample_name TEXT NOT NULL,
                created_at TEXT NOT NULL,
                updated_at TEXT NOT NULL,
                error_message TEXT,
                pid INTEGER
            )
        """)


def _row_to_dict(row) -> dict | None:
    if row is None:
        return None
    return dict(row)


def create_job(db_path: str, epub_filename: str, voice_sample_name: str) -> dict:
    job_id = str(uuid.uuid4())
    now = datetime.now(timezone.utc).isoformat()
    with _connect(db_path) as conn:
        conn.execute(
            "INSERT INTO jobs (id, status, epub_filename, voice_sample_name, created_at, updated_at) "
            "VALUES (?, 'queued', ?, ?, ?, ?)",
            (job_id, epub_filename, voice_sample_name, now, now),
        )
    return get_job(db_path, job_id)


def get_job(db_path: str, job_id: str) -> dict | None:
    with _connect(db_path) as conn:
        row = conn.execute("SELECT * FROM jobs WHERE id = ?", (job_id,)).fetchone()
    return _row_to_dict(row)


def list_jobs(db_path: str) -> list[dict]:
    with _connect(db_path) as conn:
        rows = conn.execute("SELECT * FROM jobs ORDER BY created_at ASC").fetchall()
    return [dict(r) for r in rows]


def update_job_status(db_path: str, job_id: str, status: str) -> None:
    if status not in VALID_STATUSES:
        raise ValueError(f"Invalid status '{status}'. Must be one of {VALID_STATUSES}")
    now = datetime.now(timezone.utc).isoformat()
    with _connect(db_path) as conn:
        conn.execute(
            "UPDATE jobs SET status = ?, updated_at = ? WHERE id = ?",
            (status, now, job_id),
        )


def update_job_pid(db_path: str, job_id: str, pid: int) -> None:
    now = datetime.now(timezone.utc).isoformat()
    with _connect(db_path) as conn:
        conn.execute(
            "UPDATE jobs SET pid = ?, updated_at = ? WHERE id = ?",
            (pid, now, job_id),
        )


def update_job_error(db_path: str, job_id: str, error_message: str) -> None:
    now = datetime.now(timezone.utc).isoformat()
    with _connect(db_path) as conn:
        conn.execute(
            "UPDATE jobs SET error_message = ?, updated_at = ? WHERE id = ?",
            (error_message, now, job_id),
        )
