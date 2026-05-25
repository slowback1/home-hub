# Add Task Modal with Add/Edit Form

## Status

`done`

## Description

Build a modal component with a shared Add/Edit form for tasks. The Add Task button on the page opens the modal in create mode. Each task row's edit icon opens it in edit mode pre-populated with the task's current values. Edit mode includes a Delete button. All actions (create, update, delete) call the API and update the reactive task list on success.

## Acceptance Criteria

- [ ] Clicking "Add Task" opens the modal in create mode with an empty form
- [ ] Form fields: Name (text, required), Do Date (date picker, optional), Recurring toggle, Interval Days (number input, shown only when recurring is toggled on)
- [ ] Submitting the create form calls `POST /api/tasks`, adds the new task to the correct section (Due or Upcoming), and closes the modal
- [ ] Clicking the edit icon on any task row opens the modal in edit mode with the task's values pre-filled
- [ ] Submitting the edit form calls `PUT /api/tasks/{id}`, updates the task in the list, and closes the modal
- [ ] Edit mode shows a Delete button; clicking it calls `DELETE /api/tasks/{id}`, removes the task from the list, and closes the modal
- [ ] The form validates that Name is non-empty before submitting
- [ ] The form validates that Interval Days is ≥ 1 when recurring is toggled on
- [ ] API errors display an inline error message within the modal
- [ ] The modal can be dismissed without saving (Escape key or close button)

## Notes

- There is no existing generic modal component in `$lib/ui/` — one will need to be built or the modal can be implemented inline in the page
- Changing `IntervalDays` in edit mode does NOT recalculate `DoDate` — the interval change applies to future completions only (per PRD decision)
- After a successful create, determine Due vs Upcoming by comparing the new task's `doDate` against today client-side
