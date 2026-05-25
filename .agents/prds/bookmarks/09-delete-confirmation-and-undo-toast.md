# Delete Confirmation Dialog + Undo Toast

## Status

`pending`

## Description

Wire the Delete button on each card to a confirmation dialog. After the user confirms, delete the bookmark via the API and show a 5-second undo toast that can restore it.

## Acceptance Criteria

- [ ] Clicking a card's Delete button opens a confirmation dialog (not an immediate delete)
- [ ] Confirmation dialog shows the bookmark's name and hostname so the user knows what they're deleting
- [ ] "Cancel" closes the dialog and the bookmark is untouched
- [ ] "Delete" confirms: calls `BookmarksApi.deleteBookmark`, removes the card from the grid, closes the dialog
- [ ] After deletion, a toast appears: _"Deleted '[name]'."_ with an "Undo" action
- [ ] Clicking "Undo" within 5 seconds calls `BookmarksApi.createBookmark` to restore the bookmark and dismisses the toast
- [ ] Toast auto-dismisses after 5 seconds if Undo is not clicked
- [ ] Consistent with the app-wide pattern: confirmation dialog for all destructive deletes

## Notes

- Use the existing `Modal` primitive for the confirmation dialog (or a `ConfirmDialog` variant if one exists in the design system)
- Use the existing `Toast` primitive if one exists; otherwise build a minimal toast component
- The undo restore should re-create the bookmark with the same `name`, `url`, `description`, and `starred` values — `id` and `createdAt` will be new (backend assigns them)
- See `e2e/steps/tasks.steps.ts` for how the existing undo toast is tested, as a reference for E2E step wording
