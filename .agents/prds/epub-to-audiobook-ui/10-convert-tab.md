# Convert Tab

## Status

`done`

## Description

Build the Convert tab at `/audiobook`. It contains an EPUB upload form (with voice sample dropdown) and a job queue list that polls for status updates. Job rows display contextual actions based on their current status.

## Acceptance Criteria

- [ ] Page lives at `frontend/src/routes/audiobook/+page.svelte` and replaces the placeholder
- [ ] On load, voice samples are fetched; if the list is empty the form is disabled and displays: "No voice samples available. Add one in the Voice Samples tab."
- [ ] Upload form contains:
  - EPUB file picker (accepts `.epub` only)
  - Voice sample dropdown populated from `AudiobookApi.listVoiceSamples()`
  - Submit button
- [ ] Submitting the form calls `AudiobookApi.submitJob()`, resets the form on success, and the new job immediately appears at the bottom of the job list
- [ ] Job list is ordered oldest-first (ascending `createdAt`)
- [ ] The in-progress job row is visually highlighted (distinct background or border)
- [ ] Each row shows: EPUB filename, voice sample name, status badge, `createdAt` timestamp
- [ ] Contextual row actions:
  - `queued` or `in_progress`: "Cancel" button → calls `cancelJob(id)` and updates the row
  - `completed`: "Download" link (anchor with `href` from `getFileUrl(id)` and `download` attribute) + "Delete File" button → calls `deleteFile(id)`
  - `failed`: inline error message displaying the job's `errorMessage`
  - `cancelled`: no actions
- [ ] Polling runs every 30 seconds while at least one job has status `queued` or `in_progress`; polling stops when all jobs are in terminal states
- [ ] Inline error message is shown near the form if job submission fails

## Notes

Use `onMount` + `setInterval` for polling. Clear the interval in `onDestroy` to avoid memory leaks.

The "Download" action should be a plain `<a>` tag with `href={audiobookApi.getFileUrl(id)}` and the `download` attribute — not a button that fetches a blob — so the browser handles the file download natively.

The "Delete File" button should update the job row in the local list (remove the download/delete buttons) after the API call succeeds, reflecting that the file is gone. The job row itself stays in the list.
