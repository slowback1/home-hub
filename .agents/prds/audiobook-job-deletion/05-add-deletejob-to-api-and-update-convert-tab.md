# Add deleteJob to AudiobookApi.ts and Update Convert Tab

## Status

`done`

## Description

Add a `deleteJob(id)` method to `AudiobookApi.ts`, then update the Convert tab to surface a "Delete" button on all terminal-state job rows. Replace the existing "Delete File" button on completed rows. Remove the job from local state immediately on success. All three E2E deletion scenarios must pass GREEN.

## Acceptance Criteria

- [ ] `AudiobookApi.ts` exports `deleteJob(id: string): Promise<void>` calling `DELETE /api/audiobook/jobs/{id}`, expecting 204
- [ ] Completed job rows show "Download" and "Delete" (no longer "Delete File")
- [ ] Failed job rows show a "Delete" button (previously no action buttons)
- [ ] Cancelled job rows show a "Delete" button (previously no action buttons)
- [ ] Clicking "Delete" removes the job from the local list immediately without a re-fetch
- [ ] Queued and InProgress rows are unchanged (Cancel button only)
- [ ] `task e2e:test` runs with all three new scenarios passing GREEN (`@audiobook-delete-completed-job`, `@audiobook-delete-failed-job`, `@audiobook-delete-cancelled-job`)
- [ ] All existing E2E scenarios continue to pass

## Notes

- On a 204 response, filter the deleted job id out of the local jobs array — do not re-poll.
- Error handling should be consistent with existing patterns in `+page.svelte` (e.g. how cancel errors are surfaced).
- The button label is "Delete" (not "Delete Job" or "Delete File").
