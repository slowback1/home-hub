# Implement Undo Toast

## Status

`pending`

## Description

After a successful task completion, show a toast notification with an "Undo" action. If the user clicks Undo before the toast expires (~5 seconds), call `DELETE /api/tasks/{id}/completions/latest` and restore the task to its pre-completion state in the list.

## Acceptance Criteria

- [ ] A toast appears immediately after a successful completion with a message (e.g. "Task completed") and an "Undo" button
- [ ] The toast auto-dismisses after ~5 seconds with no further action
- [ ] Clicking "Undo" within the window calls `DELETE /api/tasks/{id}/completions/latest`
- [ ] On successful undo: a one-off task reappears in the Due section; a recurring task returns to Due with its original `doDate`
- [ ] On successful undo, the toast is dismissed
- [ ] If the undo API call fails, an error message is shown and the list state is not corrupted
- [ ] Only one undo toast is active at a time — completing a second task while a toast is pending dismisses the first toast without undoing it

## Notes

- Use the existing Toast component from `$lib/ui/containers/toast`
- The pre-completion task snapshot captured in task 09 is used to restore list state on undo (avoid re-fetching the full list)
- "Only one toast at a time" prevents confusing multi-undo scenarios; the prior completion is simply committed if a new one arrives
