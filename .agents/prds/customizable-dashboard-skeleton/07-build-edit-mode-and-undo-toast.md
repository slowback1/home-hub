# Build Edit Mode and Undo Toast

## Status

`pending`

## Description

Wire the "Edit Dashboard" / "Done" toggle in the page header. In edit mode, occupied slots gain a red × badge. Removing a widget clears the slot immediately, saves via the API, and shows a 4-second undo toast that can restore the widget to its original slot.

## Acceptance Criteria

- [ ] "Edit Dashboard" button appears in the dashboard header and is disabled when all slots are empty
- [ ] Clicking it enters edit mode: button changes to a "Done" button; occupied slots each gain a red × badge absolutely positioned offset outside the top-right corner of the card
- [ ] Clicking "Done" exits edit mode and the × badges disappear
- [ ] Clicking × removes the widget from that slot immediately, updates local state, and calls `DashboardApi.saveLayout()` with the full updated slot array
- [ ] A toast appears after removal for 4 seconds showing "{Widget Name} removed." with an "Undo" button
- [ ] Clicking "Undo" restores the widget to its original slot index, updates local state, and saves via the API
- [ ] The toast auto-dismisses after 4 seconds if Undo is not clicked

## Notes

× badge visual reference from design handoff: `position: absolute; top: -10px; right: -10px; width: 28px; height: 28px; background: var(--color-error); border-radius: var(--radius-full)`. The parent slot needs `position: relative; overflow: visible`. Check whether the existing `ToastWrapper` (in `+layout.svelte`) can be reused via the `MessageBus`, or wire a local toast component directly on the dashboard page.
