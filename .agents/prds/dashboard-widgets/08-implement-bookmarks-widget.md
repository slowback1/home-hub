# Implement BookmarksWidget

## Status

`pending`

## Description

Replace the `BookmarksWidget.svelte` placeholder with a real implementation that fetches bookmarks, shows starred ones (up to 5) as clickable new-tab links with a fallback to the 5 most recently added, and polls every hour.

## Acceptance Criteria

- [ ] On mount, the widget calls `BookmarksApi.listBookmarks()` and shows a spinner during the request.
- [ ] Bookmark selection logic:
  - Filter to `starred === true`; take up to 5.
  - If no starred bookmarks exist, fall back to the 5 most recently added bookmarks (sorted by `createdAt` descending).
- [ ] On success, the widget renders a vertical list where each row is a link:
  - Clicking opens the bookmark URL in a new tab (`target="_blank"`, `rel="noopener noreferrer"`).
  - Each row displays a star glyph (`★`), the bookmark name, and a right-chevron (`›`).
  - On hover: star turns warning-yellow, name underline turns brand-blue, chevron nudges right.
- [ ] Empty state (no bookmarks at all): muted "No bookmarks yet" or similar.
- [ ] On API error, the widget shows the shared error state: muted `—` and italic "Unavailable".
- [ ] The widget polls `BookmarksApi.listBookmarks()` every 1 hour; the interval is cleared on destroy.

## Notes

- `BookmarksApi` is at `src/lib/api/BookmarksApi.ts`; `listBookmarks()` returns `Bookmark[]` with `{ id, name, url, description, starred, createdAt }`.
- Links must open in a new tab — the widget is a shortcut launcher, not an in-app navigator.
- 1-hour interval in ms: `3_600_000`.
- Match the design: `★ name ›` row layout, hairline underline on names, hover colour shifts per the handoff.
