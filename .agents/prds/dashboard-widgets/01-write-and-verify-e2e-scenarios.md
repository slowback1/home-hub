# Write & verify E2E scenarios (RED)

## Status

`done`

## Description

Write the Gherkin feature file for dashboard widgets and stub out the step definitions and page objects so all 10 new scenarios are discovered by the test runner and fail with a meaningful "not implemented" error — not a compilation or tooling failure. This confirmed-RED state gates all subsequent implementation tasks.

## Acceptance Criteria

- [ ] `e2e/features/dashboard-widgets.feature` exists with all 10 scenarios from the PRD verbatim (tags `@dashboard-widgets` on Feature; kebab-case slug tags on each Scenario).
- [ ] `e2e/steps/dashboard-widgets.steps.ts` exists with stub implementations for every step used in the feature file; each stub throws a `not implemented` error or calls a `todo()` equivalent.
- [ ] Any new Page Object methods referenced by the steps are stubbed in `e2e/pages/DashboardPage.ts` (or a new page file if appropriate) and throw `not implemented`.
- [ ] `task e2e:test` runs without compilation or configuration errors.
- [ ] All 10 new scenarios appear in the test output and **fail** with a meaningful assertion or `not implemented` error (not a crash or missing-step error).

## Notes

- Feature file goes to `e2e/features/dashboard-widgets.feature`.
- Step definitions go to `e2e/steps/dashboard-widgets.steps.ts`.
- Follow the tag conventions in `e2e/AGENTS.md`: domain tag (`@dashboard-widgets`) on the `Feature` line; kebab-case slug tag on each `Scenario`.
- Several steps involve seeding backend state via the API (e.g. "Given there are due tasks in the system") — stub these with `todo()` for now; the real implementations land in task 10.
- The existing `Before`/`After` hooks in `e2e/steps/dashboard.steps.ts` reset dashboard layout before/after each `@dashboard` scenario — consider whether a similar hook is needed here (likely yes, tag it `@dashboard-widgets`).
- Reference PRD: `.agents/prds/dashboard-widgets/dashboard-widgets.md` § E2E Scenarios.
