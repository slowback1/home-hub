# Add PATCH Toggle Endpoint

## Status

`done` <!-- pending | in-progress | done -->

## Description

Add the backend write path for toggling a feature flag. This introduces a repository interface, an Entity Framework implementation, a use case, and a new PATCH action on the existing `FeatureFlagController`. This is the only backend change required by the PRD; the GET endpoint and existing read path are untouched.

## Acceptance Criteria

- [ ] `IFeatureFlagRepository` interface exists in `backend/Common/Interfaces/` with an `UpdateAsync(name, isEnabled)` method
- [ ] `EntityFrameworkFeatureFlagRepository` in `backend/EntityFramework/` implements the interface and updates `IsEnabled` via `AppDbContext`
- [ ] `UpdateFeatureFlagUseCase` in `backend/Logic/FeatureFlags/` wraps the repository call
- [ ] `FeatureFlagController` exposes `PATCH /api/feature-flags/{name}` accepting `{"isEnabled": bool}`, returning `200` with the updated flag or `404` if the name is not found
- [ ] Repository and use case are registered in DI (conditional on `CrudFactory:Implementation` config, mirroring the existing pattern)
- [ ] Integration test covers the happy path (flag toggled on, flag toggled off) and the 404 case

## Notes

- Follow the layered pattern established by `system-config-infrastructure`: interface in `Common`, EF implementation in `EntityFramework`, use case in `Logic`, controller action in `WebAPI`.
- The `FeatureFlags` table and `AppDbContext` already exist — no migration needed.
- DI registration should mirror how `EntityFrameworkFeatureFlagProvider` is currently wired (see `CrudFactory:Implementation` config key).
- Request DTO: a simple record with a single `bool IsEnabled` property is sufficient.
