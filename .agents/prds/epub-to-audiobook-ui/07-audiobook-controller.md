# AudiobookController

## Status

`done`

## Description

Add `AudiobookController` to the WebAPI project. It receives HTTP requests from the SvelteKit frontend, delegates to `IAudiobookService`, and returns appropriate responses. Covers all job lifecycle and voice sample endpoints.

## Acceptance Criteria

- [ ] `AudiobookController` exists at `backend/WebAPI/Controllers/AudiobookController.cs` with base route `api/audiobook`
- [ ] All job endpoints are implemented:
  - `GET /api/audiobook/jobs` → 200 with list of `AudiobookJob`
  - `POST /api/audiobook/jobs` (multipart: `epubFile` IFormFile + `voiceSampleName` string) → 201 with created `AudiobookJob`
  - `GET /api/audiobook/jobs/{id}` → 200 with `AudiobookJob`, or 404
  - `DELETE /api/audiobook/jobs/{id}` → 204, or 404, or 409 if job is in a terminal state
  - `GET /api/audiobook/jobs/{id}/file` → 200 file stream (`audio/mp4`), or 404, or 409 if not completed
  - `DELETE /api/audiobook/jobs/{id}/file` → 204, or 404, or 409
- [ ] All voice sample endpoints are implemented:
  - `GET /api/audiobook/voice-samples` → 200 with list of strings
  - `POST /api/audiobook/voice-samples` (multipart: `file` IFormFile + `name` string) → 201, or 400 if file is not `.wav`
  - `DELETE /api/audiobook/voice-samples/{name}` → 204, or 404
- [ ] `KeyNotFoundException` from the service layer maps to 404; `InvalidOperationException` maps to 409
- [ ] `.wav` validation on voice sample upload rejects non-WAV files with 400
- [ ] Controller-level integration tests (using the in-memory test host) cover happy paths and key error cases for each endpoint

## Notes

Use constructor injection for `IAudiobookService`. The controller should not contain business logic — just HTTP translation (request parsing, status code mapping, response serialisation).

For `GET /api/audiobook/jobs/{id}/file`, use `FileStreamResult` with content type `audio/mp4` and a suggested filename derived from the job's `EpubFilename` (swap `.epub` → `.m4b`).

WAV validation for voice sample upload: check that the file extension is `.wav` (case-insensitive). Do not attempt deep magic-byte validation.

See `ActivityController.cs` for the established controller style in this project.
