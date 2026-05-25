# Step Counter Sidecar

**Status:** stub
**Created:** 2026-05-25

## Summary

A native Android app (sideloaded APK, no Play Store) that records walk sessions using the device step counter sensor, stores sessions locally when offline, and syncs them to the HomeHub backend when back on the local network.

## Problem / Opportunity

Character progression in the HomeHub RPG should be driven by real-world activity. Walking is the primary physical activity to reward, but there is no mechanism today for HomeHub to know that a walk happened or how long/far it was. This sidecar provides that data feed.

## Success Looks Like

- User opens the app, taps "Start Walk," and the app begins counting steps via the Android `StepCounter` sensor
- Steps and elapsed time are displayed in real time during the session; counting continues in the background while the screen is off
- Session data (step count, duration, start time) is persisted on-device if there is no network
- When the device reconnects to the home network, the session syncs automatically (or on demand) to a new `/api/walk-sessions` backend endpoint
- A backend record exists for each synced session that the RPG integration can read

## Notes & Open Questions

- **Platform decision (settled)**: Native Android only. PWA was ruled out because `WakeLock` doesn't solve the background processing problem well enough for reliable step counting while the screen is off. Native gives access to the `StepCounter` sensor service which counts steps silently in the background with no screen-on requirement.
- **Distribution**: No Play Store. Build a debug or release APK with Android Studio and manually transfer to device (ADB sideload or USB file copy). Removes signing/store compliance overhead entirely.
- **Tech stack TBD**: Kotlin + Jetpack Compose is the modern Android default. Could also consider React Native or Flutter if cross-platform is ever desired later, but single-platform native is simplest for now.
- **Sync mechanism**: Assumes HomeHub backend is on a local LAN — no remote sync needed. Simplest approach is a manual "Sync now" button plus automatic sync on app open when network is available.
- **No GPS / route tracking in v1** — just step count + duration. Distance can be estimated from steps if needed.
- **Depends on**: Backend `/api/walk-sessions` endpoint (new, does not exist). Step counter RPG integration stub.
