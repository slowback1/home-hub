# PRD: Epub-to-Audiobook UI

## Status

`Draft`

## Overview

A dedicated section of the HomeHub dashboard for converting EPUB files to M4B audiobooks via the GPU TTS service. Users can upload EPUBs, monitor the conversion queue, download completed audiobooks, and manage voice samples — all from a two-tab interface at `/audiobook`.

## Problem Statement

Converting an EPUB to an audiobook currently requires manually running a script on the GPU machine. There is no visibility into queue state or progress, and retrieving the finished file requires SSH or direct filesystem access. Integrating this workflow into the home dashboard provides a single-UI experience: upload, wait, download.

## Goals

- Allow the user to submit EPUB conversion jobs through the dashboard
- Display the conversion queue with live status (oldest-first, in-progress row highlighted)
- Provide contextual actions: cancel queued/in-progress jobs, download/delete completed files
- Allow voice sample management (upload WAV, delete) from a dedicated tab
- Keep the feature toggleable via a feature flag and switchable between a real GPU service and a behavioral mock via system config

## Non-Goals

- In-browser audio playback / streaming (download only)
- Configuring TTS parameters (voice speed, model, silence) from the UI
- Deleting job history rows (the API has no delete-job endpoint)
- Pagination of the job list

## User Stories / Use Cases

- **As a** user, **I want to** upload an EPUB and pick a voice sample, **so that** I can kick off a conversion job without touching the GPU machine directly.
- **As a** user, **I want to** see the status of all conversion jobs in a queue, **so that** I know where my job is and when it finishes.
- **As a** user, **I want to** cancel a job I no longer need, **so that** I can free the GPU for a different conversion.
- **As a** user, **I want to** download the finished M4B file, **so that** I can listen to it on any device.
- **As a** user, **I want to** upload and delete WAV voice samples, **so that** I can control which voices are available for conversion.

## E2E Scenarios

```gherkin
@audiobook
Feature: Epub to Audiobook

  @audiobook-submit-job-happy-path
  Scenario: Submit a job and see it progress to completed
    Given I am on the audiobook convert page
    And at least one voice sample exists
    When I upload an EPUB file and select a voice sample
    And I submit the conversion form
    Then a new job appears in the queue with status "queued"
    And the job progresses to "in_progress"
    And the job progresses to "completed"
    And a download button is visible for the completed job

  @audiobook-cancel-queued-job
  Scenario: Cancel a queued job
    Given I am on the audiobook convert page
    And a queued job exists
    When I click cancel on the queued job
    Then the job status changes to "cancelled"

  @audiobook-download-completed-file
  Scenario: Download a completed audiobook file
    Given I am on the audiobook convert page
    And a completed job exists
    When I click download on the completed job
    Then the file download is initiated

  @audiobook-failed-job-shows-error
  Scenario: Failed job displays an error message
    Given I am on the audiobook convert page
    And a voice sample exists
    When I submit a conversion job that will fail
    Then the job status changes to "failed"
    And an error message is visible on the failed job row

  @audiobook-no-voice-samples-disables-form
  Scenario: Upload form is disabled when no voice samples exist
    Given I am on the audiobook convert page
    And no voice samples exist
    Then the conversion form is disabled
    And a message directing me to the Voice Samples tab is visible

  @audiobook-upload-voice-sample
  Scenario: Upload a voice sample
    Given I am on the audiobook voice samples page
    When I upload a WAV file as a voice sample
    Then the new voice sample appears in the list

  @audiobook-delete-voice-sample
  Scenario: Delete a voice sample
    Given I am on the audiobook voice samples page
    And at least one voice sample exists
    When I delete a voice sample
    Then the voice sample is removed from the list
```

## Proposed Solution

Add a new `/audiobook` section to the SvelteKit frontend with two tabs — **Convert** and **Voice Samples** — backed by a dedicated `AudiobookController` in the .NET Web API that proxies to the GPU service through an `IAudiobookService` interface.

### Convert Tab (`/audiobook`)

- Upload form: EPUB file picker + voice sample dropdown
- If no voice samples exist, the form is disabled with a message: "No voice samples available. Add one in the Voice Samples tab."
- On submit: form resets, new job appears at the bottom of the queue (oldest-first)
- Job list polls every 30 seconds while any job is in a non-terminal state
- Each row shows: EPUB filename, voice sample, status, timestamps
- In-progress row is visually highlighted
- Contextual actions:
  - `queued` / `in_progress`: Cancel button
  - `completed`: Download button + Delete File button
  - `failed`: Inline error message (no actions)
  - `cancelled`: No actions

### Voice Samples Tab (`/audiobook/voice-samples`)

- List of existing voice sample names
- Upload button: opens file picker filtered to `.wav` only; uploads immediately on selection
- Delete button per row

### Navigation

- Sidebar item: `BookAudio` icon, label "Audiobook", href `/audiobook`
- Hidden behind `AUDIOBOOK_ENABLED` feature flag (seeded **disabled** by default)

## Technical Approach

### Key Decisions

| Decision | Chosen Approach | Rationale |
|----------|----------------|-----------|
| Frontend ↔ GPU service communication | Proxy through .NET `AudiobookController` | Keeps GPU API key server-side; avoids CORS; consistent with existing architecture |
| Backend abstraction | `IAudiobookService` interface with `GpuServiceClient` and `MockAudiobookService` | Mirrors the `IWeatherProvider` switchable pattern; enables E2E testing without the real GPU service |
| Implementation selection | Factory registration in `Program.cs` reading `audiobook::provider` from system config | Consistent with weather provider pattern |
| Mock behaviour | Behavioral: jobs auto-advance through all states (queued → in_progress → completed/failed) in a few seconds | Enables full E2E test coverage of the entire state machine |
| Status polling interval | 30 seconds, active only while non-terminal jobs exist | Appropriate for hour-long conversions; stops automatically when queue is idle |
| Job list ordering | Oldest-first | Reinforces "to-do queue" mental model |
| Tab routing | `/audiobook` (Convert, index) + `/audiobook/voice-samples` | Mirrors `/admin/*` layout pattern |
| Error display | Inline, adjacent to the relevant action or job row | No toast infrastructure needed; errors are localized |

### Backend: `IAudiobookService`

```
IAudiobookService
  - ListJobsAsync() → IEnumerable<AudiobookJob>
  - SubmitJobAsync(epubFileName, epubBytes, voiceSampleName) → AudiobookJob
  - GetJobAsync(id) → AudiobookJob
  - CancelJobAsync(id) → void
  - DownloadFileAsync(id) → Stream
  - DeleteFileAsync(id) → void
  - ListVoiceSamplesAsync() → IEnumerable<string>
  - UploadVoiceSampleAsync(name, wavBytes) → void
  - DeleteVoiceSampleAsync(name) → void
```

**`GpuServiceClient`** — reads `audiobook::url` and `audiobook::api_key` from system config; forwards HTTP calls to the FastAPI GPU service.

**`MockAudiobookService`** — in-memory store; jobs advance: queued (1s) → in_progress (3s) → completed (or failed on a deterministic condition for testing). Voice samples stored in-memory list.

### System Config Seeds

| Id | Type | Default | Notes |
|----|------|---------|-------|
| `audiobook::provider` | select | `mock` | Options: `mock`, `gpu-service` |
| `audiobook::url` | string | `` (empty) | GPU service base URL |
| `audiobook::api_key` | secret | `` (empty) | Bearer token for GPU service |

### Feature Flag

| Flag | Default |
|------|---------|
| `AUDIOBOOK_ENABLED` | `false` |

### Frontend: API Layer

A new `AudiobookApi.ts` following the existing `baseApi.ts` middleware pattern, covering all job and voice sample endpoints exposed by `AudiobookController`.

### E2E Tests

- Feature file: `e2e/features/audiobook.feature`
- Step definitions: `e2e/steps/audiobook.steps.ts`
- Page objects: `e2e/pages/AudiobookConvertPage.ts`, `e2e/pages/AudiobookVoiceSamplesPage.ts`
- Fixture WAV file: `e2e/fixtures/sample.wav` (minimal valid WAV, checked in)
- All scenarios run against the mock provider (default config)

### Dependencies

- GPU service (epub-to-audiobook-gpu-service) — must be deployed and reachable for the `gpu-service` provider mode
- Existing SvelteKit frontend, .NET Web API, and system config infrastructure

## Open Questions

- [ ] What is the maximum EPUB file size the .NET proxy should accept? (The GPU service has no documented limit.)
- [ ] Should the "Delete File" action on a completed job require a confirmation prompt?

## Out of Scope

- In-browser audio streaming/playback
- TTS parameter configuration from the UI (voice speed, model, silence gap)
- Job history deletion
- Pagination of the job list
- Voice sample playback preview

## Success Metrics

- User can upload an EPUB and receive a downloadable M4B without touching the GPU machine
- All 7 E2E scenarios pass against the mock provider
- The feature flag hides the section cleanly when disabled

## Timeline / Milestones

_TBD_
