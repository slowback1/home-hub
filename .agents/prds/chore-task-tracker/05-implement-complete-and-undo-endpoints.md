# Implement Complete and Undo Endpoints

## Status

`pending`

## Description

Add `POST /api/tasks/{id}/complete` and `DELETE /api/tasks/{id}/completions/latest` to the task controller. The complete endpoint writes a `TaskCompletion` log entry and either advances `DoDate` (recurring) or stamps `CompletedAt` (one-off). The undo endpoint deletes the latest completion and reverts the task to its prior state.

## Acceptance Criteria

- [ ] `POST /api/tasks/{id}/complete` writes a `TaskCompletion` record with `CompletedAt = now` and `PreviousDoDate = task.DoDate` (before update)
- [ ] For recurring tasks: `DoDate` is updated to `IRecurrenceCalculator.GetNextDoDate(task, now)`; `CompletedAt` remains null
- [ ] For one-off tasks: `CompletedAt` is stamped with the current time; `DoDate` is unchanged
- [ ] `POST /api/tasks/{id}/complete` returns HTTP 404 if task not found
- [ ] `DELETE /api/tasks/{id}/completions/latest` deletes the most recent `TaskCompletion` for the task (by `CompletedAt` descending)
- [ ] For recurring tasks: restores `Task.DoDate` from `TaskCompletion.PreviousDoDate`
- [ ] For one-off tasks: clears `Task.CompletedAt`
- [ ] `DELETE /api/tasks/{id}/completions/latest` returns HTTP 404 if task not found or no completions exist
- [ ] Integration tests cover: complete one-off, complete recurring, undo one-off, undo recurring, 404 cases
- [ ] `dotnet test` passes

## Notes

- `IRecurrenceCalculator` is injected via DI — constructor-inject it into the controller
- The `PreviousDoDate` field on `TaskCompletion` is what makes undo safe for future non-interval rule formats
- `TaskCompletion` does not go through `ICrud<T>` if the generic CRUD pattern doesn't support FK queries well — a direct `AppDbContext` query or a dedicated repository is fine
