# Add Frontend WIDGET_REGISTRY and API Client

## Status

`done`

## Description

Create the `WIDGET_REGISTRY` constant listing all known widget types with their metadata and stub components, and a `DashboardApi` service that calls the backend GET/PUT layout endpoints. The registry is feature-flag-filtered at runtime using the existing `FeatureFlagService`.

## Acceptance Criteria

- [ ] `WIDGET_REGISTRY` exists (e.g. in `src/lib/services/Dashboard/`) with entries for `tasks`, `activity`, `retro`, `weather`, `audiobook`, `bookmarks`, `comfyui` — each entry has `id`, `name`, `description`, `icon` (lucide icon name), optional `featureFlag`, optional `href` (for the nav chevron), and `component` (stub Svelte component)
- [ ] A `getVisibleWidgets()` helper returns registry entries whose `featureFlag` is either absent or currently enabled via `FeatureFlagService`
- [ ] Each stub widget component renders at minimum the widget name (placeholder until `dashboard-widgets` ships)
- [ ] `DashboardApi` has `getLayout(): Promise<LayoutResponse>` and `saveLayout(slots: SlotAssignment[]): Promise<void>` methods
- [ ] Unit tests cover `getVisibleWidgets()` filtering (flag enabled → included; flag disabled → excluded; no flag → always included)

## Notes

Follow the pattern of `BookmarksApi.ts` for the API client. `comfyui` uses `FeatureFlags.COMFYUI_ENABLED` (added in the PRD prep work). The stub widget components should live alongside the registry (e.g. `src/lib/services/Dashboard/widgets/`). Actual widget content is out of scope here — that's the `dashboard-widgets` PRD.
