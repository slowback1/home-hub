# ActivityPick Repository and Read API

## Status

`pending` <!-- pending | in-progress | done -->

## Description

Introduce the `ActivityPick` model and a custom repository interface for storing and querying pick history. Unlike `Activity`, this entity is not user-managed CRUD — it is written exclusively by the background job and read by the API. A dedicated `IActivityPickRepository` interface with InMemory, FileData, and EF implementations is the right shape here rather than the generic ICRUD pattern. The read endpoints power the display page's hero card and 7-day calendar grid.

## Acceptance Criteria

- [ ] `ActivityPick` model exists in `backend/Common/Models/` with fields: `Id` (Guid), `PickedAt` (DateTime, UTC), `ActivityName` (string) — no foreign key to `Activity`
- [ ] `IActivityPickRepository` interface is defined with at minimum: `WriteAsync(ActivityPick)`, `GetCurrentAsync()` (most recent pick), `GetRangeAsync(DateTimeOffset from, DateTimeOffset to)`
- [ ] InMemory, FileData, and EF implementations of `IActivityPickRepository` exist
- [ ] Repository is registered in DI (keyed off `CrudFactory:Implementation` or an equivalent mechanism)
- [ ] EF migration adds the `ActivityPicks` table with the correct schema
- [ ] `ActivityPickController` exposes:
  - `GET /api/activity/current` — returns the most recent `ActivityPick` (or 204 if none exists)
  - `GET /api/activity/history` — returns all picks in the last 7 days, ordered by `PickedAt`
- [ ] Controller is covered by unit or integration tests

## Notes

- `PickedAt` must be stored as UTC; no local time zone conversion in the backend
- `GetCurrentAsync` should return `null` / 204 (not 404) when the list is empty — the display page uses this to show the placeholder state
- `GetRangeAsync` for the history endpoint: caller passes `now - 7 days` to `now`
