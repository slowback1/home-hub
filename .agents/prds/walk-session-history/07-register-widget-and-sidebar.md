# Register WalkHistoryWidget in Widget Registry and Sidebar

## Status

`done`

## Description

Wire the Walk History widget into the dashboard widget picker and add the sidebar nav entry, both gated by the `WALK_SESSION_HISTORY_ENABLED` feature flag.

## Acceptance Criteria

- [ ] `WIDGET_REGISTRY` in `widgetRegistry.ts` contains a `walk-history` entry with `id`, `name`, `description`, `icon` (Footprints), `href` (`/walk-history`), `featureFlag` (`WALK_SESSION_HISTORY_ENABLED`), and `component` (WalkHistoryWidget)
- [ ] The widget appears in the dashboard picker when `WALK_SESSION_HISTORY_ENABLED` is enabled, and is absent when it is disabled
- [ ] `Sidebar.svelte` contains a nav item for "Walk History" (href `/walk-history`, icon `Footprints`, flag `WALK_SESSION_HISTORY_ENABLED`, testId `nav-item-walk-history`)
- [ ] The sidebar nav item is visible when the flag is enabled and hidden when disabled

## Notes

- `widgetRegistry.ts` is at `frontend/src/lib/services/Dashboard/widgetRegistry.ts`.
- `Sidebar.svelte` is at `frontend/src/lib/ui/navigation/Sidebar.svelte`.
- Follow the exact same pattern as the `bookmarks` or `tasks` entries in both files.
- Import `Footprints` from `lucide-svelte`.
