# Register IFeatureFlagProvider in DI

## Status

`pending` <!-- pending | in-progress | done -->

## Description

Wire `IFeatureFlagProvider` into the ASP.NET Core DI container in `Program.cs`, conditional on the `CrudFactory:Implementation` config value. Mirrors the existing pattern used for `ICrudFactory` registration.

## Acceptance Criteria

- [ ] When `CrudFactory:Implementation` is `entityframework`, `EntityFrameworkFeatureFlagProvider` is registered as `IFeatureFlagProvider`
- [ ] When `CrudFactory:Implementation` is `inmemory`, `DictionaryFeatureFlagProvider` (with an empty dictionary) is registered as `IFeatureFlagProvider`
- [ ] App starts without DI errors in both `entityframework` and `inmemory` modes
- [ ] `GET /feature-flags` resolves correctly at runtime (returns flags from DB in EF mode)

## Notes

- Registration goes in `backend/WebAPI/Program.cs` alongside the existing `crudImpl` conditional block
- `DictionaryFeatureFlagProvider` constructor takes a `Dictionary<string, bool>` — pass `new Dictionary<string, bool>()` for the InMemory registration
- Depends on tasks 02 and 04
