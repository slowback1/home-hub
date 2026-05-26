# Implement TasksWidget

## Status

`pending`

## Description

Replace the `TasksWidget.svelte` placeholder with a real implementation that fetches the task list, filters to due tasks, renders up to 5 rows with a compact "Done" button per row (using `TaskCompletionService`), shows an overflow count when more than 5 are due, and polls every hour.

## Acceptance Criteria

- [ ] On mount, the widget calls `TasksApi.listTasks()` and shows a spinner during the request.
- [ ] Due task filtering: include tasks where `doDate <= today` or `doDate` is null. Sort results by task name alphabetically.
- [ ] Display at most 5 task rows. When the due list exceeds 5, show a muted overflow line below the list: `and N more…` where N is the count beyond 5.
- [ ] Each row shows the task name and a pill-shaped uppercase "DONE" micro-button. Clicking the button:
  - Calls `TaskCompletionService.completeTask(task)`.
  - Optimistically removes the task from the visible due list.
  - Fires an undo toast (via `ToastService` through `TaskCompletionService`) with an "Undo" button valid for 5 seconds.
- [ ] Marking done and undoing both work correctly without navigating away from the dashboard.
- [ ] On API error, the widget shows the shared error state: muted `—` and italic "Unavailable".
- [ ] The widget polls `TasksApi.listTasks()` every 1 hour; the interval is cleared on destroy.

## Notes

- Depends on task 03 (`ToastService` action callback) and task 04 (`TaskCompletionService`).
- `TasksApi` is at `src/lib/api/TasksApi.ts`; `listTasks()` returns `ChoreTask[]` with `{ id, name, isRecurring, intervalDays, doDate, completedAt, createdAt }`.
- "Due today": `new Date(task.doDate) <= today()` or `task.doDate === null`. Match the same `isDue` logic used in `tasks/+page.svelte`.
- Done button visual: compact pill, uppercase "DONE" label — matches the design handoff. On click + optimistic removal, if the task reappears after undo it should slot back into the sorted list.
- 1-hour interval in ms: `3_600_000`.
- Match the design: tight row padding (~5px), no per-row borders, overflow label without a divider.
