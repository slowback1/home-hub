# Implement Task CRUD Endpoints

## Status

`done`

## Description

Add a `ChoreTaskController` with standard CRUD endpoints for tasks. `GET /api/tasks` returns only active (non-completed) tasks. `POST`, `PUT`, and `DELETE` handle creation, updates, and hard deletion respectively. Integration tests cover all endpoints.

## Acceptance Criteria

- [ ] `GET /api/tasks` returns all tasks where `CompletedAt IS NULL`, with HTTP 200
- [ ] `POST /api/tasks` creates a task and returns it with HTTP 200; request body includes `Name`, `DoDate` (nullable), `IsRecurring`, `IntervalDays` (nullable)
- [ ] `POST /api/tasks` returns HTTP 400 if `Name` is empty
- [ ] `POST /api/tasks` returns HTTP 400 if `IsRecurring = true` and `IntervalDays` is null or < 1
- [ ] `PUT /api/tasks/{id}` updates `Name`, `DoDate`, `IsRecurring`, `IntervalDays` and returns the updated task; returns HTTP 404 if not found
- [ ] `DELETE /api/tasks/{id}` hard-deletes the task and all its `TaskCompletion` records; returns HTTP 204; returns HTTP 404 if not found
- [ ] `GET /api/tasks/history` exists and returns tasks where `CompletedAt IS NOT NULL` (stub for future history UI)
- [ ] Integration tests cover happy paths and key error cases for all endpoints
- [ ] `dotnet test` passes

## Notes

- Follow the `ActivityController` pattern: inject `ICrudFactory`, use `ICrud<ChoreTask>` for persistence
- For `DELETE`, cascade-delete `TaskCompletion` records either via EF FK cascade or explicit query before deleting the task
- Controller route: `[Route("api/tasks")]`
- `GET /api/tasks/history` can be a thin implementation for now — just filter on `CompletedAt IS NOT NULL` — since the history UI is out of scope for v1
