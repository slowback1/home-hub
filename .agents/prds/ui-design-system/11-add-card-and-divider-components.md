# Add Card and Divider Components

## Status

`done`

## Description

Build two new layout primitives: `Card` (a general-purpose content container with `--radius-md` and a subtle surface background) and `Divider` (a simple horizontal or vertical visual separator). Both use design tokens exclusively and include unit tests and Storybook stories.

## Acceptance Criteria

- [ ] `Card` component exists and renders a container with `--radius-md` border radius and a dark-theme surface background color from the token system
- [ ] `Card` accepts default slot content and optionally a title/header area
- [ ] `Card` padding uses spacing tokens
- [ ] `Divider` component exists and renders a horizontal rule using a border-color token
- [ ] `Divider` supports at minimum horizontal orientation; vertical is a bonus
- [ ] Both components use no hardcoded hex color values
- [ ] Unit tests exist for both components covering rendering and slot/prop behavior
- [ ] Storybook stories exist for both components

## Notes

Place new components alongside existing ones: `frontend/src/lib/ui/containers/Card.svelte` and `frontend/src/lib/ui/typography/Divider.svelte` (or adjust paths to match the existing directory convention).
