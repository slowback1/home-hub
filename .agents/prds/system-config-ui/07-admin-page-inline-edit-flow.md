# Admin Page: Inline Edit Flow

## Status

`done`

## Description

Make config values editable inline. Clicking a displayed value switches that row into edit mode, showing a text input pre-filled with the current value alongside explicit Save and Cancel buttons. Save calls PUT and shows a success or error toast. Cancel reverts the row to display mode without saving. Pressing Escape also cancels.

## Acceptance Criteria

- [ ] Clicking a value in a row switches that row to edit mode (text input + Save + Cancel buttons)
- [ ] Only one row can be in edit mode at a time
- [ ] Save calls `SystemConfigApi.update(namespace, key, value)` with the new value
- [ ] On success: row returns to display mode showing the updated value; success toast is shown
- [ ] On error: row remains in edit mode; error toast is shown
- [ ] Cancel reverts the input to the original value and returns the row to display mode
- [ ] Pressing Escape also cancels
- [ ] Unit/component tests cover: entering edit mode, saving successfully, saving with error, and cancelling

## Notes

Use `SystemConfigApi.update()` from task 05. Toast messaging uses the existing `MessageBus` + `ToastWrapper` pattern — see `frontend/src/lib/bus/Messages.ts` and the existing toast components for the correct message format.

The text input should be the `TextBox` component from `frontend/src/lib/ui/inputs/TextBox/TextBox.svelte`. Save and Cancel should use the `Button` component.
