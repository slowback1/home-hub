# Implement Done Button

## Status

`pending`

## Description

Add a "Done" button to each row in the Due section. Clicking it calls `POST /api/tasks/{id}/complete` and optimistically removes the task from the Due list (or moves it to Upcoming if recurring). The undo toast is wired in the next task.

## Acceptance Criteria

- [ ] Each row in the Due section has a "Done" button
- [ ] Clicking "Done" calls `POST /api/tasks/{id}/complete`
- [ ] On success, a one-off task is removed from the Due list entirely
- [ ] On success, a recurring task moves from Due to the Upcoming section with its updated `doDate`
- [ ] If the API call fails, the task remains in the Due list and an error is surfaced to the user
- [ ] The "Done" button is disabled while the API call is in flight to prevent double-submission

## Notes

- The updated task returned by the complete endpoint should be used to update the list (rather than re-fetching all tasks)
- The undo state (task snapshot before completion) should be captured at this point so task 10 can use it — e.g. store the pre-completion task in a variable before calling the API
