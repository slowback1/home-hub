# Sync Logic + Sync Now Wiring

## Status

`done`

## Description

Implement the sync flow: a Retrofit HTTP client posts pending sessions to `POST /api/walk-sessions` one at a time. Sync runs automatically on app open and manually via the Sync Now button on the History screen. A successful sync stamps `serverSyncedAt` locally, causing the session to disappear from the History list. Network errors are swallowed silently (session stays pending).

## Acceptance Criteria

- [ ] Retrofit client is configured with the backend URL read from Room (`backend_url` setting) at sync time — not hardcoded at app startup
- [ ] On app open (before the main screen is visible), all pending sessions are POSTed to `POST /api/walk-sessions`
- [ ] A successful 200 response updates the local `WalkSessionEntity.serverSyncedAt` to the current time, removing it from the History list
- [ ] A 200 response on a duplicate `clientId` (idempotent re-submit) is also treated as success and stamps `serverSyncedAt`
- [ ] Network errors and non-2xx responses leave the session pending (no crash, no dialog)
- [ ] Tapping Sync Now on the History screen triggers the same sync flow; a loading indicator is shown on the button while in flight; a snackbar confirms "Synced X sessions" or "Sync failed — will retry next time" on completion
- [ ] If no backend URL is configured, sync is skipped silently (no crash)
- [ ] Sessions are POSTed sequentially, not in parallel, to avoid overwhelming the backend

## Notes

Request body sent to the backend:
```json
{
  "clientId": "...",
  "startedAt": "2026-05-27T14:32:00Z",
  "durationSeconds": 1423,
  "stepCount": 2847
}
```

The backend sets `Id` and `SyncedAt` — do not send them.

Define a `SyncService` (or `WalkSessionRepository` method) that encapsulates the loop over pending sessions and can be called from both the on-open trigger and the ViewModel. Expose sync state as a `StateFlow<SyncState>` (idle / syncing / done / error) for the History screen to observe.

On-open sync: trigger from `MainActivity.onCreate` or a top-level `LaunchedEffect` in the nav host, before the user can tap Sync Now manually.
