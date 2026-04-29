# Register ISystemConfigProvider in DI

## Status

`pending` <!-- pending | in-progress | done -->

## Description

Wire `EfSystemConfigProvider` into the service collection so that `ISystemConfigProvider` can be injected anywhere in the application. Registration should be consistent with how `IFeatureFlagProvider` and the CRUD factory are registered.

## Acceptance Criteria

- [ ] `ISystemConfigProvider` is registered in `Program.cs` (or an appropriate configurator) with `EfSystemConfigProvider` as the implementation
- [ ] The registration is scoped (consistent with other scoped services in the app)
- [ ] Resolving `ISystemConfigProvider` from the DI container in an integration test succeeds without throwing
- [ ] `task test` passes

## Notes

Follow the pattern of `IFeatureFlagProvider` registration. If registration is conditional on `CrudFactory:Implementation`, match whatever pattern is used for the feature flag provider.
