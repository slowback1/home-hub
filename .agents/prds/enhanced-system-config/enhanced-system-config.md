# PRD: Enhanced System Config — Typed Fields & Select Options

## Status

`Draft`

## Overview

Extend the existing SystemConfig infrastructure to support typed config fields. Currently all config values are stored and rendered as plain text strings. This enhancement adds a `Type` discriminator (`text` | `select`) to each config entry, a new `SystemConfigOptions` child table for select-type fields, and updates the admin UI to render each field appropriately. Namespace continues to serve as the grouping mechanism, now displayed as a Title Case section header. Individual keys are displayed as human-friendly labels via snake_case → Title Case conversion.

This is prerequisite infrastructure for the Weather Widget PRD, which requires a `weather::provider` select field, but is designed to serve all future config domains (e.g. Retro Achievements).

## Problem Statement

Every config entry is currently rendered as a plain text input on the System Config admin page. Fields with constrained value sets (e.g. a provider selector, a units toggle) have no affordance to guide the user toward valid values — they must know the exact strings to type. As more features add config entries, this will become increasingly error-prone.

## Goals

- Allow config entries to declare a `Type` of `text` or `select`
- `select`-type entries expose their valid options via a `SystemConfigOptions` child table
- Admin UI renders `select` entries as a `<select>` dropdown, with options loaded alongside the config
- Namespace is rendered as a Title Case section header (e.g. `weather` → "Weather")
- Config keys are rendered as Title Case labels (e.g. `zip_code` → "Zip Code")
- Existing `text`-type behavior (plain text input, secret masking) is fully preserved

## Non-Goals

- Additional types beyond `text` and `select` (e.g. `boolean`, `number`) — add when needed
- Removing or replacing the `IsSecret` field (secret masking stays as-is)
- Inline editing of select options from the UI (options are managed via migrations)
- Creating or deleting config keys from the UI

## User Stories / Use Cases

- **As a** home hub user, **I want** constrained config fields to render as dropdowns, **so that** I can only select valid values without memorising string constants.
- **As a** home hub user, **I want** config sections labelled by domain (e.g. "Weather"), **so that** I can quickly navigate to the relevant group as config grows.
- **As a** home hub user, **I want** field labels derived from the key name, **so that** the page is readable without consulting documentation.

## E2E Scenarios

```gherkin
@admin
Feature: Enhanced System Config

  @system-config-select-renders-as-dropdown
  Scenario: A select-type config field renders as a dropdown
    Given I navigate to the admin system config page
    Then the "Provider" field in the "Weather" section should render as a dropdown
    And the dropdown should contain "Mock" and "Open Weather Map" as options

  @system-config-select-saves
  Scenario: Changing a select dropdown saves the new value
    Given I navigate to the admin system config page
    When I change the "Provider" dropdown to "Open Weather Map"
    Then the "Provider" field should display "Open Weather Map"
    And I should see a success toast

  @system-config-section-headers
  Scenario: Namespaces appear as Title Case section headers
    Given I navigate to the admin system config page
    Then I should see a "Weather" section header

  @system-config-key-labels
  Scenario: Config keys appear as Title Case labels
    Given I navigate to the admin system config page
    Then I should see a "Zip Code" label in the "Weather" section
    And I should see an "Api Key" label in the "Weather" section
```

## Proposed Solution

### Backend

1. **`SystemConfigOption` entity** — new model with `SystemConfigId` (FK to `SystemConfig.Id`), `Value` (string), and `Label` (string).
2. **EF migration** — adds the `SystemConfigOptions` table and populates `Type` on existing `SystemConfigs` rows:
   - `weather::zip_code` → `Type = "text"`
   - `weather::api_key` → `Type = "secret"` (retain `IsSecret = true`)
   - Seed rows for `weather::provider` and `weather::units` (added by the Weather Widget PRD migration) will carry `Type = "select"` with their options inserted into `SystemConfigOptions`.
3. **`ISystemConfigProvider`** — extend `GetAllAsync` response to include the `Type` field and associated `Options` list for each entry.
4. **`SystemConfigEntry` DTO** — add `Type` (string) and `Options` (`List<SystemConfigOptionDto>`) fields.

### Frontend

1. **Config field rendering** — the admin page inspects `Type` on each entry:
   - `"text"` or unset → existing plain text input behaviour
   - `"secret"` or `IsSecret = true` → existing masked input + show/hide toggle
   - `"select"` → `<select>` element populated with the entry's `Options`; save on change (no separate Save button for selects — the value is always valid)
2. **Section headers** — namespace converted to Title Case via a simple utility (`weather` → `"Weather"`, `retro_achievements` → `"Retro Achievements"`).
3. **Field labels** — key converted to Title Case in the same utility (`zip_code` → `"Zip Code"`).

## Technical Approach

### Key Decisions

| Decision | Chosen Approach | Rationale |
|----------|----------------|-----------|
| Type values | `text`, `secret`, `select` | Covers all current use cases; easy to extend |
| `IsSecret` vs `Type = "secret"` | Keep both, `IsSecret` controls masking, `Type` controls input widget | Avoids a data migration on existing secret rows; they are complementary concerns |
| Options storage | `SystemConfigOptions` child table | Relational FK keeps options close to their parent; manageable via migrations |
| Select save UX | Save on change (no explicit Save button) | Select values are always valid; avoids an unnecessary confirm step |
| Label derivation | `snake_case → Title Case` utility function | Zero schema changes; readable output for all expected key formats |
| Group derivation | Namespace → Title Case (same utility) | Namespace already segments entries by domain; no new column needed |

### Dependencies

- `system-config-infrastructure` PRD (completed): `SystemConfig` entity, `ISystemConfigProvider`, `EfSystemConfigProvider`
- `system-config-ui` PRD (completed): admin page at `/admin/system-config`, existing grouping by namespace, secret masking, inline edit UX
- Existing `Select` UI component in `/frontend/src/lib/ui/inputs/`

## Open Questions

_None._

## Out of Scope

- Additional `Type` values (`boolean`, `number`, `url`, etc.)
- Editing select options from the UI
- Sorting or searching config entries

## Success Metrics

- All four E2E scenarios pass
- `weather::provider` renders as a dropdown on the System Config page with the correct options
- All existing System Config E2E scenarios continue to pass

## Timeline / Milestones

_TBD_
