# Implement GetFeatureFlagsUseCase

## Status

`pending` <!-- pending | in-progress | done -->

## Description

Add a `GetFeatureFlagsUseCase` in `backend/Logic/FeatureFlags/` that calls `IFeatureFlagProvider.GetFeatureFlags()` and wraps the result in a `UseCaseResult`. Keeps the controller thin and consistent with the existing use-case pattern.

## Acceptance Criteria

- [ ] `GetFeatureFlagsUseCase` exists in `backend/Logic/FeatureFlags/` and accepts `IFeatureFlagProvider` via constructor
- [ ] `Execute()` returns a `UseCaseResult<IEnumerable<FeatureFlag>>` with status `Success` and the full list of flags
- [ ] Unit tests exist covering: returns all flags from provider, returns empty list when provider returns nothing

## Notes

- Follow the pattern established by `backend/Logic/GetExampleDataUseCase.cs`
- `UseCaseResult` and `UseCaseStatus` are in `backend/Common/Models/`
- `IFeatureFlagProvider` is in `backend/Common/Interfaces/IFeatureFlagProvider.cs`
- Depends on task 02 (provider exists to inject in tests)
