# Migrate Existing Pages

## Status

`pending`

## Description

Update the home page and all demo pages to use the new design system components and tokens. Remove any one-off inline styles or hardcoded color values. After this task, no page in the app should bypass the token system.

## Acceptance Criteria

- [ ] Home page (`/`) uses design system components (e.g. `Heading`, `Card`) and references no hardcoded color or spacing values
- [ ] Demo pages (`/demo/content`, `/demo/form`, `/demo/list`) use design system components throughout
- [ ] No `style` attributes or hardcoded hex values remain in any page-level `.svelte` file
- [ ] All pages render correctly in the dark theme with the Sidebar visible
- [ ] All existing tests pass

## Notes

Current pages: `frontend/src/routes/+page.svelte`, `frontend/src/routes/demo/+layout.svelte`, `frontend/src/routes/demo/content/+page.svelte`, `frontend/src/routes/demo/form/+page.svelte`, `frontend/src/routes/demo/list/+page.svelte`.

The demo pages appear to be component showcase pages — they are a good opportunity to use `Card` as a wrapper and `Divider` between sections. Keep the demo content itself intact; only update the surrounding layout and styling.
