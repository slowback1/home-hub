# Remove ComfyUI and RetroAchievements from widget registry

## Status

`done`

## Description

Delete the ComfyUI and RetroAchievements entries from `WIDGET_REGISTRY` and remove their stub widget files. Neither feature has a query API worth surfacing on the dashboard; leaving them in the registry clutters the widget picker with dead entries.

## Acceptance Criteria

- [ ] `WIDGET_REGISTRY` in `src/lib/services/Dashboard/widgetRegistry.ts` no longer contains entries with `id: 'comfyui'` or `id: 'retro'`.
- [ ] `src/lib/services/Dashboard/widgets/ComfyUiWidget.svelte` is deleted.
- [ ] `src/lib/services/Dashboard/widgets/RetroWidget.svelte` is deleted.
- [ ] `widgetRegistry.spec.ts` (and any other tests referencing these widgets) is updated so the test suite passes.
- [ ] The widget picker modal no longer lists ComfyUI or RetroAchievements when running the app.

## Notes

- `widgetRegistry.ts` imports `ComfyUiWidget` and `RetroWidget` — remove those imports along with the registry entries.
- Check `widgetRegistry.spec.ts` to see if it asserts a specific widget count or references these IDs; update accordingly.
- The icons (`Image`, `Gamepad2`) imported from `lucide-svelte` solely for these entries can be removed from the import list too.
- No backend changes required.
