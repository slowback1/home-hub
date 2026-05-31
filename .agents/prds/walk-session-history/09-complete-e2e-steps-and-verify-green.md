# Complete E2E Step Implementations and Verify GREEN

## Status

`pending`

## Description

Implement all step definitions and the WalkHistoryPage page object so every `@walk-history` scenario passes. Wire the page object into `fixtures.ts` and seed any required test data. This task closes the RED→GREEN loop opened in task 01.

## Acceptance Criteria

- [ ] `e2e/pages/WalkHistoryPage.ts` has fully implemented navigation and assertion methods (no remaining stubs)
- [ ] `WalkHistoryPage` is wired into `e2e/fixtures.ts` (type declaration + `base.extend` entry)
- [ ] All step definitions in `e2e/steps/walk-history.steps.ts` are fully implemented
- [ ] `task e2e:test --grep @walk-history` passes with all 5 scenarios GREEN
- [ ] No existing `@dashboard-widgets` or other unrelated e2e scenarios are broken

## Notes

- Steps that need feature flag state (enabled/disabled) should follow the pattern in `e2e/steps/feature-flags-ui.steps.ts` or `dashboard-widgets.steps.ts` — check how other steps toggle flags programmatically or via the API.
- Steps that need seeded walk session data should POST to `GET /api/walk-sessions` (via the backend API) during test setup — see how other steps seed data (e.g. tasks, bookmarks).
- The widget scenario requires the walk history widget to be placed on the dashboard — follow the `Given the tasks widget is placed in slot 0` pattern in `dashboard-widgets.steps.ts`.
- Run `task e2e:test` (not just `task generate`) to confirm the full pipeline: generate → run → pass.
