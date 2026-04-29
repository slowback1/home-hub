# Implement DictionarySystemConfigProvider

## Status

`pending` <!-- pending | in-progress | done -->

## Description

Implement an in-memory dictionary-backed `ISystemConfigProvider` in the `Logic` project, following the same pattern as `DictionaryFeatureFlagProvider`. This implementation is used by unit and integration tests throughout the feature so later tasks can test without hitting the database.

## Acceptance Criteria

- [ ] `Logic/SystemConfig/DictionarySystemConfigProvider.cs` exists and implements `ISystemConfigProvider`
- [ ] `GetAsync` returns the entry with `Value = "***"` when `IsSecret` is true
- [ ] `GetAsync` throws (e.g. `KeyNotFoundException`) when the key is not found
- [ ] `GetSecretAsync` returns the entry with the real value; throws if not found
- [ ] `GetAllAsync` returns all seeded entries with secrets masked
- [ ] `UpdateAsync` updates only the `Value` field on the in-memory entry and returns it
- [ ] Unit tests in `Logic.Tests` cover: happy path for each method, secret masking, and throw-on-missing for `GetAsync` and `GetSecretAsync`
- [ ] All tests pass

## Notes

The provider should accept seed data via its constructor (a list or dictionary of `SystemConfig` entries) so tests can pre-populate known state without relying on any external setup.

Lives in `Logic` alongside `DictionaryFeatureFlagProvider` in `Logic/FeatureFlags/`.
