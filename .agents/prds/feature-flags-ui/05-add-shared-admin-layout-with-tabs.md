# Add Shared Admin Layout with Tabs

## Status

`pending` <!-- pending | in-progress | done -->

## Description

Introduce a shared SvelteKit layout for the admin section that renders a two-tab bar ("System Config" | "Feature Flags") above all admin pages. Update the sidebar so the "Admin" nav item stays highlighted when navigating to either admin page.

## Acceptance Criteria

- [ ] `frontend/src/routes/admin/+layout.svelte` exists and renders the `Tabs` component with two items: "System Config" linking to `/admin/system-config` and "Feature Flags" linking to `/admin/feature-flags`
- [ ] The active tab is highlighted correctly based on the current route
- [ ] The existing `/admin/system-config` page continues to work and renders within the new layout without visual regression
- [ ] The sidebar "Admin" nav item is active when the current path starts with `/admin` (covers both admin pages)
- [ ] Unit or component test covers the layout rendering and correct active-tab behaviour

## Notes

- The `Tabs` component is at `frontend/src/lib/ui/navigation/Tabs.svelte` — check its props API before wiring.
- The sidebar active detection is in `frontend/src/lib/ui/navigation/Sidebar.svelte`. Currently the Admin item uses `href: '/admin/system-config'` and `isActive` checks `currentPath.startsWith(href)`. Change the href to `/admin` so both child routes match, and ensure clicking the nav item still lands on `/admin/system-config` (either via a redirect or by keeping the href pointing there while fixing the active check separately).
- SvelteKit layouts apply to all routes nested under the folder — `/admin/system-config` and `/admin/feature-flags` both inherit `admin/+layout.svelte` automatically.
