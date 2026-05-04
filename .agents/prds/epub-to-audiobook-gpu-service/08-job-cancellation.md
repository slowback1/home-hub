# Job Cancellation

## Status

`done`

## Description

Implement `DELETE /jobs/{id}` to allow cancelling a queued or in-progress job. For in-progress jobs, the running subprocess is terminated via SIGTERM and partial chapter WAVs are cleaned up. For queued jobs, the job is removed from the queue without any subprocess interaction.

## Acceptance Criteria

- [ ] `DELETE /jobs/{id}` returns `HTTP 204` on success
- [ ] `DELETE /jobs/{id}` returns `HTTP 404` for an unknown job id
- [ ] `DELETE /jobs/{id}` returns `HTTP 409` if the job is already `completed`, `failed`, or `cancelled`
- [ ] For an `in_progress` job: the tracked subprocess PID is sent SIGTERM; job status is set to `cancelled`
- [ ] For a `queued` job: job status is set to `cancelled` with no subprocess interaction
- [ ] Partial chapter WAVs and the input epub in `/data/jobs/<job_id>/` are deleted after cancellation
- [ ] The worker does not attempt to start the next job using the cancelled job's resources
- [ ] Unit tests cover: cancel queued job, cancel in-progress job (mock SIGTERM), cancel completed job returns 409, cancel unknown job returns 404

## Notes

Read the subprocess PID from `jobs.pid` in the DB. If the PID is no longer alive (process already exited), treat it as a no-op and still mark the job `cancelled`. Use `os.kill(pid, signal.SIGTERM)` and catch `ProcessLookupError` for the already-dead case.
