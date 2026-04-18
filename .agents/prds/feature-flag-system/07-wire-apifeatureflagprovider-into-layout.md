# Wire ApiFeatureFlagProvider into Layout

## Status

`pending` <!-- pending | in-progress | done -->

## Description

Swap `ConfigFeatureFlagProvider` for `ApiFeatureFlagProvider` in the root layout so that the app fetches feature flags from the backend API at startup instead of from static config.

## Acceptance Criteria

- [ ] `+layout.svelte` initializes `FeatureFlagService` with `ApiFeatureFlagProvider` instead of `ConfigFeatureFlagProvider`
- [ ] `ConfigFeatureFlagProvider` import is removed from `+layout.svelte` (it remains available for tests/offline dev — just not wired in production)
- [ ] App loads without console errors when the backend is running
- [ ] `DEMO_FEATURE_FLAG` state is correctly reflected at runtime (disabled by default per the seeded migration)

## Notes

- Change is in `frontend/src/routes/+layout.svelte` at line 20
- `ConfigFeatureFlagProvider` should not be deleted — it remains useful for unit tests and offline dev scenarios
- Depends on tasks 05 and 06 (backend endpoint live, frontend provider implemented)
