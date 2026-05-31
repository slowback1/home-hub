# Add WalkSessionApi Frontend Client

## Status

`done`

## Description

Create the typed frontend API client for walk sessions, following the existing `BaseApi` pattern. This client is shared by both the widget and the list page.

## Acceptance Criteria

- [ ] `frontend/src/lib/api/WalkSessionApi.ts` exists and exports a `WalkSessionApi` class extending `BaseApi`
- [ ] The client exports a `WalkSession` type matching the backend model: `{ id, clientId, startedAt, durationSeconds, stepCount, syncedAt }`
- [ ] `listSessions(): Promise<WalkSession[]>` calls `GET /api/walk-sessions` and returns the typed array
- [ ] `frontend/src/lib/api/WalkSessionApi.spec.ts` exists with unit tests covering the happy-path fetch and verifying the endpoint URL

## Notes

- Follow the pattern in `TasksApi.ts` and `TasksApi.spec.ts` exactly — same constructor, same `this.Get<T>()` call style.
- Use `getFetchMock` from `$lib/testHelpers/getFetchMock` in the spec (see existing API specs for usage).
