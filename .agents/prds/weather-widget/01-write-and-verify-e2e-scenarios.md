# Write & Verify E2E Scenarios

## Status

`pending`

## Description

Write the Gherkin feature file and stub the step definitions and page object for the Weather Widget scenarios. The goal of this task is a confirmed-RED state: all three new scenarios are discovered and fail with a meaningful "not implemented" error, not a tooling or compilation error.

## Acceptance Criteria

- [ ] `e2e/features/weather-widget.feature` exists with all three scenarios from the PRD (tag: `@weather` on the Feature, kebab-case slug tags on each Scenario)
- [ ] `e2e/steps/weather-widget.steps.ts` exists and stubs all step definitions (throw `'not implemented'` or use `todo()`)
- [ ] `e2e/pages/WeatherPage.ts` exists with stubbed methods for all page interactions
- [ ] `task e2e:test` targeting the new feature runs without tooling or compilation errors
- [ ] All three new scenarios fail RED with a meaningful assertion or "not implemented" error

## Notes

New steps needed (none exist yet for weather):
- `Given the weather provider is {string}`
- `When I navigate to the weather page`
- `When I navigate to the weather page and the weather API returns an error`
- `Then I should see a temperature value`
- `Then I should see a condition description`
- `Then I should see a humidity value`
- `Then I should see a wind speed value`
- `Then I should see a {string} message`
- `Given the {string} feature flag is disabled`
- `Then I should be redirected or see a not-found state`

The error-state scenario (`@weather-unavailable-state`) should use `cy.intercept` / Playwright route interception to mock `GET /api/weather/current` returning a 500 — no special backend changes required.

The feature-flag scenario (`@weather-feature-flag-hidden`) can toggle the flag via the existing `PUT /api/feature-flags/{name}` endpoint in a `Before` hook, and restore it after.
