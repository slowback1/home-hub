# Wire E2E Environment

## Status

`pending`

## Description

Configure the backend to use `DictionarySystemConfigProvider` when running under `ASPNETCORE_ENVIRONMENT=E2E`, and update the E2E task runner to set that environment variable when spinning up the backend. The dictionary is pre-seeded with the same weather config defaults used in E2E scenarios so tests run without a real database.

## Acceptance Criteria

- [ ] `appsettings.E2E.json` exists in the WebAPI project
- [ ] When `ASPNETCORE_ENVIRONMENT=E2E`, the DI container resolves `ISystemConfigProvider` as `DictionarySystemConfigProvider`
- [ ] The dictionary is pre-seeded with `weather/zip_code = "10001"` (non-secret) and `weather/api_key = "test-api-key-1"` (secret)
- [ ] Writes (PUT) work end-to-end in E2E mode via the existing InMemory CRUD implementation
- [ ] `Taskfile.yml` sets `ASPNETCORE_ENVIRONMENT=E2E` when starting the backend for E2E tests
- [ ] The existing production DI registration (EfSystemConfigProvider) is unaffected
- [ ] Existing integration tests continue to pass

## Notes

The `DictionarySystemConfigProvider` already exists at `backend/Logic/SystemConfig/DictionarySystemConfigProvider.cs`. Check how it is initialised (constructor args, dictionary format) before wiring DI.

The InMemory CRUD implementation handles the write path — verify that `UpdateAsync` on the dictionary provider delegates correctly to it in E2E mode.

`Taskfile.yml` is at the repo root. The E2E task that spins up the backend (see `e2e/AGENTS.md`) needs an `env` block or equivalent to inject `ASPNETCORE_ENVIRONMENT=E2E`.
