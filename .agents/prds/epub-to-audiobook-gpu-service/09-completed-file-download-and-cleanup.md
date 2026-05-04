# Completed File Download and Cleanup

## Status

`pending`

## Description

Implement the two file endpoints: `GET /jobs/{id}/file` streams the completed M4B to the caller, and `DELETE /jobs/{id}/file` deletes the file from the GPU disk after the main HomeHub server has successfully downloaded it.

## Acceptance Criteria

- [ ] `GET /jobs/{id}/file` streams the M4B file with `Content-Type: audio/mp4` and `Content-Disposition: attachment; filename="<epub_filename>.m4b"`
- [ ] `GET /jobs/{id}/file` returns `HTTP 404` if the job does not exist
- [ ] `GET /jobs/{id}/file` returns `HTTP 409` if the job status is not `completed`
- [ ] `GET /jobs/{id}/file` returns `HTTP 410` if the file has already been deleted (job completed but file gone)
- [ ] `DELETE /jobs/{id}/file` deletes the M4B from `/data/jobs/<job_id>/` and returns `HTTP 204`
- [ ] `DELETE /jobs/{id}/file` returns `HTTP 404` if the job does not exist or the file is already gone
- [ ] `DELETE /jobs/{id}/file` returns `HTTP 409` if the job is not yet `completed`
- [ ] Both endpoints require a valid API key
- [ ] Unit tests cover: stream completed file, stream non-existent job, stream incomplete job, delete file success, delete already-deleted file, delete incomplete job

## Notes

Use FastAPI's `FileResponse` or `StreamingResponse` for the download — `StreamingResponse` with a generator is preferable for large files to avoid loading the entire M4B into memory. The main HomeHub server calls `DELETE /jobs/{id}/file` immediately after a successful download to free GPU disk space.
