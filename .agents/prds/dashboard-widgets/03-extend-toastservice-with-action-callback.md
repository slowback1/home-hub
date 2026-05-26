# Extend ToastService with action callback and migrate dashboard undo toast

## Status

`done`

## Description

Add an optional `action` field to `ToastConfig` so callers can attach a labelled callback button (e.g. "Undo") to any toast. Update `ToastWrapper` to render the button when present. Then replace the dashboard page's bespoke `undoToast` state, timer logic, and custom `.toast` CSS with `ToastService.AddToast()` — eliminating the duplicate toast implementation and establishing a single system.

## Acceptance Criteria

- [ ] `ToastConfig` in `ToastService.ts` has an optional `action?: { label: string; onClick: () => void }` field.
- [ ] `ToastWrapper.svelte` renders the action button when `action` is present; clicking it calls `action.onClick` and removes the toast.
- [ ] `ToastService` unit tests cover the action field (presence and absence).
- [ ] `+page.svelte` (dashboard) no longer contains the `undoToast` reactive variable, the `undoRemove` function, the `TOAST_DURATION_MS` constant, the `<div class="toast">` markup, or the `.toast` / `.undo-btn` CSS rules.
- [ ] Widget removal undo on the dashboard still works end-to-end: removing a widget fires a toast with an "Undo" button that restores the widget within 4 seconds.
- [ ] Existing E2E and unit tests continue to pass.

## Notes

- `ToastService.ts` is at `src/lib/ui/containers/toast/ToastService.ts`; `ToastWrapper.svelte` is at `src/lib/ui/containers/toast/ToastWrapper.svelte`.
- The dashboard currently hard-codes a 4-second (`TOAST_DURATION_MS = 4000`) undo window. Preserve this by passing a `durationMs` or by using the existing toast duration mechanism — check how `ToastWrapper` currently handles dismissal timing before deciding.
- The dashboard's existing toast is wired to `removeWidget` — after the migration, `removeWidget` should call `ToastService.AddToast({ message: 'Removed ...', action: { label: 'Undo', onClick: undoRemove } })`.
- `ToastWrapper` is already used globally via the layout — no new mounting needed.
