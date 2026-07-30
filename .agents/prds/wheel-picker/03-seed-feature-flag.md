# Seed `WHEEL_PICKER_ENABLED` feature flag

## Status

`pending`

## Description

Add a migration that seeds the `WHEEL_PICKER_ENABLED` feature flag as disabled, so the feature ships gated off until explicitly enabled.

## Acceptance Criteria

- [ ] A migration inserts `{ Name: "WHEEL_PICKER_ENABLED", IsEnabled: false }` into the `FeatureFlags` table via `InsertData`.
- [ ] The migration's `Down` removes the flag via `DeleteData`.
- [ ] The migration scaffold is generated with `dotnet ef` (only the `InsertData`/`DeleteData` body is hand-edited); the snapshot/`.Designer.cs` come from the tool.
- [ ] App starts without a `PendingModelChangesWarning`.

## Notes

- Mirror `backend/EntityFramework/Migrations/20260531010045_SeedWalkSessionHistoryFeatureFlag.cs`.
- Generate with:
  `dotnet ef migrations add SeedWheelPickerFeatureFlag --project backend/EntityFramework --startup-project backend/WebAPI`
- Depends on task 02 being applied first (ordering of migrations).
