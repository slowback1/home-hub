# Polish Button Component

## Status

`pending`

## Description

Update the existing `Button` component's visual style to match the new design token system: apply the correct border radius, spacing, color, and typography tokens. Ensure all existing variants (primary, secondary, text) and sizes (small, medium, large) look cohesive and meet the warm/friendly aesthetic.

## Acceptance Criteria

- [ ] Button uses `--radius-sm` (8px) for border radius across all variants
- [ ] Button uses spacing tokens from the design token scale for padding
- [ ] Button color values reference design token CSS custom properties — no hardcoded hex values
- [ ] Button uses Nunito (inherited via the global font-family token) — no font overrides needed
- [ ] All three variants (primary, secondary, text) are visually distinct and consistent with the dark theme palette
- [ ] All three sizes (small, medium, large) render correctly
- [ ] Existing Button unit tests pass; update tests if class names or token names changed
- [ ] Storybook story for Button reflects the updated visual

## Notes

Button component is at `frontend/src/lib/ui/buttons/Button.svelte`. The existing component uses component-level CSS variables (e.g. `--button-border-radius`) — these can be replaced with the global token references directly.
