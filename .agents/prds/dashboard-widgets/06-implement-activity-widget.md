# Implement ActivityWidget

## Status

`pending`

## Description

Replace the `ActivityWidget.svelte` placeholder with a real implementation that fetches the current activity pick, displays the activity name prominently with the picked date in muted secondary text, and polls every 5 minutes.

## Acceptance Criteria

- [ ] On mount, the widget calls `ActivityPickApi.getCurrent()` and shows a spinner during the request.
- [ ] On success, the widget displays:
  - The activity name large and bold, vertically centered in the card body.
  - Muted monospace secondary text below it reading `picked <date>` (format the `pickedAt` ISO string as a readable date, e.g. `May 25`).
- [ ] On error, the widget shows the shared error state: muted `—` and italic "Unavailable".
- [ ] The widget polls `ActivityPickApi.getCurrent()` every 5 minutes; the interval is cleared on destroy.
- [ ] No re-roll button or interaction is present — the widget is read-only.

## Notes

- `ActivityPickApi` is at `src/lib/api/ActivityPickApi.ts`; `getCurrent()` returns `{ id, activityName, pickedAt }` where `pickedAt` is an ISO date string.
- The card body is the only area to implement; the header chrome is handled by the dashboard page.
- 5-minute interval in ms: `300_000`.
- Match the design: activity name large + bold, vertically centered; `picked <date>` beneath it in muted monospace micro-text.
