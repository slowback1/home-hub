# Implement E2E steps & drive scenarios GREEN

## Status

`done`

## Description

Flesh out the stubbed step definitions and Page Object, add the test-cleanup endpoint the hooks depend on, and drive all Wheels E2E scenarios to passing.

## Acceptance Criteria

- [ ] `DELETE /api/test/wheels` is added to `TestHelperController` and deletes all wheels (used by the `@wheels` Before/After hooks).
- [ ] `e2e/steps/wheels.steps.ts` is fully implemented (no remaining `not implemented`/`todo` stubs), including flag enable/disable, seeding wheels via `POST /api/wheels`, placing the widget on the dashboard, and the manage/spin/empty/flag-gating assertions.
- [ ] `e2e/pages/WheelsPage.ts` implements the navigation and query helpers the steps use.
- [ ] Spin assertions verify the displayed result is **one of** the seeded wheel's items.
- [ ] `task e2e:test` runs and **all 8** new `@wheels` scenarios pass GREEN, with no regressions in the existing suite.

## Notes

- Mirror `e2e/steps/walk-history.steps.ts` (Before/After cleanup + flag patch via `PATCH /api/feature-flags/{FLAG}`, dashboard layout via `PUT /api/dashboard/layout` with `widgetType: 'wheels'`) and `TestHelperController` for the cleanup endpoint (`DELETE /api/test/walk-sessions` / `DELETE /api/test/dashboard`).
- This is the final task: it completes the RED stubs created in task 01.
- Depends on all prior tasks (backend API + page + widget must exist to satisfy the scenarios).
