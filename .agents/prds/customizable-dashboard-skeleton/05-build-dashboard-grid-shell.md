# Build Dashboard Grid Shell

## Status

`pending`

## Description

Replace the `/` route with the dashboard page. Render a 3×2 CSS grid of `FilledSlot` and `EmptySlot` components, load the persisted layout from the API on mount, and save changes back on every assignment. Include the widget header with a navigation chevron and responsive breakpoints.

## Acceptance Criteria

- [ ] `/` renders a 3×2 grid (3 columns × 2 rows, 6 cells) via `display: grid; grid-template-columns: repeat(3, minmax(0, 1fr))`
- [ ] `FilledSlot` renders the widget's stub component with a header row: icon + uppercase name + `›` chevron button that navigates to the widget's `href`
- [ ] `EmptySlot` renders a dashed-border placeholder with a centered circular `+` icon and "Add widget" label; the whole slot is a button
- [ ] Layout is fetched via `DashboardApi.getLayout()` on page mount; slots render correctly from the returned data
- [ ] Page header shows title "Dashboard" and a subtitle: `{n}/6 widgets · {time-of-day greeting}` when any slots are filled, or instructional copy when all are empty
- [ ] Grid reflows to 2 columns at `≤960px` and 1 column at `≤640px` via CSS media queries
- [ ] Slot minimum height is `260px`

## Notes

The "Edit Dashboard" button and widget picker are wired in tasks 06–07 — leave a placeholder `{#if editMode}` block and an `onAdd` prop stub for now. Use the design handoff CSS (`dashboard.css` in the bundle) as the visual reference; key classes: `slot-grid`, `slot`, `slot--filled`, `slot--empty`, `widget-header`, `slot-empty__plus`. Time-of-day greeting: before 12 → "good morning", 12–17 → "good afternoon", 17–22 → "good evening", otherwise → "late night".
