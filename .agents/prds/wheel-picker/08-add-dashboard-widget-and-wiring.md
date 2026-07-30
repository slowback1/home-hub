# Add the quick-spin dashboard widget + wiring

## Status

`done`

## Description

Add the quick-spin dashboard widget and register the feature so it appears (gated by `WHEEL_PICKER_ENABLED`) in the sidebar nav and the widget picker.

## Acceptance Criteria

- [ ] `frontend/src/lib/services/Dashboard/widgets/WheelsWidget.svelte` renders a compact wheel selector, a Spin button, and an inline result; it always defaults the selector to the first saved wheel (no selection memory).
- [ ] The widget's Spin is disabled when the selected wheel has zero items; the widget shows a sensible empty state when no wheels exist.
- [ ] The widget uses the shared spin util (task 06) — no duplicated random logic.
- [ ] `WHEEL_PICKER_ENABLED` is added to `frontend/src/lib/services/FeatureFlag/FeatureFlags.ts`.
- [ ] The widget is registered in `widgetRegistry.ts` with `id: 'wheels'`, name "Wheels", `href: '/wheels'`, `featureFlag: FeatureFlags.WHEEL_PICKER_ENABLED`, and the `Disc3` lucide icon.
- [ ] With the flag off, the widget is excluded from `getVisibleWidgets()` and the sidebar nav item is hidden; with it on, both appear.
- [ ] `widgetRegistry.spec.ts` (and `Sidebar.spec.ts` if applicable) are updated and pass.

## Notes

- Mirror the `walk-history` registry entry in `frontend/src/lib/services/Dashboard/widgets/WalkHistoryWidget.svelte` and `widgetRegistry.ts`; the sidebar nav derives visibility from the registry via `getVisibleWidgets()`.
- `data-testid` hooks needed for E2E: widget selector, Spin button, and result (see task 09). The feature-flag-hidden scenario checks `nav-item-wheels` and the widget-picker card `data-widget-id="wheels"`.
- Depends on tasks 05 (`WheelApi`) and 06 (spin util).
