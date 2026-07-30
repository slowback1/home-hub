# Build the `/wheels` page

## Status

`done`

## Description

Build the single `/wheels` page with its two sections: **Manage wheels** (list + create/edit/delete) and **Spin** (select a wheel, spin, see a result). Wired to `WheelApi` and the shared spin utility.

## Acceptance Criteria

- [ ] `frontend/src/routes/wheels/+page.svelte` renders a **Manage** section listing saved wheels (name + item count) with create, edit, and delete actions.
- [ ] Creating/editing a wheel uses a textarea with one item per line; blank lines are trimmed and ignored on save.
- [ ] A wheel with a blank/whitespace name cannot be saved; a wheel with zero items **can** be saved.
- [ ] A **Spin** section lets the user select a saved wheel and click Spin; the result (one random item) is displayed inline.
- [ ] The Spin button is disabled when the selected wheel has zero items.
- [ ] When no wheels exist, the page shows an empty state.
- [ ] Loading and error states are handled (Spinner / error message), consistent with existing pages.
- [ ] `data-testid` hooks exist for the manage list, wheel rows, create/edit/delete controls, wheel selector, Spin button, spin result, and empty state (to satisfy the task-09 E2E steps).
- [ ] Frontend checks (typecheck/lint + any component tests) pass.

## Notes

- Mirror `frontend/src/routes/activity/config/+page.svelte` (CRUD table, `TextBox`, `Button`, `ToastService`, delete affordance) and `frontend/src/routes/activity/+page.svelte` (hero/result + placeholder/empty state) for structure and styling; use existing `$lib/ui` components and design tokens.
- Use the shared spin util from task 06 for both parsing `items` and selecting the result; do not duplicate the random logic here.
- Icon for the feature is `Disc3` (lucide) — used in nav/widget (task 08); the page header may reuse it.
- Coordinate `data-testid` names with the E2E steps/Page Object so task 09 can drive the scenarios green.
