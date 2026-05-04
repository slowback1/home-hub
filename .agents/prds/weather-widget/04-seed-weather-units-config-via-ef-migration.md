# Seed weather::units Config via EF Migration

## Status

`done`

## Description

Generate an EF migration that seeds `weather::units` as a `select`-type SystemConfig entry with imperial and metric options. Also pre-seed the `DictionarySystemConfigProvider` with `weather::units` for the E2E environment.

## Acceptance Criteria

- [ ] EF migration generated via `dotnet ef migrations add` (never hand-written) that inserts:
  - `weather::units` into `SystemConfigs` with `Type = "select"`, `Value = "imperial"`, `IsSecret = false`
  - Two rows into `SystemConfigOptions` for `weather::units`: `{ Value = "imperial", Label = "Imperial (°F, mph)" }` and `{ Value = "metric", Label = "Metric (°C, km/h)" }`
- [ ] `DictionarySystemConfigProvider` pre-seeds `weather::units` as a select entry with the same two options and default value `"imperial"` (for E2E)
- [ ] `dotnet build` passes with no errors or warnings
- [ ] `GET /api/system-config` includes `weather::units` with `type = "select"` and both options in its response

## Notes

Use `dotnet ef migrations add SeedWeatherUnitsConfig --project backend/EntityFramework --startup-project backend/WebAPI` (see CLAUDE.md). After generating, edit the `.cs` migration file to add the seed `INSERT` statements — never hand-write `.Designer.cs` or the snapshot.

`weather::provider` was already seeded by the Enhanced System Config PRD (task 02 of that PRD) — do not re-seed it here.

This task has no frontend changes. The `weather::units` dropdown will render automatically on the admin page once the Enhanced System Config PRD is complete and the entry carries `Type = "select"`.
