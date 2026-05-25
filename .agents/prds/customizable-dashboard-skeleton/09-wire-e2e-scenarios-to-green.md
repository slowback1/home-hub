# Wire E2E Scenarios to GREEN

## Status

`done`

## Description

Complete the `DashboardPage` page object and all step definitions stubbed in task 01. Run `task e2e:test` targeting `@dashboard` and confirm all 6 scenarios pass GREEN.

## Acceptance Criteria

- [ ] `DashboardPage` exposes concrete async methods for: navigating to `/`, counting empty slot placeholders, clicking the add-widget button on a specific slot, interacting with the picker modal (open check, widget selection), clicking "Edit Dashboard" and "Done", clicking a remove button on a specific slot, and reloading the page
- [ ] All 6 `@dashboard` step definitions are fully implemented with no `todo()` or "not implemented" throws remaining
- [ ] `task e2e:test` with `--grep @dashboard` exits 0 with all 6 scenarios GREEN
- [ ] No existing passing E2E scenarios are broken

## Notes

Run from the `e2e/` directory or repo root via `task e2e:test`. The `@dashboard-widget-persists` scenario requires an API reset between runs — check whether an existing test-helper endpoint (e.g. `TestHelperController`) can seed/clear dashboard layout state, or add one. Confirm `DashboardPage.ts` is registered as a fixture in `e2e/fixtures.ts`.
