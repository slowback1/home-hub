# WalkSession Backend Model + EF Migration

## Status

`pending`

## Description

Add the `WalkSession` model to the HomeHub backend, register it with EF Core, and generate the database migration. This is the foundational data layer that the controller (task 02) and the future RPG integration depend on.

## Acceptance Criteria

- [ ] `WalkSession.cs` exists in `Common/Models/` with fields: `Id` (string), `ClientId` (string), `StartedAt` (DateTime), `DurationSeconds` (int), `StepCount` (int), `SyncedAt` (DateTime)
- [ ] `WalkSession` implements `IIdentifyable` (consistent with other models)
- [ ] `AppDbContext` has a `DbSet<WalkSession> WalkSessions` property
- [ ] A migration named `AddWalkSessions` exists under `EntityFramework/Migrations/`, generated via `dotnet ef` (not hand-written)
- [ ] The backend starts without a `PendingModelChangesWarning`

## Notes

Generate the migration with:
```bash
export PATH="$PATH:/home/claude/.dotnet/tools"
dotnet ef migrations add AddWalkSessions --project backend/EntityFramework --startup-project backend/WebAPI
```

`Id` is the server-assigned GUID (string, like all other models). `ClientId` is the Android-assigned GUID used for deduplication — it should have a database index to make the idempotency lookup in task 02 fast. After generating the migration it's fine to add `CreateIndex` for `ClientId` manually to the `.cs` migration file, but do not hand-write the `.Designer.cs` or snapshot.
