# Write & Verify E2E Scenarios (RED)

## Status

`pending`

## Description

Write the Gherkin feature file for the four `@comfyui` scenarios defined in the PRD, then stub the corresponding step definitions and page objects so the tests are discovered but fail with a meaningful "not implemented" error. The goal is a confirmed-RED state — not a tooling, compilation, or configuration error.

## Acceptance Criteria

- [ ] `e2e/features/comfyui-text-to-image.feature` exists with all four scenarios: `@comfyui-add-workflow`, `@comfyui-delete-workflow`, `@comfyui-generate-happy-path`, `@comfyui-generate-error`
- [ ] `e2e/steps/comfyui-text-to-image.steps.ts` exists with stubbed Given/When/Then implementations that throw a `not implemented` error (or use `todo()`)
- [ ] `e2e/pages/ComfyUiPage.ts` and `e2e/pages/ComfyUiConfigPage.ts` exist as stub page objects
- [ ] Page objects and step file are wired into `e2e/fixtures.ts`
- [ ] `task e2e:test` targeting `@comfyui` runs without tooling or compilation errors
- [ ] All four scenarios FAIL with a meaningful assertion or "not implemented" error

## Notes

Scenario text and tags are in the PRD's `E2E Scenarios` section. Use the exact tags (`@comfyui` on the Feature line, `@comfyui-add-workflow` etc. on each Scenario line).

Step stubs should follow the pattern established in existing step files — import `Given`, `When`, `Then` from `../fixtures`, not from `playwright-bdd` directly.
