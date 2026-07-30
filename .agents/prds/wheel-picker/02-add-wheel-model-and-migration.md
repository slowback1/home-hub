# Add `Wheel` model, DbSet, and table migration

## Status

`done`

## Description

Introduce the `Wheel` entity and its database table. A wheel is a flat entity with its items stored as a single newline-delimited string, persisted via the existing generic `ICrud<Wheel>`.

## Acceptance Criteria

- [ ] `backend/Common/Models/Wheel.cs` implements `IIdentifyable` with `Id` (string), `Name` (string), `Items` (newline-delimited string), and `CreatedAt` (DateTime).
- [ ] `DbSet<Wheel> Wheels` is registered in `AppDbContext`.
- [ ] A migration adding the `Wheels` table is generated with `dotnet ef` (never hand-written); `.Designer.cs` and the snapshot come from the tool.
- [ ] An EF test confirms a `Wheel` round-trips through the context (create → query returns the same `Name`/`Items`), mirroring `WalkSessionTests`.
- [ ] Solution builds and the new EF test passes.

## Notes

- Mirror `backend/Common/Models/WalkSession.cs` for the model shape and `backend/EntityFramework.Tests/WalkSessionTests.cs` for the test.
- Migration command (per CLAUDE.md):
  `dotnet ef migrations add AddWheels --project backend/EntityFramework --startup-project backend/WebAPI`
- No child `WheelItem` table — items are a single newline-delimited column (PRD Key Decision). Item parsing/trimming lives in the frontend/controller, not the schema.
