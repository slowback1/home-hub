# IAudiobookService Interface and Models

## Status

`done`

## Description

Define the `IAudiobookService` interface and the shared models it depends on in the `Common` project. This is the contract that both the mock and real implementations will fulfil, and the type surface the controller will program against.

## Acceptance Criteria

- [ ] `IAudiobookService` interface exists in `Common/Interfaces/` with methods covering all job and voice-sample operations:
  - `ListJobsAsync()` → `IEnumerable<AudiobookJob>`
  - `SubmitJobAsync(string epubFileName, byte[] epubBytes, string voiceSampleName)` → `AudiobookJob`
  - `GetJobAsync(string id)` → `AudiobookJob`
  - `CancelJobAsync(string id)` → `Task`
  - `GetFileAsync(string id)` → `Stream`
  - `DeleteFileAsync(string id)` → `Task`
  - `ListVoiceSamplesAsync()` → `IEnumerable<string>`
  - `UploadVoiceSampleAsync(string name, byte[] wavBytes)` → `Task`
  - `DeleteVoiceSampleAsync(string name)` → `Task`
- [ ] `AudiobookJob` model exists in `Common/Models/` with fields matching the GPU service response: `Id`, `Status`, `EpubFilename`, `VoiceSampleName`, `CreatedAt`, `UpdatedAt`, `ErrorMessage`
- [ ] `AudiobookJobStatus` enum exists in `Common/Models/` with values: `Queued`, `InProgress`, `Completed`, `Failed`, `Cancelled`
- [ ] All new types are in the `Common` project (not Logic or WebAPI) so both consumers can reference them without circular dependencies
- [ ] Project builds with no errors

## Notes

Model field names should use C# conventions (PascalCase) but map cleanly to the GPU service's snake_case JSON fields (`epub_filename`, `voice_sample_name`, etc.) — serialization attributes or naming policy can be handled in the controller task.

`GetFileAsync` returns a raw `Stream`; the controller is responsible for wrapping it in a file response with the correct content type (`audio/mp4`).
