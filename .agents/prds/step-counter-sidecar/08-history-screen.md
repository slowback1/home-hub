# History Screen

## Status

`pending`

## Description

Implement the History screen showing the queue of pending (unsynced) walk sessions. Each row displays the session date, step count, and duration. Sessions can be deleted locally. An empty state is shown when nothing is pending. The Sync Now button is present but wired to sync logic in task 09.

## Acceptance Criteria

- [ ] History screen replaces the placeholder on the History tab
- [ ] Sessions where `serverSyncedAt IS NULL` are listed, most recent first
- [ ] Each row shows: date/time of `startedAt`, step count, formatted duration (e.g. "23 min" or "1h 04m")
- [ ] A delete button (or swipe-to-dismiss) removes the session from Room permanently
- [ ] Deleting a session that has never been synced does not attempt any network call
- [ ] An empty state message is shown when there are no pending sessions (e.g. "No pending sessions")
- [ ] A "Sync Now" button is visible at the top of the screen (its tap handler is a no-op placeholder — wired in task 09)
- [ ] The list updates reactively: if a new session is completed on the Walk screen while History is open, it appears without requiring a manual refresh

## Notes

Use a `HistoryViewModel` that exposes `Flow<List<WalkSessionEntity>>` from `WalkSessionDao.queryAllPending()`, collected as `collectAsStateWithLifecycle()` in the composable.

Duration formatting: `durationSeconds / 60` minutes. If >= 60 minutes, display as `Xh Ym`. Keep formatting in a utility function.

Synced sessions (where `serverSyncedAt IS NOT NULL`) are not shown — they remain in Room but are invisible to this screen. They are not deleted from Room on sync to preserve a local audit trail.
