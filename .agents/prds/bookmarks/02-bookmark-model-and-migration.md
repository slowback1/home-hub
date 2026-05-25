# Bookmark Model + EF Core Migration

## Status

`pending`

## Description

Define the `Bookmark` entity class in the Common models project, register it with `AppDbContext`, and generate the EF Core migration. This is the foundation all backend and frontend tasks depend on.

## Acceptance Criteria

- [ ] `Bookmark` entity class exists with fields: `Id` (string GUID), `Name` (string), `Url` (string), `Description` (string?, nullable), `Starred` (bool, default `false`), `CreatedAt` (DateTime UTC)
- [ ] `Bookmark` is registered as a `DbSet` in `AppDbContext`
- [ ] Migration generated via `dotnet ef migrations add AddBookmarks --project backend/EntityFramework --startup-project backend/WebAPI`
- [ ] Migration applies cleanly (`dotnet ef database update`)
- [ ] No `PendingModelChangesWarning` at startup

## Notes

- Follow the existing entity conventions — see other model classes in `Common/Models/`
- Use `dotnet ef` CLI to generate the migration (never hand-write it); see CLAUDE.md for the exact command
- `CreatedAt` should be stored as UTC; use `DateTime.UtcNow` on creation and `DateTime.SpecifyKind(..., DateTimeKind.Utc)` if accepting values from outside to avoid the `Unspecified` kind error seen in task 6927850
