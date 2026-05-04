# Voice Sample Management Endpoints

## Status

`pending`

## Description

Implement the three voice sample management endpoints. Voice samples are WAV files stored in `/data/voice-samples/` and referenced by name when submitting a conversion job.

## Acceptance Criteria

- [ ] `GET /voice-samples` returns a list of available voice sample names (filenames without extension)
- [ ] `POST /voice-samples` accepts a WAV file upload and saves it to `/data/voice-samples/<name>.wav`; returns `HTTP 201` with the sample name
- [ ] `POST /voice-samples` returns `HTTP 400` if the uploaded file is not a WAV
- [ ] `DELETE /voice-samples/{name}` deletes the file and returns `HTTP 204`; returns `HTTP 404` if the sample does not exist
- [ ] All three endpoints require a valid API key
- [ ] Unit tests cover: list empty, list with samples, upload success, upload wrong format, delete existing, delete non-existent

## Notes

Voice sample directory defaults to `/data/voice-samples/`. Tests should use a temp directory. The `name` used in `POST /voice-samples` should be derived from the uploaded filename (stem only, no extension).
