# Build Widget Picker Modal

## Status

`done`

## Description

Implement the `WidgetPicker.svelte` modal that opens when the user clicks an empty slot. It lists available widgets as selectable cards and already-placed widgets greyed out below a divider. Selecting a widget assigns it to the slot and saves the full layout via the API.

## Acceptance Criteria

- [ ] Clicking an empty slot's "+" button opens the picker modal with the slot index tracked
- [ ] Available widgets (not already placed, feature flag enabled) appear as interactive cards showing icon, name, and description
- [ ] Already-placed widgets appear below an "already on dashboard" divider, greyed out with a ✓ indicator and `aria-disabled="true"`
- [ ] Clicking an available widget card assigns it to the target slot, closes the modal, and calls `DashboardApi.saveLayout()` with the full updated slot array
- [ ] The modal is dismissed by clicking the Cancel button or pressing Escape
- [ ] Clicking the modal backdrop also dismisses it

## Notes

`alreadyPlaced` is derived from the current in-memory slot state — no extra API call needed. The save is a full PUT of all slots (consistent with the API design). See the `picker-modal`, `picker-card`, `picker-card--disabled`, and `picker-divider` CSS classes in the design handoff for visual reference. The modal should be accessible: `role="dialog"`, `aria-modal="true"`, `aria-labelledby` pointing at the title.
