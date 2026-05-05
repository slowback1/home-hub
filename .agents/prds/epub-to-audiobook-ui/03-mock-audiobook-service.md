# MockAudiobookService

## Status

`done`

## Description

Implement `MockAudiobookService` in the `Logic` project — an in-memory implementation of `IAudiobookService` that automatically advances job status through the full state machine on a short timer. This enables full UI development and E2E test coverage without a running GPU service.

## Acceptance Criteria

- [ ] `MockAudiobookService` exists in `Logic/Audiobook/` and implements `IAudiobookService`
- [ ] Jobs progress automatically: `Queued` → `InProgress` after ~1 second, `InProgress` → `Completed` after ~3 more seconds
- [ ] If `epubFileName` contains the substring `"fail"` (case-insensitive), the job instead advances to `Failed` with a non-empty `ErrorMessage` — this provides a deterministic way to trigger the failed state in E2E tests
- [ ] Voice samples are stored in an in-memory list; the list is pre-seeded with at least one sample (e.g. `"default"`) so the Convert tab form is enabled on first load
- [ ] `ListJobsAsync` returns jobs oldest-first (ascending `CreatedAt`)
- [ ] `GetFileAsync` returns a `MemoryStream` containing a minimal valid M4B/MP4 byte sequence for completed jobs, and throws an appropriate exception for non-completed jobs
- [ ] `CancelJobAsync` sets status to `Cancelled` for `Queued` or `InProgress` jobs; throws for jobs in terminal states
- [ ] `UploadVoiceSampleAsync` adds the name to the in-memory list (bytes are accepted but discarded)
- [ ] `DeleteVoiceSampleAsync` removes the name; throws `KeyNotFoundException` if not found
- [ ] Unit tests cover: happy-path state progression, cancel, failed-job trigger, voice sample add/delete, and exception cases

## Notes

The timer-based state progression should use `Task.Delay` or a `System.Threading.Timer` scoped to each job — not a global background thread — so tests can be run in parallel without cross-contamination. Because the service is registered as scoped (per-request), each test scenario gets its own instance.

The "fail" substring trigger gives E2E tests a stable way to reach the `Failed` state: submit a job with an EPUB filename like `fail.epub`.
