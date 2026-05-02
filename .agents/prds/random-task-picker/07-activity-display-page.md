# /activity/ Display Page

## Status

`pending` <!-- pending | in-progress | done -->

## Description

Build the `/activity/` SvelteKit route that serves as the main face of the feature. The page shows a hero card with the current hourly pick and a 7-day calendar week grid below it. It polls the API every 60 seconds so the display updates automatically when the hour turns. A settings icon links to `/activity/config`. When no pick exists yet, a placeholder message is shown instead.

## Acceptance Criteria

- [ ] SvelteKit route exists at `frontend/src/routes/activity/+page.svelte`
- [ ] `ActivityPickApi.ts` client exists in `frontend/src/lib/api/` and wraps `GET /api/activity/current` and `GET /api/activity/history`
- [ ] Hero card at the top shows the current activity name and the time it was picked (e.g. "picked at 2:00 PM"), displayed in the browser's local time zone
- [ ] When `GET /api/activity/current` returns 204 (no pick), the hero area shows a placeholder message (e.g. "Add some activities to get started") that links to `/activity/config`
- [ ] A settings icon (gear or equivalent) is visible on the page and navigates to `/activity/config`
- [ ] The week-view calendar grid renders 7 columns (days) × 24 rows (hours), covering the last 7 days including today
- [ ] Each cell shows the truncated activity name for that hour slot; hovering shows the full name as a tooltip
- [ ] Future hour cells are visually grayed out; the current hour cell is highlighted
- [ ] The page polls `GET /api/activity/current` every 60 seconds via `setInterval` and updates the hero card when a new pick is returned
- [ ] The "Coming soon" placeholder at `frontend/src/routes/activity/+page.svelte` (if it exists) is replaced by this implementation
- [ ] Page has Vitest unit tests covering the API client and key rendering states (empty, with pick, grid cell states)

## Notes

- UTC → local time conversion should happen in the Svelte component using `Date` methods or `Intl.DateTimeFormat`, not in the API client
- The 7-day history fetch (`GET /api/activity/history`) only needs to run once on mount; only the current pick needs polling
- Clear the `setInterval` in the component's `onDestroy` to prevent memory leaks
- Grid implementation tip: generate an array of the last 7 days × 24 hours, then index the history response by `(day, hour)` to fill cells
