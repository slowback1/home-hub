# Write & verify E2E scenarios (RED)

## Status

`done`

## Description

Add the Gherkin feature file and stub the step definitions and Page Object for the Wheels feature, then run the suite and confirm the new scenarios fail meaningfully. This establishes the confirmed-RED gate that the rest of the implementation drives to GREEN.

## Acceptance Criteria

- [ ] `e2e/features/wheels.feature` contains all 8 scenarios from the PRD's E2E Scenarios section, with the `@wheels` domain tag on the `Feature` line and a kebab-case slug tag on each `Scenario`.
- [ ] `e2e/steps/wheels.steps.ts` stubs every Given/When/Then, importing from `../fixtures`; each unimplemented step throws a `not implemented` error (or uses a `todo()`) so it fails meaningfully.
- [ ] `e2e/pages/WheelsPage.ts` exists as a Page Object stub, wired into `fixtures.ts` (plus any per-scenario state needed).
- [ ] `task e2e:test` discovers the new scenarios and they **FAIL** with a meaningful assertion / `not implemented` error — not a compilation, config, or tooling error.

## Notes

- Follow the conventions in `e2e/AGENTS.md`. Do not edit generated files under `e2e/tests/` (gitignored).
- Mirror the structure of the walk-history feature: `e2e/features/walk-history.feature`, `e2e/steps/walk-history.steps.ts`, `e2e/pages/WalkHistoryPage.ts`.
- Cucumber Expression note: `/` is an alternation separator — phrase step text to avoid literal slashes (e.g. "the Wheels page", not "/wheels").
- The `DELETE /api/test/wheels` cleanup endpoint used by the Before/After hooks is added in task 09; hooks should `.catch(() => {})` like walk-history so their absence doesn't error during this RED task.
- Backend URL in steps: `http://localhost:5273`. Feature flag: `WHEEL_PICKER_ENABLED`.
- Spin scenarios assert the result is **one of** the wheel's items (random pick — no deterministic single value).
