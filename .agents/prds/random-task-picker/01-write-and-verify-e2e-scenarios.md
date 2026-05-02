# Write & Verify E2E Scenarios

## Status

`pending` <!-- pending | in-progress | done -->

## Description

Write the Gherkin feature file for the Activity Picker using the scenarios defined in the PRD. Stub out step definitions and page objects so the tests are discovered by the test runner but fail with a meaningful "not implemented" error. The goal of this task is a confirmed-RED state — all 5 scenarios must be found and failing before implementation begins.

## Acceptance Criteria

- [ ] `e2e/features/activity.feature` exists and contains all 5 scenarios from the PRD verbatim (tags: `@activity` on Feature, individual slug tags on each Scenario)
- [ ] `e2e/steps/activity.steps.ts` exists with stubbed `Given`/`When`/`Then` implementations that throw a `not implemented` error or call `todo()`
- [ ] Page objects exist at `e2e/pages/ActivityPage.ts` and `e2e/pages/ActivityConfigPage.ts` with stubbed methods
- [ ] New page objects and fixtures are wired into `e2e/fixtures.ts`
- [ ] `task e2e:test` runs without tooling, compilation, or configuration errors
- [ ] All 5 `@activity` scenarios fail with a meaningful test/assertion error (not a setup error)

## Notes

- Import `Given`, `When`, `Then` from `../fixtures`, not directly from `playwright-bdd`
- The `/` character is reserved in Cucumber Expressions — avoid it in step text
- Step stubs should look like: `Given('I am on the activity config page', async () => { throw new Error('not implemented') })`
- The two display-page scenarios (`@activity-empty-state`, `@activity-display-current-pick`) will require DB seeding — stub those steps the same way; seeding is implemented in task 08
