# Write & Verify E2E Scenarios

## Status

`done`

## Description

Write the Gherkin feature file for the System Config Admin UI and stub the corresponding step definitions and page object. The goal of this task is to land confirmed-RED tests — all 5 scenarios must be discovered and fail with a meaningful `not implemented` error, not a tooling or compilation error.

## Acceptance Criteria

- [ ] `e2e/features/system-config-ui.feature` exists with all 5 scenarios from the PRD (tagged `@admin` + individual slug tags)
- [ ] `e2e/steps/system-config-ui.steps.ts` exists with all steps stubbed to throw a `not implemented` error
- [ ] `e2e/pages/SystemConfigPage.ts` exists with navigation and interaction methods stubbed
- [ ] `task e2e:test` runs without tooling, compilation, or configuration errors
- [ ] All 5 scenarios fail with a meaningful assertion or `not implemented` error (confirmed RED)

## Notes

Scenarios to implement (from PRD `E2E Scenarios` section):
- `@system-config-page-loads`
- `@system-config-edit-happy-path`
- `@system-config-edit-cancel`
- `@system-config-secret-masked`
- `@system-config-save-error`

Follow tag conventions from `e2e/AGENTS.md`: domain tag `@admin` on the `Feature` line, kebab-case slug tag on each `Scenario` line.

Import `Given`, `When`, `Then` from `../fixtures` (not from `playwright-bdd` directly). If a new fixture is needed for `SystemConfigPage`, extend `fixtures.ts`.

The `/` character is treated as an alternation separator in Cucumber expressions — avoid it in step text.
