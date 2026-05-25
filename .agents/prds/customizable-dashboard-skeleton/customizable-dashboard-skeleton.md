# PRD: Customizable Dashboard Skeleton

## Status

`Draft`

## Overview

Replace the HomeHub home page (`/`) with a configurable 3×2 widget grid. The user can assign any registered widget to any of the six slots, reorder or remove widgets, and have their layout persist across sessions. The grid is the structural skeleton only — widget components themselves are built separately under the `dashboard-widgets` PRD.

## Problem Statement

As the number of HomeHub features grows, navigating to each page individually becomes inefficient. A single overview page that surfaces at-a-glance summaries of the features the user actually cares about would reduce daily friction. The current `/` route is a dead-end placeholder with no utility.

## Goals

- Replace `/` with a functional dashboard grid.
- Let the user choose which widgets occupy which slots and persist that choice.
- Make it easy to register new widget types as future features ship.
- Support multiple grid format configurations at the data layer (even though only one format ships initially).

## Non-Goals

- Building the actual widget content components (covered by `dashboard-widgets`).
- Drag-and-drop reordering.
- Per-user layouts (single-user app; one global layout).
- Multiple simultaneous active layouts.

## User Stories / Use Cases

- **As a** HomeHub user, **I want to** see at-a-glance summaries of my chosen features on the home page, **so that** I don't have to navigate to each section individually every morning.
- **As a** HomeHub user, **I want to** pick which widgets appear on my dashboard, **so that** only the features I use are front and center.
- **As a** HomeHub user, **I want to** my layout to survive page refreshes and browser restarts, **so that** I don't have to reconfigure it every session.
- **As a** HomeHub user, **I want to** remove a widget I accidentally placed without losing it permanently, **so that** I can experiment without fear.

## E2E Scenarios

```gherkin
@dashboard
Feature: Dashboard

  @dashboard-empty-state
  Scenario: Empty dashboard shows placeholder slots
    Given I am on the home page
    Then I should see 6 empty slot placeholders
    And each placeholder should have an add widget button

  @dashboard-add-widget
  Scenario: Add a widget to an empty slot
    Given I am on the home page
    When I click the add widget button on slot 0
    Then the widget picker modal should open
    When I select a widget from the picker
    Then the modal should close
    And the widget should appear in slot 0

  @dashboard-widget-persists
  Scenario: Widget assignment persists across page reloads
    Given slot 0 has a widget assigned
    When I reload the page
    Then slot 0 should still show the same widget

  @dashboard-enter-edit-mode
  Scenario: Entering edit mode shows remove controls
    Given slot 0 has a widget assigned
    When I click the Edit Dashboard button
    Then each occupied slot should show a remove button
    And I should see a Done button

  @dashboard-remove-widget
  Scenario: Remove a widget in edit mode
    Given I am in edit mode with slot 0 occupied
    When I click the remove button on slot 0
    Then slot 0 should revert to an empty placeholder
    And the change should be saved automatically

  @dashboard-exit-edit-mode
  Scenario: Exiting edit mode hides remove controls
    Given I am in edit mode
    When I click the Done button
    Then the remove buttons should no longer be visible
```

## Proposed Solution

A SvelteKit page at `/` renders a 3×2 CSS grid of slot cells. Each cell holds either a widget component (looked up from a `WIDGET_REGISTRY`) or an empty placeholder. An "Edit Dashboard" button in the page header toggles edit mode; occupied slots in edit mode gain a red × badge. Clicking an empty slot (or "+" in edit mode) opens a modal listing available widgets filtered by feature flag. Layout is persisted to a new `DashboardLayout` backend table via a simple GET/PUT API.

## Design

**Handoff:** https://api.anthropic.com/v1/design/h/LrsVk9WuVDMXtX5n0AP-DQ?open_file=Dashboard.html

Key visual decisions from the design session:

- **First-run hero state**: When all 6 slots are empty the grid is replaced entirely by a hero panel ("Your dashboard is empty.") with an "Add your first widget" primary button and three preset-pack cards (Daily essentials / Media & generation / Everything). This is more welcoming than six dashed placeholders.
- **Edit-mode × badge**: The remove button is an iOS-jiggle-style red circle (28px) absolutely positioned offset outside the top-right corner of the card, not inside it.
- **Widget header row**: Every occupied slot has a compact header with icon + uppercase name + a `›` chevron that navigates to the feature's full page.
- **Picker modal — already-placed section**: Widgets already on the dashboard are shown greyed out with a ✓ below a "already on dashboard" divider, so the user can see the full registry without losing context.
- **Undo toast**: Removing a widget triggers a 4-second toast with an Undo button that restores the widget to its original slot.
- **Header subtitle**: Shows `{n}/6 widgets · {time-of-day greeting}` when the dashboard is populated; "Pin the modules you want to see at a glance." in the empty state.
- **Responsive breakpoints**: 2-column layout at ≤960px, single-column at ≤640px (not a hard mobile-only collapse).
- **Slot minimum height**: 260px (comfortable density) / 220px (compact density). Density is not user-configurable in v1 — default to comfortable.

## Technical Approach

### Key Decisions

| Decision | Chosen Approach | Rationale |
|----------|----------------|-----------|
| Route | `/` replaces the existing placeholder | Current page is a dead-end; no redirect needed |
| Grid layout | CSS grid, no library | No grid library installed; 3×2 fixed slots don't need drag-and-drop |
| Layout storage | New `DashboardSlot` EF Core table | Structured, queryable, survives browser clears, supports multiple formats |
| Widget registry | `WIDGET_REGISTRY` constant in frontend, feature-flag-filtered | Reuses existing `FeatureFlagService` pattern from Sidebar; no new API endpoint |
| Widget contract | Minimal: registry entry maps a slug to a `.svelte` component | Widgets own their own data fetching, consistent with all other pages |
| Empty slot default | All slots empty on first load | Avoids coupling migrations to feature-flag state |
| Edit mode | Global page-header toggle | Clean dashboard during normal use; established UX pattern |
| Widget removal | Immediate × in edit mode + undo toast | Fast, reversible, no confirmation modal needed |
| Mobile | CSS breakpoints: 2-col ≤960px, 1-col ≤640px | No separate config needed; CSS reflow is sufficient |

### Data Model

New `DashboardSlot` table (one row per slot per layout format):

```
DashboardSlot
  Id           string  (PK)
  LayoutFormat string  (e.g. "3x2", "4x3")
  SlotIndex    int     (0-based, left-to-right top-to-bottom within the format)
  WidgetType   string? (null = empty slot)
```

Unique constraint on `(LayoutFormat, SlotIndex)`. The initial format shipped is `"3x2"` (6 slots, indices 0–5).

### API

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/api/dashboard/layout` | Returns slot assignments for the active format |
| `PUT` | `/api/dashboard/layout` | Upserts all slot assignments for the active format |

Response shape: `{ layoutFormat: "3x2", slots: [{ slotIndex: 0, widgetType: "tasks" }, ...] }`. Absent slots are treated as empty.

### Frontend Widget Registry

```ts
// WIDGET_REGISTRY: maps slug → { name, description, icon, featureFlag?, component }
// Built in frontend only. Feature-flag-disabled entries are filtered out at render time.
```

Initial registry entries (all pointing to placeholder stub components until `dashboard-widgets` ships): `tasks`, `activity`, `retro`, `weather`, `audiobook`, `bookmarks`, `comfyui`.

### Dependencies

- Existing `FeatureFlagService` and `FeatureFlags` constants (frontend)
- EF Core migration for `DashboardSlot` table
- `SystemConfig` pattern (reference only — layout does not use the generic key-value store)

## Open Questions

- [x] Should `comfyui` be included in the initial registry? → Add `COMFYUI_ENABLED` feature flag. Gate the widget picker entry and the sidebar nav item on this flag, consistent with all other features.
- [x] Preset packs on the first-run hero ("Daily essentials", etc.) → Hard-coded constant arrays in the frontend component. Editorial shortcuts for first-run discoverability only; no runtime configurability needed.

## Out of Scope

- Drag-and-drop reordering (deferred; fixed-slot config covers v1 needs).
- Compact density toggle (design explored it; not shipping in v1 — default to comfortable).
- Widget accent bar ("vivid" variant from design — deferred).
- Per-user layouts.
- Mobile-specific separate layout configuration.
- Actual widget content components (see `dashboard-widgets` stub).

## Success Metrics

- Dashboard loads with correct persisted layout on every page visit.
- Adding, removing, and re-adding a widget round-trips correctly through the API.
- All 6 E2E scenarios pass.
- No widget slot shows an error state when its feature flag is disabled (slot renders as empty).

## Timeline / Milestones

_TBD_
