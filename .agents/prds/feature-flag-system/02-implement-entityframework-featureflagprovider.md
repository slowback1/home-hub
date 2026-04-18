# Implement EntityFrameworkFeatureFlagProvider

## Status

`pending` <!-- pending | in-progress | done -->

## Description

Add a new `IFeatureFlagProvider` implementation in `backend/EntityFramework/` that reads all feature flags from `AppDbContext`. This is the database-backed provider that replaces the dictionary-based approach used in tests.

## Acceptance Criteria

- [ ] `EntityFrameworkFeatureFlagProvider` exists in `backend/EntityFramework/` and implements `IFeatureFlagProvider`
- [ ] `GetFeatureFlags()` queries `AppDbContext.FeatureFlags` and returns all rows as `IEnumerable<FeatureFlag>`
- [ ] Unit tests exist covering: returns all flags when rows are present, returns empty when table is empty

## Notes

- Place file at `backend/EntityFramework/EntityFrameworkFeatureFlagProvider.cs`
- `IFeatureFlagProvider` interface is at `backend/Common/Interfaces/IFeatureFlagProvider.cs`
- `AppDbContext` will have `DbSet<FeatureFlag> FeatureFlags` after task 01
- Depends on task 01 (DbSet registration)
- The existing `DictionaryFeatureFlagProvider` in `backend/Logic/FeatureFlags/` is a useful reference for the interface contract
