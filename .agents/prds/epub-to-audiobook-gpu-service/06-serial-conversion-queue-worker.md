# Serial Conversion Queue Worker

## Status

`pending`

## Description

Implement the background worker that drives job execution. The worker runs as a background task on service startup, picks the next `queued` job, invokes the `ai-epub-to-audiobook` script as a subprocess with env-var-configured TTS parameters, tracks the subprocess PID for later cancellation, and updates job status through `in_progress` → `completed` or `failed`.

## Acceptance Criteria

- [ ] Worker starts automatically when the FastAPI app starts
- [ ] Worker processes one job at a time; a second queued job waits until the first completes
- [ ] Job status is set to `in_progress` and subprocess PID is stored in the DB before the script is invoked
- [ ] Subprocess is invoked with all TTS parameters read from env vars (`TTS_SILENCE`, `TTS_SAMPLE_RATE`, `TTS_MIN_CHARS`, `TTS_MAX_CHARS`, `TTS_MODEL`) and the correct voice sample path
- [ ] Chapter WAVs and the final M4B are written to `/data/jobs/<job_id>/`
- [ ] On successful completion, job status is set to `completed` and `updated_at` is refreshed
- [ ] On subprocess non-zero exit, job status is set to `failed` and stderr is stored in `error_message`
- [ ] Worker continues to the next queued job after each completion or failure
- [ ] Unit tests cover: successful job run (mock subprocess), failed job run (mock subprocess non-zero exit), worker picks jobs in created_at order

## Notes

Use `asyncio.create_task` or a daemon thread to run the worker loop alongside FastAPI. The subprocess command is the `ai-epub-to-audiobook` entry point with arguments derived from the job record and env vars. Store the subprocess PID in `jobs.pid` immediately after `Popen` so the cancellation endpoint (task 08) can SIGTERM it.
