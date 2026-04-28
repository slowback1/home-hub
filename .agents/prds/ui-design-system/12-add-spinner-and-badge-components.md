# Add Spinner and Badge Components

## Status

`pending`

## Description

Build two new feedback components: `Spinner` (an animated loading state indicator) and `Badge` (a small count or status label). Both use design tokens exclusively and include unit tests and Storybook stories.

## Acceptance Criteria

- [ ] `Spinner` component exists and renders an animated loading indicator
- [ ] `Spinner` accepts a size prop (at minimum small/medium/large) with values derived from spacing tokens
- [ ] `Spinner` color references a design token
- [ ] `Badge` component exists and renders a small label with a pill-shaped background
- [ ] `Badge` accepts a `variant` prop mapping to semantic token colors (e.g. default, success, warning, error)
- [ ] `Badge` accepts text content via slot or prop
- [ ] Both components use no hardcoded hex color values
- [ ] Unit tests exist for both components
- [ ] Storybook stories exist for both components showing all variants/sizes

## Notes

Place new components at `frontend/src/lib/ui/feedback/Spinner.svelte` and `frontend/src/lib/ui/feedback/Badge.svelte` (create the `feedback/` directory if it does not exist, or use `containers/` if that is the established convention for these types).
