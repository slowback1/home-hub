# PRD: Feature Flags Admin UI

## Status

`Draft` <!-- Draft | Review | Approved | Superseded -->

## Overview

An admin page at `/admin/feature-flags` for viewing and toggling database-backed feature flags at runtime, without direct DB access. The page lives alongside the existing System Config page under a shared admin tab bar.

## Problem Statement

Feature flags are managed exclusively through database migrations and direct SQL updates. Toggling a flag requires either a new migration or a manual `UPDATE` query — neither is suitable for routine runtime management. An admin UI closes this gap and makes flag management accessible to anyone on the home network.

## Goals

- The admin section gains a shared tab bar with "System Config" and "Feature Flags" tabs
- The feature flags page lists all known flags with their current enabled/disabled state
- Each flag can be toggled on or off with a single click, writing to the database immediately
- Failed toggles show a toast error and revert the switch to its previous state
- Flag names are displayed in human-readable form (auto-formatted from `SCREAMING_SNAKE_CASE`)
- The "Admin" sidebar item is active when navigating to either admin page

## Non-Goals

- Creating, deleting, or renaming flags from the UI (flags are still added via EF migrations)
- Per-user or percentage-based flag targeting
- Authentication or role-based access control
- Audit log or history of flag changes
- Flag descriptions or rich metadata beyond name and enabled state
- Server-side caching of flag values

## User Stories / Use Cases

- **As a** home hub user, **I want to** see all feature flags and their current state, **so that** I can understand which features are active without touching the database.
- **As a** home hub user, **I want to** toggle a feature flag on or off with a single click, **so that** I can enable or disable a feature without a code deploy or direct DB access.
- **As a** home hub user, **I want to** navigate between System Config and Feature Flags via tabs, **so that** all admin tools are discoverable in one place.

## E2E Scenarios

```gherkin
@admin
Feature: Feature Flags Admin UI

  @feature-flags-page-loads
  Scenario: Page loads and displays all feature flags
    Given I navigate to the admin feature flags page
    Then I should see the page heading "Feature Flags"
    And I should see a flag row for "Demo Feature Flag"
    And the toggle for "Demo Feature Flag" should be off

  @feature-flags-toggle-on
  Scenario: Toggling a flag on enables it immediately
    Given I navigate to the admin feature flags page
    And the toggle for "Demo Feature Flag" is off
    When I toggle the switch for "Demo Feature Flag" on
    Then the toggle for "Demo Feature Flag" should be on
    And I should see a success toast

  @feature-flags-toggle-off
  Scenario: Toggling a flag off disables it immediately
    Given I navigate to the admin feature flags page
    And the toggle for "Demo Feature Flag" is on
    When I toggle the switch for "Demo Feature Flag" off
    Then the toggle for "Demo Feature Flag" should be off
    And I should see a success toast

  @feature-flags-tab-navigation
  Scenario: Navigating between admin tabs
    Given I navigate to the admin system config page
    When I click the "Feature Flags" tab
    Then I should be on the admin feature flags page
    When I click the "System Config" tab
    Then I should be on the admin system config page

```

## Proposed Solution

Add a `PATCH /api/feature-flags/{name}` endpoint to `FeatureFlagController` that updates the `IsEnabled` field for the named flag. Add a shared `/admin/+layout.svelte` that renders a two-tab bar ("System Config" | "Feature Flags") above all admin pages. Add a new `/admin/feature-flags/+page.svelte` that fetches all flags on mount and renders each as a labeled row with a `ToggleSwitch`. Toggling a switch immediately calls the PATCH endpoint; on failure the switch reverts and a toast error appears. Flag names are formatted on the frontend by replacing underscores with spaces and applying title case.

Update the sidebar "Admin" nav item's active detection to match the `/admin` path prefix so it highlights correctly for both admin pages.

## Technical Approach

### Key Decisions

| Decision | Chosen Approach | Rationale |
|---|---|---|
| Toggle endpoint | `PATCH /api/feature-flags/{name}` with `{"isEnabled": bool}` | Partial update semantics; only the changed field is sent |
| UI update strategy | Pessimistic — wait for API response before confirming | Keeps UI truthful to DB state; toggle is fast enough that no lag is perceived |
| Error recovery | Revert switch + toast on failure | Consistent with system-config save error pattern |
| Flag name formatting | Frontend only: replace `_` with space, apply title case | No DB migration needed; sufficient for a small, developer-owned flag set |
| Admin tab nav | Shared `/admin/+layout.svelte` using existing `Tabs` component | SvelteKit-idiomatic; avoids duplicating tab markup in each page |
| Sidebar active detection | Update Admin nav item href to `/admin` prefix match | Ensures the Admin item highlights for both `/admin/system-config` and `/admin/feature-flags` |
| Toggle component | Existing `ToggleSwitch` component | Already in the component library; no new component needed |
| New backend use case | `UpdateFeatureFlagUseCase` in `Logic/FeatureFlags/` | Mirrors existing layered pattern; keeps controller thin |
| E2E provider | `DictionaryFeatureFlagProvider` already supports reads; extend for writes in E2E mode | Avoids real database dependency in E2E tests |

### API Changes

**New endpoint:**
```
PATCH /api/feature-flags/{name}
Body:     { "isEnabled": bool }
200 OK:   { "name": string, "isEnabled": bool }
404:      flag not found
```

**Unchanged:**
```
GET /api/feature-flags
```

### Backend Layer Changes

- New `IFeatureFlagRepository` interface in `Common/Interfaces/` with `UpdateAsync(name, isEnabled)` method
- New `EntityFrameworkFeatureFlagRepository` in `backend/EntityFramework/` that updates `IsEnabled` via `AppDbContext`
- New `UpdateFeatureFlagUseCase` in `Logic/FeatureFlags/` wrapping the repository call
- `FeatureFlagController` gains a `PATCH /{name}` action wired to `UpdateFeatureFlagUseCase`
- E2E: `DictionaryFeatureFlagProvider` extended to handle write operations in the E2E environment (mirrors the `DictionarySystemConfigProvider` pattern)

### Frontend Architecture

- New `FeatureFlagApi` in `frontend/src/lib/api/FeatureFlagApi.ts`:
  - `getAll()` → `GET /api/feature-flags`
  - `toggle(name, isEnabled)` → `PATCH /api/feature-flags/{name}`
- New `/admin/+layout.svelte` rendering the `Tabs` component with "System Config" (`/admin/system-config`) and "Feature Flags" (`/admin/feature-flags`) items
- New `/admin/feature-flags/+page.svelte`:
  - Fetches all flags on mount; shows `Spinner` while loading
  - Renders each flag as a row: formatted name (left) + `ToggleSwitch` (right)
  - On toggle: call `FeatureFlagApi.toggle()`, revert switch + toast on error
- Sidebar: update Admin nav item so active detection matches `/admin` prefix (both admin pages stay highlighted)

### Dependencies

- `feature-flag-system` PRD — `FeatureFlags` table, `GET /api/feature-flags` endpoint, `IFeatureFlagProvider` interface, and `DictionaryFeatureFlagProvider` must already be implemented
- `system-config-ui` PRD — `/admin/system-config` page must exist (the tab bar wraps it)
- Existing `ToastWrapper` and toast messaging infrastructure
- Existing `ToggleSwitch`, `Tabs`, `Spinner` UI components

## Open Questions

_None — all decisions resolved during planning session._

## Out of Scope

- Creating, deleting, or renaming flags from the UI
- Per-user or percentage-based targeting
- Flag descriptions or rich metadata
- Audit log of flag changes
- Authentication or role-based access control
- Server-side caching of flag values
- String or numeric flag variants

## Success Metrics

- All four E2E scenarios pass
- Toggling a flag via the UI and refreshing the page shows the updated state
- The "Admin" sidebar item is active when on either `/admin/system-config` or `/admin/feature-flags`
- The `Tabs` component correctly highlights the active tab based on the current route

## Timeline / Milestones

_TBD_
