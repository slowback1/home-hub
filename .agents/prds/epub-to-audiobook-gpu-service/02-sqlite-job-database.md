# SQLite Job Database

## Status

`pending`

## Description

Initialize the SQLite database on service startup and define the Job model. This is the persistence layer that all job-related endpoints and the queue worker depend on.

## Acceptance Criteria

- [ ] SQLite DB file is created at `/data/jobs/jobs.db` on first startup (path configurable via env var)
- [ ] `jobs` table is created with columns: `id` (UUID), `status` (text), `epub_filename` (text), `voice_sample_name` (text), `created_at` (datetime), `updated_at` (datetime), `error_message` (text nullable), `pid` (integer nullable)
- [ ] Helper functions exist for: create job, get job by id, list all jobs, update job status, update job pid, update job error
- [ ] Valid status values are enforced: `queued`, `in_progress`, `completed`, `failed`, `cancelled`
- [ ] DB init is idempotent — running it twice does not error or drop data
- [ ] Unit tests cover create, read, update, and status transition helpers

## Notes

DB path should default to `/data/jobs/jobs.db`. The `/data/jobs` directory is a Docker volume mount — the directory will exist at runtime but tests should use a temp path.
