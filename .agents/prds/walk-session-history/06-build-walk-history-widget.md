# Build WalkHistoryWidget

## Status

`done`

## Description

Build the dashboard widget Svelte component showing the 3 most recent walk sessions, with loading and empty states. Follows the design handoff (Rows variant).

## Acceptance Criteria

- [ ] `frontend/src/lib/services/Dashboard/widgets/WalkHistoryWidget.svelte` exists
- [ ] On mount, the widget fetches sessions via `WalkSessionApi.listSessions()` and displays up to 3 rows
- [ ] Each row shows: relative time label (primary) + weekday/time sub-label (secondary) on the left; step count (formatted with thousands separator) and duration (formatted as "Xh Ym") stacked right-aligned on the right
- [ ] Loading state: spinner + "Loading…" (matching the pattern in `TasksWidget.svelte`)
- [ ] Empty state: Footprints icon + "No walks yet — start one on your phone to see it here."
- [ ] Widget header shows Footprints icon + "Walk History" title + "View all" link (href `/walk-history`)
- [ ] Widget footer shows "View all walks →" link (href `/walk-history`)
- [ ] Uses `fmtDuration`, `relTime`, and `weekdayTime` from `walkFormatters.ts`

## Notes

- Design handoff: `https://api.anthropic.com/v1/design/h/kNbDOVd14txseG7C5_d_cA?open_file=Walk+History.html` — refer to `walk-screens.jsx` (`WalkWidgetRows`, `WalkWidgetLoading`, `WalkWidgetEmpty`) and `walk.css` for the visual spec.
- Use `Footprints` from `lucide-svelte` for the icon.
- The widget header layout must match the existing widget convention in the dashboard (`widget-header`, `widget-header__title`, `widget-header__open`) — see `TasksWidget.svelte` as the reference.
- This task does not register the widget in the registry — that is task 07.
