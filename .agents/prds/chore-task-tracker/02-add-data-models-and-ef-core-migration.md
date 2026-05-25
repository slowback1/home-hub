# Add Data Models and EF Core Migration

## Status

`done`

## Description

Add the `ChoreTask` and `TaskCompletion` model classes to `Common/Models/`, register them in `AppDbContext`, and generate the EF Core migration. This establishes the schema all subsequent backend tasks depend on.

## Acceptance Criteria

- [ ] `ChoreTask` model exists in `Common/Models/ChoreTask.cs` with fields: `Id` (string), `Name` (string), `IsRecurring` (bool), `IntervalDays` (int?, nullable), `DoDate` (DateOnly?, nullable), `CompletedAt` (DateTime?, nullable), `CreatedAt` (DateTime)
- [ ] `TaskCompletion` model exists in `Common/Models/TaskCompletion.cs` with fields: `Id` (string), `TaskId` (string, FK), `CompletedAt` (DateTime), `PreviousDoDate` (DateOnly?, nullable)
- [ ] `AppDbContext` has `DbSet<ChoreTask>` and `DbSet<TaskCompletion>` properties
- [ ] FK relationship between `TaskCompletion.TaskId` and `ChoreTask.Id` is configured in `OnModelCreating`
- [ ] EF Core migration generated via `dotnet ef migrations add` (not hand-written)
- [ ] `dotnet build` passes with no warnings about pending model changes

## Notes

- Use `dotnet ef migrations add AddChoreTasks --project backend/EntityFramework --startup-project backend/WebAPI` (see CLAUDE.md)
- Name the model class `ChoreTask` (not `Task`) to avoid collision with `System.Threading.Tasks.Task`
- `ChoreTask` should implement `IIdentifyable` (see `Common/Models/Activity.cs` for reference)
- After generating the migration, it is fine to inspect the `.cs` file but do not hand-edit the `.Designer.cs` or snapshot
