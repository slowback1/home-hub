# Write & Verify E2E Scenarios

## Status

`done`

## Description

Write the `e2e/features/dashboard.feature` file from the PRD's Gherkin scenarios, stub a `DashboardPage` page object and step definitions, and confirm all 6 scenarios run RED — failing with meaningful "not implemented" errors rather than tooling or compilation failures.

## Acceptance Criteria

- [ ] `e2e/features/dashboard.feature` exists with all 6 `@dashboard` scenarios from the PRD, with domain tag `@dashboard` on the `Feature` line and a kebab-case slug tag on each `Scenario`
- [ ] `e2e/pages/DashboardPage.ts` exists with the page object methods needed by the step definitions (navigation, slot queries, modal interaction, edit mode)
- [ ] `e2e/steps/dashboard.steps.ts` exists with stubbed `Given`/`When`/`Then` matching the Gherkin; all stubs throw a `not implemented` error so failures are meaningful
- [ ] `Given`/`When`/`Then` are imported from `../fixtures`, not directly from `playwright-bdd`
- [ ] Running `task e2e:test` targeting `@dashboard` (e.g. `--grep @dashboard`) produces 6 failing scenarios — none fail due to a tooling, config, or compile error

## Notes

Tag conventions are in `e2e/AGENTS.md`. The Gherkin lives in the PRD's `E2E Scenarios` section. Page Object reference: `e2e/pages/LoginPage.ts`. Fixtures are extended in `e2e/fixtures.ts`.
