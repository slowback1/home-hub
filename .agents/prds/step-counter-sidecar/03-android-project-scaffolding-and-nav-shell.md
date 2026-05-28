# Android Project Scaffolding + Nav Shell

## Status

`pending`

## Description

Create the SlowWalk Android project and wire up the three-tab bottom navigation shell. This task produces a runnable app that navigates between placeholder Walk, History, and Settings screens — the foundation every subsequent task builds on.

## Acceptance Criteria

- [ ] A new Android project named "SlowWalk" exists in the repo (e.g. `android/slowwalk/`)
- [ ] `minSdk = 31`, `targetSdk = 35` (or current stable), Kotlin + Jetpack Compose
- [ ] Gradle dependencies declared: Jetpack Compose BOM, Room (runtime + KSP), Retrofit + Gson converter, Kotlin Coroutines
- [ ] Bottom navigation bar with three tabs: Walk, History, Settings — each showing a placeholder composable with the tab name
- [ ] The app builds and installs via `./gradlew installDebug` (or equivalent) with no errors
- [ ] App name displayed on the device is "SlowWalk"

## Notes

Use the standard Android Studio "Empty Activity" template as a starting point, or scaffold by hand. The project root should be `android/slowwalk/` relative to the repo root.

Navigation: use Jetpack `NavHost` + `NavController` with `NavigationBar` / `NavigationBarItem`. Three routes: `walk`, `history`, `settings`.

KSP (Kotlin Symbol Processing) is required for Room annotation processing — add the KSP Gradle plugin alongside the Room compiler dependency.
