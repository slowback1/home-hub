# Introduce Hangfire Infrastructure

## Status

`done` <!-- pending | in-progress | done -->

## Description

Add Hangfire as the background job framework for the application. This is foundational infrastructure that will be reused by future background jobs beyond the activity picker. The setup includes pluggable storage (PostgreSQL in production, InMemory in dev/CI) keyed off the existing `CrudFactory:Implementation` config value, and the Hangfire dashboard mounted at `/admin/hangfire`.

## Acceptance Criteria

- [ ] `Hangfire.AspNetCore`, `Hangfire.PostgreSql`, and `Hangfire.InMemory` NuGet packages are added to the backend
- [ ] Hangfire services are registered in `Program.cs` with storage selected by `CrudFactory:Implementation`: `EntityFramework` → `Hangfire.PostgreSql`, anything else → `Hangfire.InMemory`
- [ ] Hangfire server (job processor) is started via `app.UseHangfireServer()` or equivalent
- [ ] Hangfire dashboard is mounted and accessible at `/admin/hangfire` with no auth guard (home network only — see PRD note)
- [ ] App starts without errors in both InMemory and EntityFramework configurations
- [ ] No recurring jobs are registered yet (that is task 05)

## Notes

- The existing `CrudFactory:Implementation` values are `InMemory`, `FileData`, and `EntityFramework` — only `EntityFramework` should use `Hangfire.PostgreSql`; both `InMemory` and `FileData` environments should use `Hangfire.InMemory`
- PostgreSQL connection string for Hangfire should reuse the same connection string already configured for EF (`ConnectionStrings:DefaultConnection` or equivalent — check `appsettings.json`)
- PRD note on dashboard auth: no guard needed now; adding one later requires implementing `IDashboardAuthorizationFilter` — must be done before any internet exposure
