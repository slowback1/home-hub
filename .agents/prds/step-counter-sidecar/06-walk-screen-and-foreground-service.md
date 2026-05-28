# Walk Screen + ForegroundService

## Status

`done`

## Description

Implement the core walk tracking flow: the Walk screen UI (Start/Stop, live step count, elapsed timer) wired to a `ForegroundService` that reads the `TYPE_STEP_COUNTER` sensor. The service keeps counting with the screen off, posts a live notification with a Stop action, and signals an active session via a pulsing dot on the Walk tab.

## Acceptance Criteria

- [ ] Walk screen shows a "Start Walk" button when no session is active
- [ ] Tapping "Start Walk" starts `StepCounterService` as a foreground service; the walk screen transitions to active state showing live step count and elapsed time (updating at least every second)
- [ ] Step count and timer continue to increment with the screen off
- [ ] The foreground notification shows current step count and elapsed time; it includes a "Stop Walk" action that ends the session from the notification shade
- [ ] Tapping "Stop Walk" (on screen or notification) stops the service, saves the completed `WalkSessionEntity` to Room (with `serverSyncedAt = null`), and returns the Walk screen to idle state
- [ ] The Walk tab in the bottom nav shows a pulsing dot while a session is active; the dot disappears when the session ends
- [ ] The Walk tab dot is visible when navigating to History or Settings during an active walk
- [ ] `FOREGROUND_SERVICE_TYPE_HEALTH` is declared in `AndroidManifest.xml`
- [ ] A notification channel is created on first app launch

## Notes

`TYPE_STEP_COUNTER` returns cumulative steps since last device reboot. Snapshot the sensor value at `onStartCommand` and compute delta continuously — do not use `TYPE_STEP_DETECTOR`.

Service ↔ screen communication: bind the service to the Activity and expose a `StateFlow<WalkState>` (steps, elapsedSeconds, isActive) that the Walk screen ViewModel collects.

Elapsed time: track `startEpochMillis` in the service and compute elapsed in a `flow { while(true) { emit(...); delay(1000) } }` coroutine within the service.

The completed session saved to Room uses `clientId = UUID.randomUUID().toString()`. The checkpoint logic (periodic Room writes during an active walk) is added in task 07 — this task only writes the final record on stop.

Required manifest permissions: `FOREGROUND_SERVICE`, `FOREGROUND_SERVICE_HEALTH`, `BODY_SENSORS` (for step counter on some devices — include it defensively).
