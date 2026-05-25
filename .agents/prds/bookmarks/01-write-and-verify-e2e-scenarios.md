# Write & Verify E2E Scenarios

## Status

`pending`

## Description

Write the Gherkin feature file for the bookmarks domain and stub out the step definitions and page objects so the new scenarios are discovered by the test runner but fail with meaningful "not implemented" errors. The goal is a confirmed RED state — not tooling or compilation failures.

## Acceptance Criteria

- [ ] `e2e/features/bookmarks.feature` exists with all 9 scenarios from the PRD's E2E Scenarios section
- [ ] `e2e/steps/bookmarks.steps.ts` exists and is wired to the fixture/test instance (imports `Given`, `When`, `Then`, `Before`, `After` from `../fixtures`)
- [ ] A `Before({ tags: '@bookmarks' })` hook is stubbed that will enable `BOOKMARKS_ENABLED` and clear test data (can call placeholder functions for now)
- [ ] An `After({ tags: '@bookmarks' })` hook is stubbed to restore state
- [ ] Any required Page Objects (e.g. `e2e/pages/BookmarksPage.ts`) exist with stubbed methods that throw `Error('not implemented')`
- [ ] `task e2e:test` runs without compilation or configuration errors
- [ ] All 9 new scenarios FAIL with a meaningful assertion or "not implemented" error — not a tooling error

## Notes

- Feature file path: `e2e/features/bookmarks.feature`
- Step definitions: `e2e/steps/bookmarks.steps.ts`
- Tag conventions: domain tag `@bookmarks` on the `Feature` line; slug tag on each `Scenario` line (e.g. `@add-bookmark-happy-path`)
- See `e2e/steps/audiobook.steps.ts` for the Before/After feature-flag pattern: `PATCH /api/feature-flags/BOOKMARKS_ENABLED` with `{ isEnabled: true/false }` and `DELETE /api/test/bookmarks`
- The `PATCH` and `DELETE` endpoints don't exist yet — the Before/After hooks can be written now and will start working once tasks 03 and 04 are complete
- Backend URL constant: `http://localhost:5273`
