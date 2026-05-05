# Voice Samples Tab

## Status

`pending`

## Description

Build the Voice Samples tab at `/audiobook/voice-samples`. Users can view all uploaded voice samples, upload a new `.wav` file, and delete existing samples. This is the final implementation task; its acceptance criteria include all 7 E2E scenarios passing GREEN.

## Acceptance Criteria

- [ ] Page lives at `frontend/src/routes/audiobook/voice-samples/+page.svelte` and replaces the placeholder
- [ ] On load, voice samples are fetched from `AudiobookApi.listVoiceSamples()` and displayed in a list
- [ ] Each list row shows the sample name and a "Delete" button
- [ ] Clicking "Delete" calls `AudiobookApi.deleteVoiceSample(name)` and removes the row from the list on success
- [ ] An "Upload" button opens a file picker filtered to `.wav` files only (`accept=".wav"`)
- [ ] Selecting a file immediately triggers `AudiobookApi.uploadVoiceSample(file, name)` (name derived from the filename without extension) and adds the new sample to the list on success
- [ ] Inline error message is shown if upload or delete fails
- [ ] Running `task e2e:test --grep @audiobook` results in all 7 scenarios passing **GREEN**

## Notes

The file picker `accept` attribute should be `".wav"` to restrict the OS file dialog. The sample name sent to the API should be the filename without its `.wav` extension (e.g. `narrator.wav` → name `narrator`).

For E2E tests to go GREEN, the mock provider must be active (default config), the behavioral state progression must work within the test timeout, and the `e2e/fixtures/sample.wav` fixture file must be a valid WAV that the mock accepts.

Verify all 7 scenario slugs pass before marking this task done:
- `@audiobook-submit-job-happy-path`
- `@audiobook-cancel-queued-job`
- `@audiobook-download-completed-file`
- `@audiobook-failed-job-shows-error`
- `@audiobook-no-voice-samples-disables-form`
- `@audiobook-upload-voice-sample`
- `@audiobook-delete-voice-sample`
