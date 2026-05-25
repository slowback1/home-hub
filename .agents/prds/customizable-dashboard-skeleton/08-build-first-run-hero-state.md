# Build First-Run Hero State

## Status

`done`

## Description

When all 6 slots are empty, replace the grid entirely with the first-run hero panel. It shows a welcome message, an "Add your first widget" CTA, and 3 preset-pack cards that each populate the grid with a curated widget set and save immediately.

## Acceptance Criteria

- [ ] When all 6 slots are null, the CSS grid is not rendered; the first-run hero panel is shown instead
- [ ] The hero shows the title "Your dashboard is empty." with body copy and a primary "Add your first widget" button that opens the widget picker for slot 0
- [ ] Three preset-pack cards are shown: "Daily essentials" (`tasks`, `weather`, `activity`), "Media & generation" (`audiobook`, `comfyui`, `retro`), "Everything" (all 6 in order)
- [ ] Clicking a preset card populates slot state with that pack's widget IDs (in order, indices 0–N), calls `DashboardApi.saveLayout()`, and transitions to the populated grid view
- [ ] Preset cards skip widgets whose feature flag is disabled — unavailable widgets are omitted and the remaining ones fill slots from index 0 upward
- [ ] Transitioning from the hero to the populated grid (via preset or manual first add) renders the grid correctly without a page reload

## Notes

`allEmpty` is a derived boolean: `slots.every(s => !s)`. Preset slug lists are hard-coded constants in the component. See `first-run`, `first-run__panel`, `first-run__preset`, and `first-run__suggestions-grid` CSS classes in the design handoff for visual reference. The preset grid is 3-column on desktop and stacks to 1-column at ≤960px.
