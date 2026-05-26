# Wire E2E to GREEN

## Status

`done`

## Description

Implement the full step definitions and page object methods for the 10 dashboard-widget E2E scenarios stubbed in task 01. Run the complete suite and confirm every new scenario passes.

## Acceptance Criteria

- [ ] All step definitions in `e2e/steps/dashboard-widgets.steps.ts` are fully implemented (no `not implemented` stubs remaining).
- [ ] All page object methods referenced by the steps are implemented.
- [ ] `task e2e:test` runs without errors and all 10 `@dashboard-widgets` scenarios are **GREEN**.
- [ ] No previously-passing scenarios have been broken.

## Notes

- Scenarios to cover (from `e2e/features/dashboard-widgets.feature`):
  - `@weather-widget-shows-conditions` — seed weather via provider config; assert temperature, condition, humidity, wind speed visible in the widget.
  - `@tasks-widget-shows-due-tasks` — seed tasks with `doDate <= today` via `POST /api/tasks`; assert names appear in the widget.
  - `@tasks-widget-mark-done` — seed one due task; click Done in the widget; assert undo toast appears.
  - `@tasks-widget-overflow` — seed 7 due tasks; assert 5 rows visible and "and 2 more" text present.
  - `@activity-widget-shows-current-pick` — ensure a current pick exists (may require a pick endpoint call or seeded data); assert activity name visible.
  - `@audiobook-widget-active-job` — seed an in-progress job via backend API; assert filename and status badge visible in the widget.
  - `@audiobook-widget-no-jobs` — ensure no jobs exist; assert "No conversions yet" visible.
  - `@bookmarks-widget-starred` — seed a starred bookmark named "GitHub"; assert it appears as a link.
  - `@bookmarks-widget-fallback` — seed unstarred bookmarks; assert bookmark links appear.
  - `@widget-error-state` — disable the weather feature flag or mock a failure; assert error state visible in the widget.
- Look at existing step files (`dashboard.steps.ts`, `weather-widget.steps.ts`) for patterns on seeding data via `request` and using page objects.
- The `Before`/`After` hooks for `@dashboard-widgets` (from task 01) should clean up any seeded data (tasks, bookmarks, audiobook jobs) to keep tests isolated.
- `data-testid` attributes on widget body elements will likely need to be added during widget implementation tasks (05–09) — if they're missing when you reach this task, add them to the widget components.
- Backend URL for direct API calls: `http://localhost:5273`.
