# HomeHub — Claude Guidelines

## EF Core Migrations

**Always use `dotnet ef` to add migrations. Never write them by hand.**

```bash
export PATH="$PATH:/home/claude/.dotnet/tools"
# Install if missing:
dotnet tool install --global dotnet-ef

dotnet ef migrations add <MigrationName> --project backend/EntityFramework --startup-project backend/WebAPI
```

Manually written migrations cause `PendingModelChangesWarning` at startup because the snapshot format EF Core produces internally doesn't match what a human writes. The generated `.Designer.cs` and `AppDbContextModelSnapshot.cs` must come from the tool.

After generating, it's fine to edit the `.cs` migration file (e.g. to add `InsertData` seed rows) — just never hand-write the `.Designer.cs` or snapshot.
