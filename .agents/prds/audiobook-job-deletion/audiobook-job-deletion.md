# PRD: Audiobook Job Deletion

## Status

`Draft`

## Overview

Allow users to permanently delete terminal audiobook jobs (completed, failed, or cancelled) from the conversion queue. Deleting a job removes the in-memory record, the output file, and the source EPUB. Active jobs (queued or in-progress) remain cancel-only.

## Problem Statement

Failed, cancelled, and completed jobs accumulate in the queue list indefinitely with no way to clean them up. This clutters the UI and wastes disk space on the GPU server. Users have no recourse once a job reaches a terminal state other than waiting for a server restart.

## Goals

- Surface a "Delete" button on all terminal-state job rows (completed, failed, cancelled)
- Deleting a job removes the job record, its output file, and its source EPUB
- The job disappears from the queue list immediately after deletion
- Reuse the existing `DELETE /api/audiobook/jobs/{id}` endpoint with context-sensitive behaviour: cancel if active, delete-record if terminal
- Implement deletion in the mock/in-memory layer; no DB changes required

## Non-Goals

- Confirmation dialogs before deletion
- Deleting active (queued or in-progress) jobs via this flow — those remain cancel-only
- Persistent database storage for job records (separate concern)
- Bulk deletion

## User Stories / Use Cases

- **As a** user, **I want to** delete a failed job, **so that** the error row stops cluttering my queue.
- **As a** user, **I want to** delete a completed job after downloading its audiobook, **so that** I free up GPU disk space.
- **As a** user, **I want to** delete a cancelled job I no longer care about, **so that** my queue stays clean.

## E2E Scenarios

```gherkin
@audiobook
Feature: Epub to Audiobook

  @audiobook-delete-completed-job
  Scenario: Delete a completed job
    Given I am on the audiobook convert page
    And a completed job exists
    When I click delete on the completed job
    Then the job is removed from the queue list

  @audiobook-delete-failed-job
  Scenario: Delete a failed job
    Given I am on the audiobook convert page
    And a failed job exists
    When I click delete on the failed job
    Then the job is removed from the queue list

  @audiobook-delete-cancelled-job
  Scenario: Delete a cancelled job
    Given I am on the audiobook convert page
    And a cancelled job exists
    When I click delete on the cancelled job
    Then the job is removed from the queue list
```

## Proposed Solution

Extend the existing `DELETE /api/audiobook/jobs/{id}` endpoint to branch on job status: cancel if the job is active, delete the record and files if the job is terminal. Add a `DeleteJobAsync` method to `IAudiobookService` and implement it in `MockAudiobookService` and `GpuServiceClient`. On the frontend, replace the "Delete File" button on completed rows with a "Delete" button, and add a "Delete" button to failed and cancelled rows.

## Technical Approach

### Key Decisions

| Decision | Chosen Approach | Rationale |
|----------|----------------|-----------|
| Deletable states | Completed, Failed, Cancelled | All terminal states; no useful output remains |
| Confirmation dialog | None | Consistent with existing "Delete File" behaviour |
| Delete source EPUB? | Yes | Orphaned EPUBs waste disk space and cannot be resubmitted |
| Endpoint strategy | Reuse `DELETE /api/audiobook/jobs/{id}`, context-sensitive | Clean REST; avoids endpoint proliferation |
| DB persistence in scope? | No | Larger architectural change; deserves its own task |
| GPU service call | Single `DELETE /api/jobs/:id` | GPU service encapsulates its own filesystem cleanup |
| Replace "Delete File" button | Yes — replace with "Delete" | Two deletion buttons per row is confusing UX |
| Service interface | Add `DeleteJobAsync(string id)` | Single-purpose methods; cancel ≠ delete |
| Button label | "Delete" | Consistent with "Cancel"; context makes it unambiguous |

### Endpoint: `DELETE /api/audiobook/jobs/{id}`

Updated behaviour (currently cancel-only):

| Job State | Behaviour | Response |
|---|---|---|
| Queued, InProgress | Cancel job (existing) | 204 No Content |
| Completed, Failed, Cancelled | Delete job record + files | 204 No Content |
| Not found | — | 404 Not Found |

The controller checks job status, then calls either `CancelJobAsync` (active) or `DeleteJobAsync` (terminal).

### `IAudiobookService` — new method

```csharp
Task DeleteJobAsync(string id);
```

Throws `KeyNotFoundException` if the job does not exist. Throws `InvalidOperationException` if the job is not in a terminal state.

### `MockAudiobookService` — implementation

- Remove job from `ConcurrentDictionary<string, AudiobookJob> _jobs`
- Remove job id from `HashSet<string> _deletedFiles` (cleanup)
- No actual files exist in the mock; file deletion is a no-op

### `GpuServiceClient` — implementation

- Call `DELETE {baseUrl}/api/jobs/{id}` with bearer auth
- GPU service handles output directory and source EPUB cleanup server-side
- Map 404 from GPU service → `KeyNotFoundException`

### Frontend — `/audiobook` Convert tab

| Job Status | Current Buttons | New Buttons |
|---|---|---|
| Queued | Cancel | Cancel |
| InProgress | Cancel | Cancel |
| Completed | Download, Delete File | Download, Delete |
| Failed | _(error message only)_ | Delete |
| Cancelled | _(none)_ | Delete |

- "Delete" calls `DELETE /api/audiobook/jobs/{id}`
- On 204, remove the job from local state immediately (no re-fetch)
- On error, surface a message consistent with existing error handling

### `AudiobookApi.ts` — new method

```typescript
deleteJob(id: string): Promise<void>
```

Calls `DELETE /api/audiobook/jobs/{id}`, expects 204.

### Dependencies

- Existing `IAudiobookService` interface and implementations (`MockAudiobookService`, `GpuServiceClient`)
- Existing `AudiobookController` and `AudiobookApi.ts`
- GPU service must support `DELETE /api/jobs/:id` with filesystem cleanup

### Files to Change

| File | Change |
|---|---|
| `backend/Common/Interfaces/IAudiobookService.cs` | Add `DeleteJobAsync(string id)` |
| `backend/Logic/Audiobook/MockAudiobookService.cs` | Implement `DeleteJobAsync` |
| `backend/Logic/Audiobook/GpuServiceClient.cs` | Implement `DeleteJobAsync` |
| `backend/WebAPI/Controllers/AudiobookController.cs` | Update `DELETE /jobs/{id}` to branch on status |
| `backend/Logic.Tests/Audiobook/MockAudiobookServiceTests.cs` | Add unit tests |
| `backend/WebAPI.Integration.Tests/Controllers/AudiobookControllerTests.cs` | Add integration tests |
| `frontend/src/lib/api/AudiobookApi.ts` | Add `deleteJob(id)` method |
| `frontend/src/routes/audiobook/+page.svelte` | Replace "Delete File" with "Delete"; add "Delete" for failed/cancelled rows |
| `e2e/features/audiobook.feature` | Add three deletion scenarios |
| `e2e/steps/audiobook.steps.ts` | Implement new step definitions |

## Open Questions

_N/A_

## Out of Scope

- Persistent database storage for `AudiobookJob` records (no `DbSet` or migration)
- Bulk deletion
- Undo / soft delete

## Success Metrics

_TBD_

## Timeline / Milestones

_TBD_
