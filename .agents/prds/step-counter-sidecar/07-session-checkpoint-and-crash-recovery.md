# Session Checkpoint + Crash Recovery

## Status

`done`

## Description

Make the foreground service resilient to unexpected termination by checkpointing session state to Room every 30 seconds. On app open, if an incomplete checkpoint exists, prompt the user to recover or discard. If recovered, the walk screen shows a "Resumed from checkpoint" indicator.

## Acceptance Criteria

- [ ] `StepCounterService` writes current step count and elapsed seconds to a dedicated `InProgressSessionEntity` row in Room every 30 seconds during an active walk
- [ ] The in-progress row is deleted when the session ends normally (Stop Walk)
- [ ] On app open, if an `InProgressSessionEntity` row exists, a dialog is shown: "A walk was interrupted. Resume or discard?"
- [ ] Tapping "Resume" restarts `StepCounterService`; it takes a fresh sensor snapshot as the new baseline and accumulates steps on top of the checkpointed total; the walk screen enters active state
- [ ] Tapping "Discard" deletes the in-progress row; no session is saved; the walk screen shows idle state
- [ ] When a session is resumed from a checkpoint, the walk screen displays a subtle "Resumed from checkpoint" label beneath the step count
- [ ] The "Resumed from checkpoint" label is not shown for normally started sessions

## Notes

Add `InProgressSessionEntity` to the Room database as a separate table (not a nullable column on `WalkSessionEntity`). Fields: `clientId` (String, primary key — generated when the walk starts), `startedAt` (Long), `accumulatedSteps` (Int), `elapsedSeconds` (Int), `sensorBaselineAtLastCheckpoint` (Int).

The 30-second checkpoint write should be a `while (isActive) { delay(30_000); saveCheckpoint() }` coroutine launched in the service's `CoroutineScope`.

On resume: read `accumulatedSteps` and `elapsedSeconds` from the in-progress row to seed the service's running totals. The new sensor snapshot becomes the baseline; steps counted from the snapshot add to `accumulatedSteps`.

Update `SlowWalkDatabase` to include `InProgressSessionEntity` and generate a new Room schema version (bump `version` in `@Database`).
