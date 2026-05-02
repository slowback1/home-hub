# /activity/config Management Page

## Status

`done` <!-- pending | in-progress | done -->

## Description

Build the `/activity/config` SvelteKit route where the user manages their activity list. The page displays all activities in a table with inline weight editing and per-row delete. A persistent add-new row at the bottom lets the user create activities. This page is not in the sidebar — it is reached only via the settings icon on the display page (added in task 07).

## Acceptance Criteria

- [ ] SvelteKit route exists at `frontend/src/routes/activity/config/+page.svelte`
- [ ] `ActivityApi.ts` client exists in `frontend/src/lib/api/` and wraps all four CRUD endpoints from task 03
- [ ] Page loads and displays all existing activities on mount
- [ ] Each activity row shows the activity name and a segmented 1–5 weight button row (styled like pagination controls); the current weight is highlighted
- [ ] Clicking a weight button immediately saves the new weight via `PUT /api/activities/{id}` and shows a success toast on completion
- [ ] Each row has a delete icon; clicking it hard-deletes the activity via `DELETE /api/activities/{id}` and removes the row (with toast feedback)
- [ ] An add-new row at the bottom has a text input for the activity name and a weight selector (default weight 1); submitting creates the activity via `POST /api/activities` and adds it to the list
- [ ] Loading spinner shown while fetching; error state shown if the request fails
- [ ] Page has Vitest unit tests covering the API client and key interactions

## Notes

- Follow the pattern established by `frontend/src/routes/admin/system-config/+page.svelte` for loading, inline editing, toast feedback, and error states
- Weight buttons: a row of 5 buttons labelled 1–5; selected weight gets a distinct highlight color; no separate "save" button — clicking saves immediately
- The route to this page from the display page is wired in task 07; this task does not need to add the settings icon
