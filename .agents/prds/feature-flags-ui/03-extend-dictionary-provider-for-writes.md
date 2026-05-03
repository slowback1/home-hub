# Extend DictionaryFeatureFlagProvider for Writes

## Status

`done` <!-- pending | in-progress | done -->

## Description

The E2E environment uses `DictionaryFeatureFlagProvider` instead of the EF provider to avoid a real database. Currently it only supports reads. Two of the four E2E scenarios toggle a flag, so the dictionary provider needs to handle `UpdateAsync` calls in-memory. This mirrors how `DictionarySystemConfigProvider` handles writes for the system-config E2E suite.

## Acceptance Criteria

- [ ] `DictionaryFeatureFlagProvider` implements `IFeatureFlagRepository.UpdateAsync` — mutates the in-memory flag state
- [ ] The E2E environment wires `DictionaryFeatureFlagProvider` as the `IFeatureFlagRepository` implementation (alongside its existing `IFeatureFlagProvider` registration)
- [ ] Toggling a flag in E2E mode is reflected by a subsequent `GET /api/feature-flags` call within the same server session

## Notes

- `DictionaryFeatureFlagProvider` is in `backend/Logic/FeatureFlags/`. It currently implements `IFeatureFlagProvider`; extend it to also implement `IFeatureFlagRepository`.
- E2E environment is activated via `ASPNETCORE_ENVIRONMENT=E2E` — check `appsettings.E2E.json` and the DI registration block that handles this env for `DictionarySystemConfigProvider` (in `system-config-infrastructure`) as the reference pattern.
- No new class needed — just extend the existing dictionary provider and add the DI wiring.
