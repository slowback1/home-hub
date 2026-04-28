# Add FeatureFlags Table Migration

## Status

`pending` <!-- pending | in-progress | done -->

## Description

Create an EF Core migration that adds the `FeatureFlags` table to the PostgreSQL database with `Name` (string, primary key) and `IsEnabled` (bool) columns. The migration should also seed `DEMO_FEATURE_FLAG` as a disabled flag.

## Acceptance Criteria

- [ ] `AppDbContext` has a `DbSet<FeatureFlag> FeatureFlags` property registered
- [ ] A new EF Core migration exists that creates the `FeatureFlags` table with `Name` as the primary key and `IsEnabled` as a non-nullable bool
- [ ] The migration seeds a row `{ Name = "DEMO_FEATURE_FLAG", IsEnabled = false }`
- [ ] `dotnet ef database update` applies the migration without errors
- [ ] The `FeatureFlags` table exists in PostgreSQL after migration runs

## Notes

- `FeatureFlag` model already exists at `backend/Common/Models/FeatureFlag.cs` — no changes needed to the model itself
- `AppDbContext` is at `backend/EntityFramework/AppDbContext.cs` — add `DbSet<FeatureFlag>` here
- The existing `InitialCreate` migration is at `backend/EntityFramework/Migrations/20260418132028_InitialCreate.cs` — create a new migration, do not modify the existing one
- Auto-migration runs at startup via `db.Database.Migrate()` in `Program.cs` — no manual migration step needed in production
- `DEMO_FEATURE_FLAG` is the constant defined in `frontend/src/lib/services/FeatureFlag/FeatureFlags.ts`
