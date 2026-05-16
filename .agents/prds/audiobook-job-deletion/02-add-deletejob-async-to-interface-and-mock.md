# Add DeleteJobAsync to IAudiobookService and MockAudiobookService

## Status

`done`

## Description

Extend `IAudiobookService` with a new `DeleteJobAsync(string id)` method. Implement it fully in `MockAudiobookService` (remove the job record and clean up `_deletedFiles`). Add a `throw new NotImplementedException()` stub in `GpuServiceClient` to keep the build green. Add unit tests covering all cases.

## Acceptance Criteria

- [ ] `IAudiobookService` declares `Task DeleteJobAsync(string id)`
- [ ] `MockAudiobookService.DeleteJobAsync` removes the job from `_jobs` and cleans up its entry in `_deletedFiles`
- [ ] `MockAudiobookService.DeleteJobAsync` throws `KeyNotFoundException` for an unknown id
- [ ] `MockAudiobookService.DeleteJobAsync` throws `InvalidOperationException` if the job is Queued or InProgress
- [ ] `GpuServiceClient.DeleteJobAsync` stubs with `throw new NotImplementedException()`
- [ ] Solution builds without errors
- [ ] Unit tests in `MockAudiobookServiceTests` cover: delete completed, delete failed, delete cancelled, unknown id, queued job rejected, in-progress job rejected
- [ ] All existing tests continue to pass

## Notes

- `_deletedFiles` is a `HashSet<string>` in `MockAudiobookService` that tracks jobs whose output files have been deleted. Clean it up on `DeleteJobAsync` to avoid stale entries.
- Terminal states are: `Completed`, `Failed`, `Cancelled`. Active states are: `Queued`, `InProgress`.
- No actual files exist in the mock — file cleanup is a no-op.
