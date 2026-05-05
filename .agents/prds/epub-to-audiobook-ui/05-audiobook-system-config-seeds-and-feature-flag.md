# Audiobook System Config Seeds and Feature Flag

## Status

`done`

## Description

Seed the three audiobook system config entries and the `AUDIOBOOK_ENABLED` feature flag in `Program.cs` so the feature is self-configuring on first run. Mirrors the pattern used for the weather provider seeds.

## Acceptance Criteria

- [ ] Three `SystemConfig` entries are seeded in `Program.cs` (alongside the existing weather seeds):
  - `audiobook::provider` — type `select`, default value `mock`, options: `{ Value: "mock", Label: "Mock" }` and `{ Value: "gpu-service", Label: "GPU Service" }`
  - `audiobook::url` — type `string`, default value `""` (empty), `IsSecret: false`
  - `audiobook::api_key` — type `string`, default value `""` (empty), `IsSecret: true`
- [ ] `AUDIOBOOK_ENABLED` feature flag is seeded as disabled (`false`) by default
- [ ] All three config entries appear in the System Config admin UI without any manual setup
- [ ] The feature flag appears in the Feature Flags admin UI and can be toggled

## Notes

The seed data lives in `Program.cs` in the section where weather config is seeded. Follow the same `SystemConfig` / `SystemConfigOption` model structure used for `weather::provider`.

The `IsSecret: true` flag on `audiobook::api_key` ensures the value is masked in the System Config UI, consistent with how `weather::api_key` is handled.
