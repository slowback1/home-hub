# PRD: SlowWalk — Step Counter Sidecar

## Status

`Draft`

## Overview

SlowWalk is a sideloaded native Android app that records walk sessions using the device's hardware StepCounter sensor and syncs them to the HomeHub backend. It provides the physical-activity data feed that HomeHub's RPG progression system will consume, rewarding real-world walking with in-game XP.

## Problem Statement

HomeHub's RPG system needs a reliable source of real-world activity data to drive character progression. There is currently no mechanism for HomeHub to know that a walk happened or how many steps it involved. A web-based approach (PWA) cannot reliably count steps in the background when the screen is off. A native Android sidecar solves this with direct sensor access and a foreground service that keeps counting silently regardless of screen state.

## Goals

- Record walk sessions (step count, duration, start time) via the Android `StepCounter` hardware sensor
- Count steps reliably in the background while the screen is off, for the duration of a walk
- Persist sessions on-device when the home network is unavailable
- Sync pending sessions to the HomeHub backend automatically on app open and on demand via a manual button
- Survive unexpected termination mid-walk by checkpointing session state periodically

## Non-Goals

- GPS tracking or route recording
- Distance estimation from step count
- Multi-device or multi-user support
- Play Store distribution (APK is sideloaded via ADB or USB)
- Background sync while the app is closed
- Web frontend display of walk history (deferred; RPG integration reads sessions server-side)
- Authentication on the walk-sessions endpoint (revisit when HomeHub auth is more complete)

## User Stories / Use Cases

- **As a user**, I want to tap "Start Walk" and have the app count my steps in the background so that I can pocket my phone and walk normally without keeping the screen on.
- **As a user**, I want to see my current step count and elapsed time update live on the walk screen so that I can check my progress mid-walk.
- **As a user**, I want to tap "Stop Walk" and have the session saved locally so that the data is not lost if I'm away from home.
- **As a user**, I want unsynced sessions to sync automatically when I open the app at home so that I don't have to remember to tap anything.
- **As a user**, I want a "Sync Now" button on the history screen so that I can manually trigger a sync and see the result.
- **As a user**, I want to delete a local session before it syncs so that accidental or corrupted sessions don't get pushed to the backend.
- **As a user**, I want a "Test Connection" button in settings so that I can verify my backend URL is correct without starting a walk.
- **As a user**, I want the app to recover a walk session if it was killed mid-walk so that I don't lose steps I already counted.

## Proposed Solution

A three-screen Android app (Kotlin + Jetpack Compose, minSdk 31) with a bottom navigation bar:

1. **Walk screen** — "Start Walk" button triggers a foreground service that reads the `StepCounter` sensor and checkpoints to Room every 30 seconds. The screen shows live step count and elapsed time. "Stop Walk" ends the session and saves it. A pulsing dot on the Walk tab indicates an active session when navigating to other tabs.

2. **History screen** — Lists only pending (unsynced) sessions. Each row shows date, step count, duration, and a delete button. A "Sync Now" button at the top triggers sync of all pending sessions against the configured backend URL.

3. **Settings screen** — A single text field for the HomeHub backend URL, a Save button, and a "Test Connection" button that fires `GET /api/health` and shows inline success/failure feedback.

On the backend, a new `POST /api/walk-sessions` endpoint accepts session payloads from the app and persists them as `WalkSession` records. The endpoint is unauthenticated (LAN-only) and idempotent on `ClientId`.

## Technical Approach

### Android App

**Language / UI:** Kotlin + Jetpack Compose. Direct access to Android sensor and service APIs with no cross-platform abstraction layer.

**Step counting:** `SensorManager` + `Sensor.TYPE_STEP_COUNTER`. This sensor returns cumulative steps since last reboot — the app snapshots the value at session start and computes the delta continuously. On foreground service resume after a kill, the app takes a fresh snapshot and adds to the previously checkpointed total.

**Background execution:** `ForegroundService` with `foregroundServiceType="health"`. A persistent notification shows live step count and elapsed time while a walk is in progress. The system notification channel is created on first launch.

**Local storage:** Room database (SQLite) for both sessions and settings. No secondary storage mechanism.

- `sessions` table: stores `WalkSession` rows with a nullable `serverSyncedAt` to track pending vs. synced state. Once synced, the row is removed from the in-app history view (but retained in the DB).
- `settings` table: simple key-value rows (e.g. `backend_url`).

**Crash recovery:** Every 30 seconds during an active walk, the foreground service writes current step count and elapsed time to an "in-progress" row in Room. On app open, if an incomplete row exists, the user is presented with a recover/discard prompt.

**Sync:** On app open and on "Sync Now" button tap, the app queries Room for all sessions where `serverSyncedAt IS NULL` and POSTs them one at a time to the backend. A 200 response (including idempotent re-submission) marks the session synced locally. Network errors are silently swallowed; the session remains pending.

**Distribution:** Debug or release APK built via Android Studio, transferred to the device via ADB sideload or USB file copy.

### Backend

**New model:** `WalkSession` in `Common/Models/`.

```
WalkSession {
  Id             string    // GUID, generated by backend
  ClientId       string    // GUID, generated by the Android app (deduplication key)
  StartedAt      DateTime  // UTC, set by the device
  DurationSeconds int
  StepCount      int
  SyncedAt       DateTime  // UTC, set by backend on receipt
}
```

**New controller:** `WalkSessionController` at `POST /api/walk-sessions`. If a session with the given `ClientId` already exists, returns 200 with the existing record (idempotent). Otherwise creates and returns the new record.

**EF migration:** Add `WalkSessions` `DbSet` to `AppDbContext` and generate migration via `dotnet ef migrations add AddWalkSessions`.

**No GET endpoint in v1.** The RPG integration (separate PRD) will read `WalkSession` records directly via `ICrud<WalkSession>` server-side.

### Key Decisions

| Decision | Chosen Approach | Rationale |
|---|---|---|
| Android tech stack | Kotlin + Jetpack Compose | Direct access to sensor/service APIs; no abstraction overhead for a single-platform app |
| Background step counting | ForegroundService (health type) | Only reliable way to keep counting steps with screen off on modern Android |
| Local storage | Room (SQLite) for everything | Handles both session list and settings; no need for a second storage mechanism |
| LAN discovery | User-configured URL in settings | Simpler than mDNS; home server IP is stable; Test Connection button mitigates misconfiguration |
| Authentication | None on walk-sessions endpoint | LAN-only; existing auth system is still partially stubbed; easy to add a token field to settings later |
| Duplicate handling | Idempotent on ClientId (200 + existing record) | Simplifies retry logic; double-submission on home LAN is low-probability |
| Sync trigger | On app open + manual button | No background sync to preserve battery on an older device |
| Crash recovery | 30-second Room checkpoint + recover/discard prompt | Prevents step loss under memory pressure without requiring continuous writes |
| History screen scope | Pending sessions only | Synced sessions are no longer actionable; list stays manageable over time |
| Target Android version | minSdk 31 (Android 12) | Matches the target device; all required APIs available without compatibility shims |

### Dependencies

- Android `Sensor.TYPE_STEP_COUNTER` (hardware sensor, available API 19+)
- Jetpack Room, Compose, and Lifecycle libraries
- HomeHub backend running on the local LAN
- `GET /api/health` endpoint (already exists) for the settings connection test
- `ICrud<WalkSession>` pattern (already established in the backend) for the RPG integration to consume later

## Open Questions

- [x] **Notification stop action:** Include a "Stop Walk" action button directly on the foreground notification. Resolved: yes, include it.
- [x] **Resume UX:** When recovering a crashed session, show a subtle "Resumed from checkpoint" label on the walk screen so the user understands why the step count starts at a non-zero value. Resolved: show the indicator.
- [x] **RPG integration access pattern:** The RPG integration reads `WalkSession` records via `ICrud<WalkSession>` directly server-side. It has no dependency on the Android app or the HTTP endpoint. Resolved: confirmed.

## Out of Scope

- GPS / route tracking (may be a future v2 addition)
- Distance estimation from steps
- Step goal setting or daily summaries
- Push notifications or reminders to go for a walk
- HomeHub web dashboard widget for walk history (separate ideation pending)
- Play Store listing or remote distribution

## Success Metrics

- A completed walk produces a `WalkSession` record in the HomeHub database with accurate step count and duration
- Sessions recorded while off the home network sync successfully on next app open
- A walk of 30+ minutes with the screen off counts steps without interruption
- A force-killed mid-walk session is recoverable with no step loss beyond the last 30-second checkpoint

## Timeline / Milestones

_TBD_
