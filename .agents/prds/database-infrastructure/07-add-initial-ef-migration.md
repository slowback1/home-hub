# Add Initial EF Migration

## Status

`done` <!-- pending | in-progress | done -->

## Description

Generate the `InitialCreate` EF Core migration covering `AppUser` and `ExampleData`, and commit the resulting migration files to the repository. This migration is what `MigrateAsync()` (added in task 08) will apply on first startup.

## Acceptance Criteria

- [ ] Migration files exist at `backend/EntityFramework/Migrations/` (`InitialCreate` snapshot + migration class)
- [ ] Migration creates tables for `AppUser` and `ExampleData` with correct column types and a primary key on `Id`
- [ ] `dotnet build` passes with no errors after migration files are added

## Notes

- Run from the `backend/` directory:
  ```
  dotnet ef migrations add InitialCreate \
    --project EntityFramework \
    --startup-project WebAPI
  ```
- The `WebAPI` project must already have a valid connection string in `appsettings.json` (added in task 07) for the design-time tooling to resolve `AppDbContext`. If task 07 is not yet done, add a temporary connection string or use `--no-build` workarounds — but the cleanest approach is to do task 07 first.
- Task ordering recommendation: complete tasks 06 and 07 (connection string config) before this task, or at minimum ensure `appsettings.json` has a `ConnectionStrings:DefaultConnection` value so `dotnet ef` can instantiate the context at design time.
- Do not hand-edit the generated migration files.
