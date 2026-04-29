# Implement EfSystemConfigProvider

## Status

`pending` <!-- pending | in-progress | done -->

## Description

Implement the EntityFramework-backed `ISystemConfigProvider` in the `EntityFramework` project. This is the production implementation that reads from and writes to the `system_config` table. It must enforce secret masking on `GetAsync` and `GetAllAsync`, expose real values on `GetSecretAsync`, and throw when a requested key does not exist.

## Acceptance Criteria

- [ ] `EntityFramework/EfSystemConfigProvider.cs` exists and implements `ISystemConfigProvider`
- [ ] `GetAsync` returns the entry with `Value = "***"` when `IsSecret` is true; throws if the key is not found
- [ ] `GetSecretAsync` returns the entry with the real `Value`; throws if the key is not found
- [ ] `GetAllAsync` returns all rows with secrets masked (`Value = "***"` where `IsSecret` is true)
- [ ] `UpdateAsync` sets only the `Value` field and persists the change; returns the updated entry (with secret masking applied)
- [ ] The implementation uses `ICrud<SystemConfig>` obtained via `ICrudFactory` (not `AppDbContext` directly)
- [ ] Integration tests in `EntityFramework.Tests` (or `WebAPI.Integration.Tests`) cover: each method's happy path, secret masking, and throw-on-missing
- [ ] `task test` passes

## Notes

Obtain `ICrud<SystemConfig>` via `ICrudFactory.GetCrud<SystemConfig>()`. This keeps the EF implementation decoupled from `AppDbContext` directly and consistent with how `EfCrud<T>` is used elsewhere.

Masking is applied in the service layer — the controller does not perform additional masking. `"***"` is the sentinel value returned for any entry where `IsSecret == true`.
