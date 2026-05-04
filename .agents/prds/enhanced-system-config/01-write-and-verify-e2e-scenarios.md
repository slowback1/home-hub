# Write & Verify E2E Scenarios

## Status

`done`

## Description

Write the Gherkin feature file and stub the step definitions for the Enhanced System Config scenarios. The goal of this task is a confirmed-RED state: all four new scenarios are discovered and fail with a meaningful "not implemented" error, not a tooling or compilation error.

## Acceptance Criteria

- [ ] `e2e/features/enhanced-system-config.feature` exists with all four scenarios from the PRD (tags: `@admin` on the Feature, kebab-case slug tags on each Scenario)
- [ ] `e2e/steps/enhanced-system-config.steps.ts` exists and stubs all new step definitions (throw `'not implemented'` or use `todo()`)
- [ ] Any new Page Object methods needed are stubbed on `e2e/pages/SystemConfigPage.ts`
- [ ] `task e2e:test` targeting the new feature runs without tooling/compilation errors
- [ ] All four new scenarios fail RED with a meaningful assertion or "not implemented" error

## Notes

The existing `@admin` tag and many shared steps already exist in `e2e/steps/system-config-ui.steps.ts`. New steps needed for this feature:
- `Then the {string} field in the {string} section should render as a dropdown`
- `And the dropdown should contain {string} and {string} as options`
- `When I change the {string} dropdown to {string}`
- `Then I should see a {string} section header`
- `Then I should see a {string} label in the {string} section`

Reuse existing steps where possible (e.g. `I navigate to the admin system config page`, `I should see a success toast`).

The `Before { tags: '@admin' }` hook in `system-config-ui.steps.ts` resets E2E seed data before each scenario — the new step file should not duplicate this hook.
