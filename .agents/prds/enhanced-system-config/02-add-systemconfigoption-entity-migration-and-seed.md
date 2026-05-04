# Add SystemConfigOption Entity, EF Migration, and Seed Data

## Status

`done`

## Description

Introduce the `SystemConfigOption` model, wire it into `AppDbContext`, generate the EF migration, and seed all initial data: update `Type` on existing weather rows, create the `SystemConfigOptions` table, and seed `weather::provider` as the first `select`-type entry with its two options. The `DictionarySystemConfigProvider` is also pre-seeded with `weather::provider` so the E2E environment has a select-type field to work with.

## Acceptance Criteria

- [ ] `SystemConfigOption` entity exists in the correct backend project with properties: `SystemConfigId` (string, FK to `SystemConfig.Id`), `Value` (string), `Label` (string)
- [ ] `AppDbContext` includes a `SystemConfigOptions` `DbSet` with the FK relationship configured
- [ ] EF migration generated via `dotnet ef migrations add` (never hand-written) that:
  - Creates the `SystemConfigOptions` table
  - Sets `Type = "text"` on `weather::zip_code`
  - Sets `Type = "secret"` on `weather::api_key`
  - Inserts `weather::provider` into `SystemConfigs` with `Type = "select"`, `Value = "mock"`, `IsSecret = false`
  - Inserts two rows into `SystemConfigOptions` for `weather::provider`: `{ Value = "mock", Label = "Mock" }` and `{ Value = "openweathermap", Label = "Open Weather Map" }`
- [ ] `DictionarySystemConfigProvider` pre-seeds `weather::provider` as a select entry with the same two options (for E2E)
- [ ] `dotnet build` passes with no errors or warnings

## Notes

Use `dotnet ef migrations add AddSystemConfigOptions --project backend/EntityFramework --startup-project backend/WebAPI` (see CLAUDE.md).

After generating, it is fine to edit the `.cs` migration file to add the seed `INSERT` statements — just never hand-write the `.Designer.cs` or snapshot.

`weather::units` will be seeded by the Weather Widget PRD — do not include it here.

The `Type` column already exists on `SystemConfig` (currently empty string for all rows) — no column add needed, only the data updates.
