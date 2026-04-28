# Write & Verify E2E Scenarios

## Status

`done`

## Description

Write the Gherkin feature file for the design system app shell, stub the step definitions and any required Page Objects, then run the E2E suite targeting the new scenarios. The acceptance gate is confirmed-RED: all new scenarios must fail with meaningful test assertion or "not implemented" errors — not tooling, configuration, or compilation errors.

## Acceptance Criteria

- [ ] `e2e/features/design-system.feature` exists with all four scenarios from the PRD (`@sidebar-navigation`, `@sidebar-collapse`, `@sidebar-collapse-persists`, `@dark-theme-applied`)
- [ ] `e2e/steps/design-system.steps.ts` exists with stubbed step definitions (throwing `not implemented` or using `todo()`)
- [ ] Any required Page Objects exist as stubs in `e2e/pages/`
- [ ] `task e2e:test` runs without tooling or compilation errors
- [ ] All four new scenarios FAIL with meaningful assertion or "not implemented" errors (confirmed RED)

## Notes

Gherkin scenarios are defined in the PRD at `.agents/prds/ui-design-system/ui-design-system.md` under `E2E Scenarios`. Copy them verbatim into the feature file.

Domain tag: `@design-system`. Per `e2e/AGENTS.md`, import `Given`, `When`, `Then` from `../fixtures` (not from `playwright-bdd` directly).
