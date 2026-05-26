# Extract TaskCompletionService

## Status

`done`

## Description

Create `src/lib/services/Tasks/TaskCompletionService.ts` to encapsulate the complete-a-task / undo-completion flow: calling the API, maintaining a 5-second undo window, and firing the undo toast via `ToastService`. Refactor `tasks/+page.svelte` to import and use this service instead of its current inline implementation, so `TasksWidget` can reuse the same logic without duplication.

## Acceptance Criteria

- [ ] `src/lib/services/Tasks/TaskCompletionService.ts` exists and exports a class or composable that provides at minimum: `completeTask(task)` (calls `TasksApi.completeTask`, fires undo toast) and handles the undo callback (calls `TasksApi.undoCompletion`, updates task state).
- [ ] The service uses `ToastService.AddToast` with an `action: { label: 'Undo', onClick: ... }` for the undo toast (depends on task 03).
- [ ] `tasks/+page.svelte` uses `TaskCompletionService` for task completion and undo — the inline `handleDone`, `handleUndo`, `undoTask`, `undoTimer`, and `UNDO_TIMEOUT_MS` logic is removed from the page component.
- [ ] Unit tests for `TaskCompletionService` cover: successful completion + toast fired, undo within window, undo after window (no-op or graceful).
- [ ] All existing task page behaviour is preserved (complete, undo, error handling).

## Notes

- Depends on task 03 (`ToastService` action callback).
- The existing undo timeout on the tasks page is 5000 ms (`UNDO_TIMEOUT_MS = 5000`); preserve this in the service.
- The service needs to update the caller's local task list after completion and undo — decide whether to accept a reactive list as a parameter or return update events/callbacks. Keep it simple; a callback pattern (`onComplete`, `onUndo`) is fine.
- `TasksWidget` (task 09) will import and use this service — design the API with both the full page and the widget as consumers in mind.
- Place unit test at `src/lib/services/Tasks/TaskCompletionService.spec.ts`.
