# Implement SystemConfigController

## Status

`done` <!-- pending | in-progress | done -->

## Description

Expose the three system config REST endpoints via a new `SystemConfigController`. Secrets are always masked in responses — the controller delegates to `ISystemConfigProvider`, which handles masking. The PUT endpoint accepts only a `value` update; all other fields are immutable.

## Acceptance Criteria

- [ ] `WebAPI/Controllers/SystemConfigController.cs` exists with three endpoints:
  - `GET /api/system-config` — returns a flat array of all config entries (secrets masked)
  - `GET /api/system-config/{namespace}/{key}` — returns a single entry (secret masked); returns 404 if not found
  - `PUT /api/system-config/{namespace}/{key}` — accepts `{ "value": "..." }` body; returns the updated entry (secret masked); returns 404 if not found
- [ ] Response shape per entry: `{ "id", "namespace", "key", "value", "type", "isSecret" }`
- [ ] Secret entries always return `"***"` as `value` in all responses
- [ ] Integration tests in `WebAPI.Integration.Tests` cover:
  - GET all returns all entries with secrets masked
  - GET one returns the correct entry
  - GET one returns 404 for an unknown key
  - PUT updates the value and returns the updated entry
  - PUT returns 404 for an unknown key
- [ ] `task test` passes

## Notes

Use `DictionarySystemConfigProvider` (seeded with known test data) in integration tests — do not rely on the database for controller tests.

The controller does not apply masking itself — it calls `ISystemConfigProvider.GetAsync` / `GetAllAsync`, which already return masked values. The controller is responsible only for routing, HTTP status codes, and response serialization.
