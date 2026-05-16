# Implement DeleteJobAsync in GpuServiceClient

## Status

`done`

## Description

Replace the `NotImplementedException` stub in `GpuServiceClient.DeleteJobAsync` with a real HTTP implementation that calls `DELETE /api/jobs/{id}` on the GPU service. The GPU service handles filesystem cleanup (output directory and source EPUB) server-side.

## Acceptance Criteria

- [ ] `GpuServiceClient.DeleteJobAsync` calls `DELETE {baseUrl}/api/jobs/{id}` with bearer auth
- [ ] A 204 response is treated as success
- [ ] A 404 response from the GPU service maps to `KeyNotFoundException`
- [ ] Other non-success responses are treated as errors (throw or surface appropriately, consistent with other methods in the class)
- [ ] Solution builds without errors
- [ ] All existing tests continue to pass

## Notes

- Follow the pattern of existing methods in `GpuServiceClient` for auth header attachment and response handling.
- The GPU service encapsulates all filesystem cleanup (output directory + source EPUB) — no separate file-deletion call is needed from the client.
- No new integration tests are required here since `GpuServiceClient` is not exercised in the integration test suite (which uses `MockAudiobookService`).
