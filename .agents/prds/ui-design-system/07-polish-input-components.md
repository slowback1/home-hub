# Polish Input Components

## Status

`done`

## Description

Apply design token styling to all six input components: TextBox, Checkbox, CheckboxGroup, Select, ComboBox, and ToggleSwitch. Each should use the correct radius, spacing, and color tokens and feel visually consistent with one another and with the dark theme palette.

## Acceptance Criteria

- [ ] TextBox uses `--radius-sm` for border radius and color tokens for border, background, and text
- [ ] Checkbox and CheckboxGroup use `--checkbox-size` (or equivalent token) and color tokens for checked/unchecked states
- [ ] Select uses `--radius-sm` and color tokens; dropdown matches the dark theme surface color
- [ ] ComboBox uses `--radius-sm` and color tokens; suggestion list matches the dark theme surface
- [ ] ToggleSwitch uses color tokens for on/off states; track and thumb dimensions use spacing tokens
- [ ] All inputs use Nunito (inherited via global font-family — no per-component font overrides)
- [ ] No hardcoded hex color values in any of the six components
- [ ] All existing unit tests for these components pass; update tests if class names or token names changed
- [ ] Storybook stories for each component reflect the updated visuals

## Notes

Input components live in `frontend/src/lib/ui/inputs/`. Existing components use a mix of component-level CSS variables and some hardcoded values — replace all with global token references.
