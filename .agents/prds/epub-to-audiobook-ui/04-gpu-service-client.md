# GpuServiceClient

## Status

`pending`

## Description

Implement `GpuServiceClient` in the `Logic` project — the real `IAudiobookService` implementation that forwards calls to the FastAPI GPU service over HTTP. It reads the GPU service base URL and API key from system config at runtime.

## Acceptance Criteria

- [ ] `GpuServiceClient` exists in `Logic/Audiobook/` and implements `IAudiobookService`
- [ ] Base URL is read from system config key `audiobook::url` via `ISystemConfigProvider`
- [ ] API key is read from system config key `audiobook::api_key` and sent as a `Bearer` token in the `Authorization` header on every request
- [ ] All GPU service endpoints are covered:
  - `GET /jobs` → `ListJobsAsync`
  - `POST /jobs` (multipart: `epub_file` + `voice_sample_name`) → `SubmitJobAsync`
  - `GET /jobs/{id}` → `GetJobAsync`
  - `DELETE /jobs/{id}` → `CancelJobAsync`
  - `GET /jobs/{id}/file` → `GetFileAsync` (streams response body)
  - `DELETE /jobs/{id}/file` → `DeleteFileAsync`
  - `GET /voice-samples` → `ListVoiceSamplesAsync`
  - `POST /voice-samples` (multipart: `file` + `name`) → `UploadVoiceSampleAsync`
  - `DELETE /voice-samples/{name}` → `DeleteVoiceSampleAsync`
- [ ] HTTP 404 responses from the GPU service are mapped to `KeyNotFoundException`
- [ ] HTTP 409 responses are mapped to `InvalidOperationException`
- [ ] `HttpClient` is injected (not instantiated directly) to support testing and connection pooling
- [ ] Unit tests cover: successful calls for each method, and 404/409 error mapping

## Notes

Follow the pattern of `OpenWeatherMapProvider` in `Logic/Weather/` for reading config via `ISystemConfigProvider` and for `HttpClient` usage.

GPU service JSON uses snake_case (`epub_filename`, `voice_sample_name`, etc.). Use `JsonSerializerOptions` with `JsonNamingPolicy.SnakeCaseLower` when deserializing responses, or annotate the DTO models with `[JsonPropertyName]` attributes.

`GetFileAsync` should stream the response directly rather than buffering the full file in memory, since audiobook files can be large.
