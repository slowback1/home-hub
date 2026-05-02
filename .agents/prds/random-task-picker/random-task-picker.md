# PRD: Random Task Picker

## Status

`Draft` <!-- Draft | Review | Approved | Superseded -->

## Overview

The Random Task Picker removes decision fatigue around unstructured free time by automatically selecting one activity per hour from a user-defined list. A display page shows the current pick and a 7-day history grid; a config sub-page lets the user manage the activity list. The hourly selection is driven by a Hangfire background job, establishing the app's background job infrastructure for future features.

## Problem Statement

Choosing what to do with free time is surprisingly draining. A randomizer that surfaces one activity per hour removes that friction and adds light structure to unstructured time. The secondary motivation is that this feature requires introducing a background job framework — a foundational piece several other planned features will depend on.

## Goals

- User can define and manage a list of activities with relative weights (1–5)
- A background job picks one activity at random (weighted) at the top of every hour
- A display page shows the current pick and a 7-day history in a calendar week view
- The display page refreshes automatically without a manual reload
- Background job infrastructure (Hangfire) is in place and reusable for future features

## Non-Goals

- Activities are not scheduled or time-boxed (no "do chores at 9am")
- No notifications or alerts when a new pick is made
- Pick frequency is not user-configurable (always hourly)
- No multi-user support; this is a single-user personal tool
- No bulk import/export of activity lists

## User Stories / Use Cases

- **As a user**, I want to add activities with weights so that I can influence how often each one is picked.
- **As a user**, I want to delete an activity so that it is removed from future picks immediately.
- **As a user**, I want to open the activity page and immediately see what I should be doing this hour without having to decide.
- **As a user**, I want to see a week's worth of past picks in a calendar grid so that I can reflect on how I've been spending my time.
- **As a user**, I want the page to stay current without refreshing so that the new pick appears automatically when the hour turns.

## E2E Scenarios

```gherkin
@activity
Feature: Activity Picker

  @add-activity-happy-path
  Scenario: Add a new activity on the config page
    Given I am on the activity config page
    When I add a new activity with name "Play Chess" and weight 3
    Then I should see "Play Chess" in the activity list with weight 3

  @change-activity-weight
  Scenario: Change the weight of an existing activity
    Given I am on the activity config page
    And the activity list contains "Play Chess" with weight 3
    When I change the weight of "Play Chess" to 5
    Then I should see "Play Chess" in the activity list with weight 5

  @delete-activity
  Scenario: Delete an activity from the config page
    Given I am on the activity config page
    And the activity list contains "Play Chess"
    When I delete "Play Chess"
    Then I should not see "Play Chess" in the activity list

  @activity-empty-state
  Scenario: Display page shows placeholder when no pick has been made
    Given there are no activity picks recorded
    When I navigate to the activity display page
    Then I should see a placeholder message prompting me to configure activities

  @activity-display-current-pick
  Scenario: Display page shows the current hourly pick
    Given an activity pick exists for the current hour with name "Play Chess"
    When I navigate to the activity display page
    Then I should see "Play Chess" displayed as the current pick
```

> **Note:** The last two scenarios require database seeding (inserting/clearing `ActivityPick` rows). A small test-helper API endpoint or direct DB access in fixtures will be needed at implementation time.

## Proposed Solution

Two new frontend routes are added under `/activity/`:

- **`/activity/`** — Display page. Shows a hero card with the current pick (activity name + time picked) and a 7×24 calendar week grid showing the last 7 days of picks. Polls the API every 60 seconds.
- **`/activity/config`** — Management page. Table of activities with inline weight editing (segmented 1–5 button row) and a delete icon per row. An add-new row at the bottom allows creating activities.

A Hangfire recurring job fires at the top of every hour (`0 * * * *`), randomly selects an activity weighted by the `Weight` field, and writes a snapshot record to the `ActivityPick` table. If the activity list is empty, the job skips silently.

The Hangfire dashboard is mounted at `/admin/hangfire` for operational visibility.

## Technical Approach

### Data Model

**`Activity`** — managed through the existing `ICRUD` infrastructure:
| Field | Type | Notes |
|-------|------|-------|
| `Id` | `Guid` | PK |
| `Name` | `string` | Display name (e.g. "Play Final Fantasy VII") |
| `Weight` | `int` | 1–5; defaults to 1 |

**`ActivityPick`** — standalone history log, no foreign key to `Activity`:
| Field | Type | Notes |
|-------|------|-------|
| `Id` | `Guid` | PK |
| `PickedAt` | `DateTime` (UTC) | When the pick was made |
| `ActivityName` | `string` | Snapshot of the activity name at pick time |

The `ActivityPick` table is intentionally decoupled from `Activity`. Because activities are expected to churn frequently (e.g. specific games replaced when finished), a foreign key would either cascade-delete history or require soft deletes accumulating indefinitely. Storing the name as a plain string preserves history cleanly through any activity lifecycle changes.

### Background Job

Hangfire is introduced as the background job framework. Storage is backend-dependent:
- **Production (EF/PostgreSQL):** `Hangfire.PostgreSql`
- **Dev / CI (InMemory):** `Hangfire.InMemory`

Storage selection keys off the existing `CrudFactory:Implementation` config value, consistent with the app's pluggable persistence pattern. The recurring job is registered on startup with `RecurringJob.AddOrUpdate` using the cron expression `0 * * * *`. No immediate pick is made on startup.

### Frontend

- Picks are stored in UTC and displayed in the browser's local time zone.
- The display page polls `/api/activity/current` every 60 seconds for the current pick.
- The week view renders a 7-column × 24-row grid. Cells show truncated activity names; full names appear on hover (tooltip). Future hour cells are grayed out; the current hour cell is highlighted.
- Weight selection on the config page uses a segmented button row (styled like pagination controls), with the selected weight highlighted. No edit mode required — clicking a button saves immediately.
- Empty state on the display page shows a placeholder like "Add some activities to get started" and links to `/activity/config`.
- `/activity/config` is not in the sidebar; it is accessible only via a settings icon on the `/activity/` display page.

### Key Decisions

| Decision | Chosen Approach | Rationale |
|----------|----------------|-----------|
| Background job framework | Hangfire | First of multiple planned background jobs; Hangfire's persistence, dashboard, and retry logic justify the dependency over `BackgroundService` |
| Hangfire storage | PostgreSQL (prod) / InMemory (dev/CI) | Mirrors existing pluggable persistence pattern; no new config concepts needed |
| Activity persistence | ICRUD infrastructure (EF table) | Activities are user-managed data, not config; consistent with existing data entities |
| Pick history FK | None — denormalized name string | Activities churn frequently; decoupling preserves history without soft-delete accumulation |
| Activity deletion | Hard delete only | No disable toggle needed; delete-and-re-add is sufficient given expected workflow |
| Real-time updates | 60-second client polling | Picks change once per hour; polling is negligible overhead and adds no infrastructure |
| Pick timing | Top of hour via cron `0 * * * *` | Predictable clock boundaries; no immediate pick on startup (list likely empty) |
| History display | 7-day × 24-hour calendar grid | Matches "Google Calendar week view" intent; full history at a glance |
| Activity weights | Integer 1–5 | Enough granularity to express preference without overwhelming the UI |

### Dependencies

- **Hangfire** (`Hangfire.AspNetCore`, `Hangfire.PostgreSql`, `Hangfire.InMemory`) — new NuGet dependencies
- **EF migrations** — new `Activities` and `ActivityPicks` tables
- **Existing ICRUD infrastructure** — `Activity` entity plugs into the existing factory pattern
- **`/admin/hangfire` route** — Hangfire dashboard mounted in the ASP.NET Core pipeline

## Open Questions

_None — all questions resolved during planning._

> **Hangfire dashboard auth note:** No auth guard is required for the initial implementation (home network only). Adding one later is low effort — implement `IDashboardAuthorizationFilter` and register it in the `DashboardOptions`. This must be done before any internet exposure.

## Out of Scope

- Configurable pick frequency (always hourly)
- Activity enable/disable toggle (delete and re-add instead)
- Pick notifications or alerts
- History beyond 7 days (no pagination or longer-range view)
- Multi-user support

## Success Metrics

- Hourly pick job runs reliably at the top of every hour with no missed firings
- Display page reflects the current pick within 60 seconds of it being made
- Activity CRUD operations are reflected immediately on the config page

## Timeline / Milestones

_TBD_
