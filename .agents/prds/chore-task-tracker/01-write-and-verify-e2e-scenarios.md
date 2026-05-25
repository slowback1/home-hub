# Write & Verify E2E Scenarios

## Status

`in-progress`

## Description

Write the Gherkin feature file for the Chore / Task Tracker using the scenarios defined in the PRD, then stub out the step definitions and page object so all scenarios are discovered but fail with a meaningful "not implemented" error. This confirmed-RED state is the acceptance gate for this task.

## Acceptance Criteria

- [ ] `e2e/features/tasks.feature` exists with all 9 scenarios from the PRD, tagged with `@tasks` on the Feature line and individual slug tags on each Scenario
- [ ] `e2e/steps/tasks.steps.ts` exists with stubbed step definitions (each throws `not implemented` or calls `todo()`)
- [ ] `e2e/pages/TasksPage.ts` exists as a stub Page Object
- [ ] `task e2e:test` runs without tooling/compilation errors
- [ ] All 9 new scenarios FAIL with a meaningful test assertion or `not implemented` error (not a config or import error)

## Notes

- Feature file path: `e2e/features/tasks.feature`
- Step definitions: `e2e/steps/tasks.steps.ts`
- Page object: `e2e/pages/TasksPage.ts`
- Import `Given`, `When`, `Then` from `../fixtures` (not from `playwright-bdd` directly) — see `e2e/AGENTS.md`
- The `/` character is treated as an alternation separator in Cucumber expressions — avoid it in step text
- The page object must be added to the `Fixtures` type and the `base.extend` call in `e2e/fixtures.ts`
