# Implement FeatureFlagController

## Status

`pending` <!-- pending | in-progress | done -->

## Description

Add a `FeatureFlagController` in `backend/WebAPI/Controllers/` that exposes a `GET /feature-flags` endpoint. The controller delegates to `GetFeatureFlagsUseCase` and returns the flag list as JSON, following the same thin-controller pattern as `ExampleDataController`.

## Acceptance Criteria

- [ ] `GET /feature-flags` returns `200 OK` with a JSON array of `{ name, isEnabled }` objects
- [ ] `GET /feature-flags` returns an empty array (not an error) when no flags exist in the DB
- [ ] Controller extends `ApplicationController` and uses `ToActionResult()` for the response
- [ ] Swagger/OpenAPI lists the endpoint after startup

## Notes

- Follow the pattern at `backend/WebAPI/Controllers/ExampleDataController.cs`
- `ApplicationController` base class is at `backend/WebAPI/Controllers/ApplicationController.cs`
- `IFeatureFlagProvider` must be injected into the controller (not `ICrudFactory` — feature flags have their own provider abstraction)
- Depends on tasks 02 and 03
