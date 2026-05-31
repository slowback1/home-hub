# Add WALK_SESSION_HISTORY_ENABLED Feature Flag

## Status

`done`

## Description

Register the `WALK_SESSION_HISTORY_ENABLED` constant in the frontend feature flags file. This is a prerequisite for gating the sidebar nav entry and widget registry entry.

## Acceptance Criteria

- [ ] `WALK_SESSION_HISTORY_ENABLED: 'WALK_SESSION_HISTORY_ENABLED'` is added to the `FeatureFlags` object in `frontend/src/lib/services/FeatureFlag/FeatureFlags.ts`
- [ ] No other files are changed in this task

## Notes

- File: `frontend/src/lib/services/FeatureFlag/FeatureFlags.ts`.
- The flag value string must match the key exactly (same pattern as all existing flags).
- The flag does not need to be seeded in the database as part of this task — that can be done manually or via a future migration.
