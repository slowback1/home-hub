# Job Submission and Status Endpoints

## Status

`done`

## Description

Implement job submission and read endpoints: `POST /jobs`, `GET /jobs`, and `GET /jobs/{id}`. Submitted jobs are validated, saved to the SQLite DB in `queued` state, and the epub file is written to disk for the worker to pick up.

## Acceptance Criteria

- [ ] `POST /jobs` accepts a multipart form with `epub_file` and `voice_sample_name`; returns `HTTP 201` with the new job id and status `queued`
- [ ] `POST /jobs` returns `HTTP 404` if the specified `voice_sample_name` does not exist in `/data/voice-samples/`
- [ ] `POST /jobs` returns `HTTP 400` if the uploaded file is not an epub (validate by extension or MIME type)
- [ ] Epub file is saved to `/data/jobs/<job_id>/input.epub` on submission
- [ ] `GET /jobs` returns all jobs with id, status, epub_filename, voice_sample_name, created_at, updated_at
- [ ] `GET /jobs/{id}` returns job detail; returns `HTTP 404` for unknown id
- [ ] All endpoints require a valid API key
- [ ] Unit tests cover: submit valid job, submit with missing voice sample, submit non-epub, list jobs, get existing job, get non-existent job

## Notes

The epub file must be persisted to disk at submission time so the worker can access it later. Use `/data/jobs/<job_id>/input.epub` as the path. The worker picks up jobs from the `queued` state in created_at order.
