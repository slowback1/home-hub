# Build /walk-history List Page

## Status

`pending`

## Description

Build the full walk history list page at `/walk-history` — a ledger table of all synced sessions sorted most-recent-first, with a page header and an empty state when no sessions exist.

## Acceptance Criteria

- [ ] `frontend/src/routes/walk-history/+page.svelte` exists and is accessible at `/walk-history`
- [ ] Page header contains: brand-colored square tile with Footprints icon, h1 "Walk History", subtitle showing session count (e.g. "8 sessions")
- [ ] Sessions are displayed in a table with columns: **When** (relative time primary + weekday/time secondary), **Steps** (right-aligned, thousands-separated, bold), **Duration** (right-aligned, secondary color, "Xh Ym" format)
- [ ] Table rows are sorted most-recent-first (relies on API returning sorted data)
- [ ] Row hover shows a surface-raised background highlight
- [ ] Empty state: centered Footprints icon in a circle, "No walks recorded yet" heading, descriptive copy, pill hint "Open SlowWalk on your phone to begin."
- [ ] Loading state handled (spinner while fetching)
- [ ] Page title is "Walk History — HomeHub" (via `<svelte:head>`)
- [ ] Uses `fmtDuration`, `relTime`, and `weekdayTime` from `walkFormatters.ts`

## Notes

- Design handoff: `https://api.anthropic.com/v1/design/h/kNbDOVd14txseG7C5_d_cA?open_file=Walk+History.html` — refer to `WalkPageLedger`, `WalkEmptyCentered`, and `walk.css` for the visual spec.
- Use `Footprints` from `lucide-svelte`.
- The page does not need its own feature-flag guard — the sidebar nav entry (task 07) controls discoverability, and the route is always accessible if navigated to directly.
- Follow the `<svelte:head>` title pattern from other pages (e.g. `weather/+page.svelte`).
