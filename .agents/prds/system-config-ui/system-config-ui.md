# PRD: System Config Admin UI

## Status

`Draft`

## Overview

An admin page at `/admin/system-config` that lets anyone on the home network view and edit database-backed system configuration values at runtime, without requiring a redeploy or direct database access. Config entries are grouped by namespace, secrets are masked by default, and changes are saved inline via the existing `SystemConfigController` API.

## Problem Statement

Once config values live in the database (delivered by `system-config-infrastructure`), there is no safe, discoverable way to read or update them without direct DB access. An admin UI page solves this for day-to-day operation of the home hub.

## Goals

- Display all system config entries grouped by namespace
- Allow inline editing of any value with explicit Save and Cancel actions
- Mask secret values by default with a show/hide toggle
- Show a success or error toast after every save attempt
- Seed the two weather config entries (`weather/zip_code`, `weather/api_key`) as default data so the weather widget feature has its config keys ready
- Wire a `DictionarySystemConfigProvider` codepath for E2E mode so tests run without a real database

## Non-Goals

- Authentication or role-based access control (deferred; the page is accessible to any user)
- Typed inputs based on the `Type` field (all values treated as plain text strings)
- Displaying or editing file-sourced (appsettings) config values
- Pagination or search (not needed at home-hub scale)

## User Stories / Use Cases

- **As a** home hub user, **I want to** navigate to an admin page, **so that** I can see all current config values without touching the database.
- **As a** home hub user, **I want to** click a config value and edit it in place, **so that** I can update runtime settings without a redeploy.
- **As a** home hub user, **I want** secret values masked by default, **so that** API keys are not visible at a glance on screen.
- **As a** home hub user, **I want** a toast notification after saving, **so that** I know whether the update succeeded or failed.

## E2E Scenarios

```gherkin
@admin
Feature: System Config Admin

  @system-config-page-loads
  Scenario: Page loads and displays config entries grouped by namespace
    Given I navigate to the admin system config page
    Then I should see a "weather" namespace section
    And I should see a "zip_code" entry with value "10001"
    And I should see an "api_key" entry with a masked value

  @system-config-edit-happy-path
  Scenario: Editing a config value and saving updates the value
    Given I navigate to the admin system config page
    When I click on the "zip_code" value
    Then an inline text input with Save and Cancel buttons should appear
    When I type "90210" and click Save
    Then the "zip_code" entry should display "90210"
    And I should see a success toast

  @system-config-edit-cancel
  Scenario: Cancelling an edit restores the original value
    Given I navigate to the admin system config page
    When I click on the "zip_code" value and type "99999"
    And I click Cancel
    Then the "zip_code" entry should display "10001"

  @system-config-secret-masked
  Scenario: Secret values are masked and can be revealed
    Given I navigate to the admin system config page
    Then the "api_key" value should be masked
    When I click the show toggle on the "api_key" entry
    Then the "api_key" value should display "test-api-key-1"

  @system-config-save-error
  Scenario: A failed save shows an error toast
    Given I navigate to the admin system config page
    When I click on the "zip_code" value and the API returns an error on save
    Then I should see an error toast
    And the input should remain open
```

## Proposed Solution

A new SvelteKit route at `/admin/system-config` fetches all config entries from `GET /api/system-config` on load. Entries are grouped by `Namespace` and rendered in a table. Clicking a value in the table switches that row to edit mode, showing a text input with Save and Cancel buttons. Save calls `PUT /api/system-config/{namespace}/{key}` and shows a toast on completion. Cancel reverts the row to display mode. Secret entries show masked values with a show/hide toggle.

The sidebar gains an "Admin" nav item (Settings icon, `/admin/system-config`).

On the backend, a new `ASPNETCORE_ENVIRONMENT=E2E` environment triggers `appsettings.E2E.json`, which registers `DictionarySystemConfigProvider` instead of `EfSystemConfigProvider`. The dictionary is pre-seeded with the weather config defaults; writes are handled by the existing InMemory CRUD implementation. The E2E task runner is updated to pass this environment variable when spinning up the backend.

A data migration seeds `weather/zip_code` and `weather/api_key` as default values in the production/dev database.

## Technical Approach

### Key Decisions

| Decision | Chosen Approach | Rationale |
|---|---|---|
| Access control | None | Home system; adding a role system is its own lift |
| Edit UX | Click-to-edit inline with explicit Save/Cancel buttons | Fastest for scanning and updating a list |
| Secret masking | `IsSecret = true` → masked by default, show/hide toggle | `IsSecret` already on the model; no backend changes needed |
| Grouping | Group rows by `Namespace` with section headers | `Namespace` already on every entry; aids readability as config grows |
| Value types | All plain text strings | `Type` field unpopulated; typed inputs deferred |
| Save feedback | Toast on success and error (existing `ToastWrapper`) | Consistent with app patterns; zero extra infrastructure |
| Load state | Spinner while fetching; inline error message on failure | `Spinner` component already in UI library |
| E2E provider | `DictionarySystemConfigProvider` via `ASPNETCORE_ENVIRONMENT=E2E` + `appsettings.E2E.json`; env var set in `Taskfile.yml` | Avoids real database dependency in E2E; writes handled by InMemory CRUD |
| Default seed data | EF data migration seeds `weather/zip_code = "10001"` and `weather/api_key` | Prepares config keys needed by the weather widget feature |

### Dependencies

- `system-config-infrastructure` PRD (completed): `SystemConfigController`, `ISystemConfigProvider`, `EfSystemConfigProvider`, `DictionarySystemConfigProvider`, `SystemConfig` model all exist
- Existing `ToastWrapper` and toast messaging infrastructure
- Existing `Spinner`, `Button`, `TextBox` UI components
- `lucide-svelte` `Settings` icon (already a dependency)
- InMemory CRUD implementation (existing, used for E2E write path)

## Open Questions

_None._

## Out of Scope

- Auth/role gating
- Typed inputs (boolean toggles, number inputs)
- File-sourced (appsettings) config values
- Creating or deleting config keys from the UI (read + update only)
- Pagination or filtering

## Success Metrics

- All five E2E scenarios pass
- Editing `weather/zip_code` via the UI and refreshing the page shows the updated value
- Secret values are never visible in the DOM without explicitly clicking the show toggle

## Timeline / Milestones

_TBD_
