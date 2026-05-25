# Build Tasks Page with Due and Upcoming Sections

## Status

`pending`

## Description

Replace the "Coming soon" stub at `/tasks` with a real page that fetches all active tasks on mount and splits them into two reactive sections: Due (DoDate is null or ≤ today) and Upcoming (DoDate > today). Task rows display the task name and, in the Upcoming section, the formatted DoDate.

## Acceptance Criteria

- [ ] `/tasks` page fetches tasks from `GET /api/tasks` on mount and handles load errors gracefully
- [ ] **Due section** shows tasks where `doDate` is null or ≤ today, sorted: past-due ascending by date first, then null-DoDate tasks by `createdAt` ascending
- [ ] **Upcoming section** shows tasks where `doDate` > today, sorted ascending by `doDate`
- [ ] Each row in the Due section displays the task name
- [ ] Each row in the Upcoming section displays the task name and formatted DoDate
- [ ] An "Add Task" button is visible on the page (can be a placeholder that does nothing yet — wired in task 08)
- [ ] Each task row has an edit icon/button (placeholder, wired in task 08)
- [ ] Page shows a loading state while fetching and an error message if the fetch fails
- [ ] If either section is empty, a short empty-state message is shown (e.g. "No tasks due")

## Notes

- Page path: `frontend/src/routes/tasks/+page.svelte`
- Use `TasksApi` from task 06 for data fetching
- Section split logic lives client-side: one `tasks` array, two derived arrays
- Today's date for the filter should use `new Date()` at render time — no need for a time-zone-aware solution in v1
- Recurring tasks should display their interval in a subtle way (e.g. "every 7 days") to distinguish them from one-offs — exact styling is up to implementation
