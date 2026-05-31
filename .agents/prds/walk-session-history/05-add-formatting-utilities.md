# Add Duration and Relative-Time Formatting Utilities

## Status

`done`

## Description

Add two pure formatting helpers that are shared across the widget and the list page: `fmtDuration` (converts seconds to "Xh Ym" / "Ym") and `relTime` (converts a date to "Today" / "Yesterday" / "N days ago") with a secondary weekday+time label. Unit-test both thoroughly since formatting correctness is user-visible.

## Acceptance Criteria

- [ ] `fmtDuration(seconds: number): string` returns `"Xh Ym"` when >= 60 minutes, `"Ym"` otherwise (e.g. `3725` → `"1h 2m"`, `2280` → `"38m"`)
- [ ] `relTime(iso: string): string` returns `"Today"`, `"Yesterday"`, or `"N days ago"` based on calendar-day difference from now
- [ ] `weekdayTime(iso: string): string` returns a string like `"Thu · 7:42 AM"`
- [ ] All three helpers are exported from a single file at `frontend/src/lib/utils/walkFormatters.ts`
- [ ] A spec file at `frontend/src/lib/utils/walkFormatters.spec.ts` covers: boundary cases for `fmtDuration` (0s, 59s, 60s, 3599s, 3600s+), `relTime` for today/yesterday/multi-day, and `weekdayTime` output shape

## Notes

- `fmtDuration` takes **seconds** (matching the `durationSeconds` field), not minutes.
- `relTime` must compare calendar days (midnight boundaries), not elapsed hours — "Yesterday" means the previous calendar day regardless of time of day.
- Do not couple these helpers to any Svelte reactivity primitives; they must be plain TypeScript functions.
