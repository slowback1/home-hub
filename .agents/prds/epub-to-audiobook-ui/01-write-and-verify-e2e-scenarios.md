# Write & Verify E2E Scenarios

## Status

`pending`

## Description

Write the Gherkin feature file for the audiobook section using the scenarios defined in the PRD, then stub out the step definitions and Page Objects so every scenario is discovered by the test runner but fails with a clear "not implemented" error. Add a minimal fixture WAV file. The goal is a confirmed-RED state — all 7 new scenarios failing for the right reason (missing implementation, not tooling or config errors).

## Acceptance Criteria

- [ ] `e2e/features/audiobook.feature` exists with all 7 scenarios from the PRD, tagged `@audiobook` on the Feature line and individual slug tags on each Scenario line
- [ ] `e2e/steps/audiobook.steps.ts` exists with stub implementations for every step (each stub throws a `not implemented` error or calls `todo()`)
- [ ] `e2e/pages/AudiobookConvertPage.ts` and `e2e/pages/AudiobookVoiceSamplesPage.ts` exist as stubbed Page Objects
- [ ] `e2e/fixtures/sample.wav` exists (a minimal valid WAV file sufficient for upload tests)
- [ ] Running `task e2e:test` with `--grep @audiobook` results in all 7 scenarios **failing** with assertion or "not implemented" errors — not compilation errors, import errors, or bddgen errors

## Notes

Scenarios to include (from PRD `E2E Scenarios` section):
- `@audiobook-submit-job-happy-path`
- `@audiobook-cancel-queued-job`
- `@audiobook-download-completed-file`
- `@audiobook-failed-job-shows-error`
- `@audiobook-no-voice-samples-disables-form`
- `@audiobook-upload-voice-sample`
- `@audiobook-delete-voice-sample`

See `e2e/AGENTS.md` for tag conventions, step definition import paths (`from '../fixtures'` not from `playwright-bdd`), and how to extend `fixtures.ts` with new Page Objects.

A minimal WAV file must be a valid WAV (RIFF header + fmt chunk + data chunk). A few bytes of silence at 22050 Hz mono is sufficient — it just needs to pass file type validation.
