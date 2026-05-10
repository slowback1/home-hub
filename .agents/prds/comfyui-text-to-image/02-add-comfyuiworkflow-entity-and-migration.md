# Add ComfyUiWorkflow Entity and EF Migration

## Status

`done`

## Description

Add the `ComfyUiWorkflow` entity to the EntityFramework project, register it in `AppDbContext`, and generate the migration using `dotnet ef`. This is the data foundation for all workflow storage and must land before the API or client layers.

## Acceptance Criteria

- [ ] `backend/EntityFramework/Entities/ComfyUiWorkflow.cs` exists with `Id` (int), `Name` (string), and `WorkflowJson` (string) properties
- [ ] Entity is registered as a `DbSet<ComfyUiWorkflow>` in `AppDbContext`
- [ ] Migration generated via `dotnet ef migrations add AddComfyUiWorkflow --project backend/EntityFramework --startup-project backend/WebAPI`
- [ ] Migration applies cleanly against a fresh database (`dotnet ef database update`)
- [ ] `AppDbContextModelSnapshot` is updated by the tool (not hand-written)

## Notes

Per CLAUDE.md: always use `dotnet ef` to generate migrations — never write them by hand. After generating, the `.cs` migration file may be edited (e.g. to add seed rows) but the `.Designer.cs` and snapshot must not be manually modified.

```bash
export PATH="$PATH:/home/claude/.dotnet/tools"
dotnet ef migrations add AddComfyUiWorkflow --project backend/EntityFramework --startup-project backend/WebAPI
```
