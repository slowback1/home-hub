# Implement AudiobookWidget

## Status

`pending`

## Description

Replace the `AudiobookWidget.svelte` placeholder with a real implementation that fetches the job list, surfaces the most relevant job (active first, then most recent completed), displays filename and status badge, handles the empty state, and polls every 5 minutes.

## Acceptance Criteria

- [ ] On mount, the widget calls `AudiobookApi.listJobs()` and shows a spinner during the request.
- [ ] Job selection logic:
  - Prefer the most recent job with status `queued` or `in_progress`.
  - If none, fall back to the most recent job with status `completed`.
  - If no jobs exist at all, show the empty state.
- [ ] On success with a job, the widget displays:
  - The `epubFilename` in monospaced text (truncated with ellipsis if too long).
  - A status badge for the job's status; badge text uses `white-space: nowrap`. Status-to-label map: `queued` → "queued", `in_progress` → "in progress", `completed` → "completed", `failed` → "failed".
  - No progress bar (the API has no `progress` field).
- [ ] Empty state (no jobs): muted italic "No conversions yet".
- [ ] On API error, the widget shows the shared error state: muted `—` and italic "Unavailable".
- [ ] The widget polls `AudiobookApi.listJobs()` every 5 minutes; the interval is cleared on destroy.

## Notes

- `AudiobookApi` is at `src/lib/api/AudiobookApi.ts`; `listJobs()` returns `AudiobookJob[]` with `{ id, status, epubFilename, voiceSampleName, createdAt, updatedAt, errorMessage }`.
- Status type: `'queued' | 'in_progress' | 'completed' | 'failed' | 'cancelled'`.
- Sort jobs by `updatedAt` descending to find "most recent" within each status group.
- Use the existing `Badge` component from `src/lib/ui/feedback/Badge.svelte` for the status badge. Check badge variant names from `Badge.stories.ts`.
- 5-minute interval in ms: `300_000`.
