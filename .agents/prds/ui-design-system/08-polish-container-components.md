# Polish Container Components

## Status

`pending`

## Description

Apply design token styling to the five container/feedback components: Accordion, Alert, Chip, Toast, and Tooltip. Each should use `--radius-md` (12px) for container radius, spacing tokens for internal padding, and color tokens for backgrounds and borders consistent with the dark theme.

## Acceptance Criteria

- [ ] Accordion uses `--radius-md` for the panel border radius and color tokens for header/body backgrounds
- [ ] Alert uses `--radius-md` and semantic color tokens for each alert variant (error, warning, success, info)
- [ ] Chip uses `--radius-sm` (pill-like chips may use a larger radius — use judgement) and color tokens
- [ ] Toast uses `--radius-md` and semantic color tokens; matches the dark theme surface palette
- [ ] Tooltip uses `--radius-sm` and color tokens for background and text
- [ ] No hardcoded hex color values in any of the five components
- [ ] All existing unit tests for these components pass; update tests if needed
- [ ] Storybook stories for each component reflect the updated visuals

## Notes

Container components live in `frontend/src/lib/ui/containers/`. Toast rendering is controlled by `ToastWrapper.svelte` — make sure the wrapper itself is also updated if it contains any hardcoded styles.
