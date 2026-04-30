# Add Admin Sidebar Nav Item

## Status

`pending`

## Description

Add an "Admin" entry to the sidebar navigation that links to `/admin/system-config`. This makes the admin page discoverable without requiring users to know the URL.

## Acceptance Criteria

- [ ] The sidebar displays an "Admin" nav item with the `Settings` icon from `lucide-svelte`
- [ ] The nav item links to `/admin/system-config`
- [ ] The item highlights as active when the current path starts with `/admin`
- [ ] The nav label hides correctly when the sidebar is collapsed (consistent with existing nav items)
- [ ] Existing sidebar unit tests continue to pass
- [ ] A `data-testid` attribute is present on the new nav item (e.g. `nav-item-admin`) consistent with existing items

## Notes

Nav items are defined in the `navItems` array in `frontend/src/lib/ui/navigation/Sidebar.svelte`. Follow the existing pattern — each item has `testId`, `href`, `label`, and `icon`.

`Settings` is available from `lucide-svelte` — confirm it is not already imported before adding it.
