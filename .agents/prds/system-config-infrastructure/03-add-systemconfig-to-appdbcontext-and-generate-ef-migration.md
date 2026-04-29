# Add SystemConfig to AppDbContext and generate EF migration

## Status

`pending` <!-- pending | in-progress | done -->

## Description

Add `DbSet<SystemConfig>` to `AppDbContext` and generate the EF migration that creates the `system_config` table. This is a schema-only migration — no seed data rows are included here; individual features introduce their own config rows via subsequent migrations.

## Acceptance Criteria

- [ ] `AppDbContext` has a `DbSet<SystemConfig>` property
- [ ] A migration named `AddSystemConfigTable` exists under `EntityFramework/Migrations/`
- [ ] The migration creates a `system_config` (or `SystemConfigs`) table with columns matching the `SystemConfig` model: `Id`, `Namespace`, `Key`, `Value`, `Type`, `IsSecret`
- [ ] The migration applies cleanly against a fresh database (`task new-migration` or equivalent runs without error)
- [ ] `task test` passes with the new DbSet in place

## Notes

Use `task new-migration --name AddSystemConfigTable` (or the equivalent backend Taskfile task) to generate the migration after adding the `DbSet`.

The `Id` column is the primary key (`"{namespace}::{key}"`). No separate unique index on `(Namespace, Key)` is needed since the PK constraint already enforces uniqueness.
