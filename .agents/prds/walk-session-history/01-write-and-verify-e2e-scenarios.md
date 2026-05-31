# Write & Verify E2E Scenarios

## Status

`pending`

## Description

Write the Gherkin feature file for Walk Session History and stub out the corresponding step definitions and page object so the scenarios are discovered by the test runner but fail with a meaningful "not implemented" error. The confirmed-RED state is the acceptance gate for this task.

## Acceptance Criteria

- [ ] `e2e/features/walk-history.feature` exists with all 5 scenarios from the PRD, tagged `@walk-history` on the Feature line and individual slug tags on each Scenario
- [ ] `e2e/steps/walk-history.steps.ts` exists with stubs for every Given/When/Then step, each throwing a `not implemented` error (or using `todo()`)
- [ ] `e2e/pages/WalkHistoryPage.ts` exists with a stub Page Object (constructor + placeholder methods)
- [ ] `task e2e:test` runs without tooling or compilation errors
- [ ] All 5 `@walk-history` scenarios appear in the test output and fail with a meaningful assertion or "not implemented" error — not a config or import error

## Notes

- Feature file scenarios are defined in the PRD's `E2E Scenarios` section — copy them verbatim.
- Import `Given`, `When`, `Then` from `../fixtures` (not from `playwright-bdd` directly) — see `e2e/AGENTS.md`.
- The `/` character is a reserved alternation separator in Cucumber Expressions; avoid it in step text.
- Page Object goes in `e2e/pages/WalkHistoryPage.ts` following the pattern of e.g. `TasksPage.ts`.
- Do not wire the page object into `fixtures.ts` yet — that belongs in task 09.
