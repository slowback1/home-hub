# Seed Default Weather Config via EF Migration

## Status

`pending`

## Description

Add an EF data migration that inserts the two weather config entries into the `system_config` table as default seed data. These entries are the config keys the weather widget feature will consume, and they need to exist in the database for production and dev environments.

## Acceptance Criteria

- [ ] A new EF migration exists that inserts `weather/zip_code` (value: `"10001"`, `IsSecret: false`) and `weather/api_key` (value: `""`, `IsSecret: true`)
- [ ] Running `dotnet ef database update` applies the migration without error
- [ ] The migration is idempotent — re-running does not insert duplicates
- [ ] Existing integration tests continue to pass

## Notes

Use the `Namespace`, `Key`, `Value`, `IsSecret`, and `Type` columns already defined by the `AddSystemConfigTable` migration. `Type` can be left as empty string.

The `Id` column should follow whatever ID scheme `EfSystemConfigProvider` uses (check the existing provider and model for the expected format).

`weather/api_key` is seeded with an empty value intentionally — a real key will be set via the admin UI at runtime.
