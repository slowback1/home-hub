# Add Tabs Component

## Status

`pending`

## Description

Build the `Tabs` component for in-page section switching. A `Tabs` container manages active state, and individual `Tab` items render as a horizontal list of clickable labels. The active tab is visually distinguished using design tokens. Includes unit tests and a Storybook story.

## Acceptance Criteria

- [ ] `Tabs` component (or `Tabs` + `Tab` pair) exists and renders a horizontal tab list
- [ ] Clicking a tab updates the active state and emits or exposes the selected tab to the parent
- [ ] The active tab is visually highlighted using design token colors (not hardcoded)
- [ ] Tab container and item padding/spacing use spacing tokens
- [ ] Component uses `--radius-sm` for tab indicator or active state styling as appropriate
- [ ] No hardcoded hex color values
- [ ] Unit tests cover: initial active state, tab switching, correct active highlighting
- [ ] Storybook story demonstrates tab switching with multiple tabs

## Notes

Place the component at `frontend/src/lib/ui/navigation/Tabs.svelte` (alongside the Sidebar). Decide whether to use a single component with items as props/slots or a compound `Tabs`/`Tab` pattern — either is acceptable; choose what fits the existing component conventions in this codebase.
