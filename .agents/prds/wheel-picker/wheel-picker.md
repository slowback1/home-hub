# PRD: Wheels (Wheel Picker)

## Status

`Draft`

## Overview

Wheels is a small HomeHub utility for making unbiased random choices from reusable, named lists — inspired by wheelofnames.com's "saved wheels" feature. A user can create, edit, and delete "wheels" (a wheel is just a named list of text items), then select a saved wheel and "spin" it to have one item chosen at random. V1 skips the spinning animation and simply surfaces the chosen item. The feature lives on a single `/wheels` page with two sections (manage + spin) and ships with a quick-spin dashboard widget.

## Problem Statement

Households frequently need to make an unbiased random choice from a *recurring* set of options — what to eat for dinner, whose turn it is for a chore, which movie to watch. General-purpose randomizers force you to retype the same list every time. HomeHub already positions itself as a household utility hub (chores, activity picker, bookmarks), and a place to **save** reusable named lists and pick from them on demand removes that repeated friction. The cost of not solving it is minor but real: the household keeps reaching for an external tool (wheelofnames.com) for a job HomeHub is otherwise well-suited to own.

## Goals

- Let a user CRUD "wheels" — named lists of plain-text items — that persist across sessions.
- Let a user select a saved wheel and get one uniformly-random item on demand ("spin").
- Provide a quick-spin dashboard widget for the highest-value action without leaving the dashboard.
- Follow existing HomeHub conventions (flat `ICrud<T>` entity, feature-flag gating, sidebar nav + widget registry, EF migration via `dotnet ef`, Playwright BDD coverage).

## Non-Goals

- **No spinning animation** in V1 — the result is displayed directly.
- **No pick history / persistence of results** — the chosen item is shown transiently and forgotten on reload.
- **No per-item weighting** — every item has equal odds (a user may add a duplicate line to bias the odds manually).
- **No "no-repeat until exhausted"** mode.
- **No sharing/exporting** wheels.
- **No per-user scoping** — wheels are global to the HomeHub instance, consistent with existing features.

## User Stories / Use Cases

- **As a** household member, **I want to** save a named list of options once, **so that** I don't have to retype it every time I need a random pick.
- **As a** household member, **I want to** spin a saved wheel and see a randomly chosen item, **so that** I can make an unbiased decision quickly.
- **As a** household member, **I want to** quick-spin a wheel right from the dashboard, **so that** I can decide without navigating to a dedicated page.
- **As a** household member, **I want to** edit or delete wheels as my options change, **so that** my saved lists stay relevant.

## E2E Scenarios

```gherkin
@wheels
Feature: Wheels

  @wheels-create
  Scenario: Create a wheel with items
    Given the WHEEL_PICKER_ENABLED feature flag is enabled
    When I visit the Wheels page
    And I create a wheel named "Dinner" with items "Pizza", "Tacos", "Sushi"
    Then I see a wheel named "Dinner" in the manage list
    And the "Dinner" wheel shows 3 items

  @wheels-edit
  Scenario: Edit a wheel's name and items
    Given the WHEEL_PICKER_ENABLED feature flag is enabled
    And a wheel named "Dinner" exists with items "Pizza", "Tacos"
    When I visit the Wheels page
    And I edit the "Dinner" wheel to be named "Dinner Options" with items "Pizza", "Tacos", "Ramen"
    Then I see a wheel named "Dinner Options" in the manage list
    And the "Dinner Options" wheel shows 3 items

  @wheels-delete
  Scenario: Delete a wheel
    Given the WHEEL_PICKER_ENABLED feature flag is enabled
    And a wheel named "Dinner" exists with items "Pizza", "Tacos"
    When I visit the Wheels page
    And I delete the "Dinner" wheel
    Then I do not see a wheel named "Dinner" in the manage list

  @wheels-empty-state
  Scenario: Page shows empty state when no wheels exist
    Given the WHEEL_PICKER_ENABLED feature flag is enabled
    When I visit the Wheels page
    Then I see the wheels empty state

  @wheels-spin
  Scenario: Spin a saved wheel and see a result from its items
    Given the WHEEL_PICKER_ENABLED feature flag is enabled
    And a wheel named "Dinner" exists with items "Pizza", "Tacos", "Sushi"
    When I visit the Wheels page
    And I select the "Dinner" wheel in the spin section
    And I click Spin
    Then I see a spin result that is one of "Pizza", "Tacos", "Sushi"

  @wheels-spin-empty-disabled
  Scenario: Spin is disabled for a wheel with no items
    Given the WHEEL_PICKER_ENABLED feature flag is enabled
    And a wheel named "Empty" exists with no items
    When I visit the Wheels page
    And I select the "Empty" wheel in the spin section
    Then the Spin button is disabled

  @wheels-widget-quick-spin
  Scenario: Quick-spin a wheel from the dashboard widget
    Given the WHEEL_PICKER_ENABLED feature flag is enabled
    And the wheels widget is on the dashboard
    And a wheel named "Dinner" exists with items "Pizza", "Tacos", "Sushi"
    When I am on the dashboard
    And I select the "Dinner" wheel in the wheels widget
    And I click Spin in the wheels widget
    Then I see a widget spin result that is one of "Pizza", "Tacos", "Sushi"

  @wheels-feature-flag-hidden
  Scenario: Wheels is hidden when feature flag is disabled
    Given the WHEEL_PICKER_ENABLED feature flag is disabled
    When I am on the dashboard
    Then the Wheels nav item is not visible in the sidebar
    And the wheels widget is not available in the widget picker
```

## Proposed Solution

A single feature-flagged `/wheels` page with two sections:

1. **Manage wheels** — a list of saved wheels (name + item count) with create / edit / delete. Item entry uses a wheelofnames-style textarea: one item per line. Blank lines are trimmed and ignored.
2. **Spin** — a wheel selector (dropdown of saved wheels) plus a "Spin" button. Clicking Spin chooses one item uniformly at random **in the frontend** (items are already loaded) and displays it. The Spin button is disabled when the selected wheel has zero items.

A **quick-spin dashboard widget** reuses the same frontend spin logic: a compact wheel selector + Spin button + inline result.

Backend is a single flat `Wheel` entity persisted via the generic `ICrud<Wheel>`, with items stored as a delimited string column. A standard `WheelController` exposes list/create/update/delete. No spin endpoint and no history table are needed. The feature is gated behind a `WHEEL_PICKER_ENABLED` flag seeded `false`, registered in the sidebar nav and widget registry exactly like Walk History.

## Design

**Handoff:** _TBD — to be generated via the `design-brief` skill / Claude Design before implementation._

_Key visual decisions from the design session:_

- _TBD_

## Technical Approach

### Key Decisions

| Decision | Chosen Approach | Rationale |
|----------|-----------------|-----------|
| Item storage | Newline-delimited string column on the `Wheel` entity (one item per line) | Keeps a single flat entity that fits the generic `ICrud<Wheel>`; matches the simple plain-text, equal-weight V1; textarea input round-trips directly (split on `\n`, trim, drop blanks). Chosen over a child `WheelItem` table since no weighting/per-item metadata is needed yet. |
| Random selection location | Frontend (`Math.random`) | Items are already loaded in the browser; no new endpoint needed; no animation in V1. |
| Pick history | None | Consistent with the no-child-table decision and with wheelofnames' transient result behavior. |
| Page structure | Single `/wheels` route with two sections (manage + spin) | Matches the requested UX; the feature is small enough for one route. |
| Dashboard widget | Quick-spin (wheel selector + Spin + inline result) | Highest-value dashboard action; reuses the frontend spin logic with no extra persistence. |
| Wheel name uniqueness | Not enforced; name required (non-empty, trimmed) | Mirrors the existing `Activity` controller's validation. |
| Duplicate items | Allowed | A legitimate manual way to weight odds, à la wheelofnames. |
| Empty wheel | Can be saved with zero items; only *spinning* requires ≥1 item (Spin disabled otherwise) | Lets a user create a wheel and fill it later; avoids an error state. |
| Feature gating | `WHEEL_PICKER_ENABLED` flag, seeded `false` via an `InsertData` migration | Mirrors `WALK_SESSION_HISTORY_ENABLED`. |

### Naming & Placement

- **Route / page:** `/wheels`
- **Sidebar + widget name:** "Wheels" (widget id `wheels`)
- **Feature flag:** `WHEEL_PICKER_ENABLED`
- **Icon:** `Disc3` (lucide) — `Shuffle` is taken by Activity Picker.

### Components to Build (mirroring the Walk History feature)

**Backend**
- `Common/Models/Wheel.cs` — `Id`, `Name`, `Items` (delimited string), `CreatedAt`.
- Register `DbSet<Wheel>` in `AppDbContext`.
- `WheelController` — `GET` list, `POST` create, `PUT`/`PATCH` update, `DELETE`.
- EF migration adding the `Wheels` table (via `dotnet ef`).
- Seed migration inserting `WHEEL_PICKER_ENABLED = false` into `FeatureFlags`.
- Test-cleanup endpoint `DELETE /api/test/wheels` in `TestHelperController`.

**Frontend**
- `lib/api/WheelApi.ts` — CRUD client (mirrors `ActivityApi`).
- `routes/wheels/+page.svelte` — manage section + spin section.
- `lib/services/Dashboard/widgets/WheelsWidget.svelte` — quick-spin widget.
- Register the widget in `widgetRegistry.ts` and the flag in `FeatureFlags.ts`; sidebar nav derives from the widget registry.

**E2E**
- `features/wheels.feature`, `steps/wheels.steps.ts`, `pages/WheelsPage.ts`, fixture wiring.

### Dependencies

- Existing HomeHub infrastructure: `ICrud<T>` / `ICrudFactory`, `AppDbContext`, feature-flag system, dashboard slot/widget registry + sidebar nav, `TestHelperController`, Playwright BDD harness.
- `lucide-svelte` for the icon.
- No external services.

## Open Questions

_All resolved during refinement:_

- [x] **Delimiter for the `Items` column:** newline-delimited (`"a\nb\nc"`). Split on `\n`, trim each line, drop blanks — round-trips directly with the textarea. Items are single-line trimmed text, so the delimiter can't collide.
- [x] **Icon:** `Disc3` (reads as a spinnable wheel; distinct from Activity Picker's `Shuffle`).
- [x] **Quick-spin widget selection memory:** none — the widget always defaults to the first saved wheel on load. Keeps it stateless, consistent with the no-persistence theme. Session-memory is a possible post-V1 nicety.

## Out of Scope

- Spinning animation.
- Pick history / result persistence.
- Per-item weighting and "no-repeat until exhausted" modes.
- Sharing, exporting, or importing wheels.
- Per-user scoping of wheels.

## Success Metrics

- A user can create a wheel, spin it, and get a valid in-list result end-to-end (all E2E scenarios green).
- The feature is fully gated: with the flag off, no nav item and no widget-picker entry appear.
- Household stops reaching for wheelofnames.com for recurring lists (qualitative).

## Timeline / Milestones

_TBD_
