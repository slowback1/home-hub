# Polish Table Components

## Status

`done`

## Description

Apply design token styling to the three table components: Table, TablePagination, and TableFilter. Table rows, headers, borders, and interactive elements should use color and spacing tokens and feel at home in the dark theme.

## Acceptance Criteria

- [ ] Table uses color tokens for header background, row backgrounds (including alternating/hover states), and border colors
- [ ] Table uses spacing tokens for cell padding
- [ ] TablePagination uses color tokens for active/inactive page controls and spacing tokens for layout
- [ ] TableFilter uses `--radius-sm` for filter inputs and color tokens consistent with the input component polish (task 07)
- [ ] No hardcoded hex color values in any of the three components
- [ ] All existing unit tests for these components pass; update tests if needed
- [ ] Storybook stories for each component reflect the updated visuals

## Notes

Table components live in `frontend/src/lib/ui/table/`. These components may have relatively complex internal layout — focus on token adoption for color and spacing rather than structural changes.
