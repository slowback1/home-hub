# PRD: Walk Session History

## Status

`Draft`

## Overview

Walk sessions are already syncing from the Android app to the HomeHub backend, but there is no way to view that history in the frontend. This feature adds a dedicated list page at `/walk-history` and a dashboard widget, giving the user full visibility into their past walk activity — date, step count, and duration — without leaving the HomeHub interface.

## Problem Statement

Walk data is flowing in from the Android app but is invisible to the user in HomeHub. There is no way to review past sessions, spot trends, or confirm that syncs are landing correctly. The backend also lacks a read endpoint, so no frontend can access the data today.

## Goals

- Add a `GET /api/walk-sessions` backend endpoint that returns all sessions sorted by `StartedAt` descending.
- Build a `/walk-history` list page showing all sessions with relative date, step count, and formatted duration.
- Build a dashboard widget showing the 3 most recent sessions in the same format.
- Gate the entire feature behind a `WALK_SESSION_HISTORY_ENABLED` feature flag.
- Follow all existing HomeHub conventions: sidebar nav entry, widget registry, API client pattern.

## Non-Goals

- Filtering or searching sessions by date range or device.
- Grouping or per-device breakdowns (single-device household; flat list is sufficient).
- Pagination (fetch all sessions; add if scale demands it later).
- Editing or deleting sessions from the frontend.
- Displaying the `SyncedAt` or `ClientId` fields to the user.
https://api.anthropic.com/v1/design/h/kNbDOVd14txseG7C5_d_cA?open_file=Walk+History.html
## User Stories / Use Cases

- **As a** HomeHub user, **I want to** see a list of all my past walk sessions, **so that** I can review my activity history at a glance.
- **As a** HomeHub user, **I want to** see my 3 most recent walks on the dashboard, **so that** I don't have to navigate away to get a quick activity summary.
- **As a** HomeHub user, **I want to** see a clear empty state when no sessions have been synced yet, **so that** I understand what to do next (open the app on my phone).

## E2E Scenarios

```gherkin
@walk-history
Feature: Walk Session History

  @walk-history-widget-recent-sessions
  Scenario: Dashboard widget shows 3 most recent walk sessions
    Given the WALK_SESSION_HISTORY_ENABLED feature flag is enabled
    And the walk session history widget is on the dashboard
    And there are walk sessions synced from the Android app
    When I view the dashboard
    Then I see up to 3 sessions in the widget
    And each row shows the session date, step count, and duration formatted as "1h 2m"

  @walk-history-widget-empty
  Scenario: Dashboard widget shows empty state when no sessions exist
    Given the WALK_SESSION_HISTORY_ENABLED feature flag is enabled
    And the walk session history widget is on the dashboard
    And no walk sessions have been synced
    When I view the dashboard
    Then the widget shows an empty state message

  @walk-history-list-page
  Scenario: List page shows all walk sessions sorted by date descending
    Given the WALK_SESSION_HISTORY_ENABLED feature flag is enabled
    When I navigate to "/walk-history"
    Then I see all synced walk sessions
    And they are sorted with the most recent session first
    And each row shows date, step count, and duration

  @walk-history-list-empty
  Scenario: List page shows empty state when no sessions exist
    Given the WALK_SESSION_HISTORY_ENABLED feature flag is enabled
    And no walk sessions have been synced
    When I navigate to "/walk-history"
    Then I see an empty state message indicating no walks have been recorded

  @walk-history-feature-flag-hidden
  Scenario: Walk History is hidden when feature flag is disabled
    Given the WALK_SESSION_HISTORY_ENABLED feature flag is disabled
    When I view the sidebar
    Then the "Walk History" nav item is not visible
    And the walk history widget does not appear in the dashboard widget picker
```

## Proposed Solution

**Backend:** Add a `GET /api/walk-sessions` action to `WalkSessionController` that returns all `WalkSession` records via the existing `ICrud<WalkSession>` interface, sorted by `StartedAt` descending.

**Frontend:** Follow the established feature pattern:
1. `WalkSessionApi.ts` — typed API client with a `listSessions()` method.
2. `WalkHistoryWidget.svelte` — dashboard widget showing 3 most recent sessions (rows variant with loading + empty states).
3. `src/routes/walk-history/+page.svelte` — full list page with ledger table layout.
4. Register the widget in `widgetRegistry.ts` and add the nav item to `Sidebar.svelte`, both gated by `WALK_SESSION_HISTORY_ENABLED`.

## Design

**Handoff:** [https://api.anthropic.com/v1/design/h/kNbDOVd14txseG7C5_d_cA?open_file=Walk+History.html](https://api.anthropic.com/v1/design/h/kNbDOVd14txseG7C5_d_cA?open_file=Walk+History.html)

Key visual decisions from the design session:

- **Dates shown as relative time** ("Today", "Yesterday", "8 days ago") rather than absolute dates, with a secondary line showing weekday + time (e.g. "Thu · 7:42 AM").
- **List page uses the Ledger table layout** (Variation A) — dense column-aligned table with uppercase "When / Steps / Duration" headers, row hover highlight, steps bold and duration in secondary color.
- **Page header** has a brand-colored square icon tile containing the Footprints icon, an h1 "Walk History", and a subtitle showing session count. The "Synced N ago" label from the design is omitted for now.
- **Dashboard widget uses the Rows variant** — each of the 3 rows shows relative time (primary) + weekday/time (secondary) on the left, and stacked step count + duration figures (right-aligned, with "steps" / "time" labels) on the right. Widget header has "View all" link; footer has "View all walks →".
- **Widget loading state**: centered spinner + "Loading…". **Widget empty state**: Footprints icon + "No walks yet — start one on your phone to see it here."
- **Page empty state**: centered Footprints icon in a circle, "No walks recorded yet" heading, instructional copy, and a pill hint "Open SlowWalk on your phone to begin."
- Duration formatted as `1h 2m` (hours + minutes, drop seconds); steps use thousands separators.

## Technical Approach

### Key Decisions

| Decision | Chosen Approach | Rationale |
|----------|----------------|-----------|
| Backend read endpoint | `GetAllAsync` via existing `ICrud<WalkSession>`, sorted in-controller | Consistent with other controllers; no custom query needed |
| Sort order | `StartedAt` descending | Most recent sessions first, both on page and widget |
| Duration formatting | `Xh Ym` / `Ym` helper | Clean, human-readable; matches design |
| Date display | Relative time ("Today" / "Yesterday" / "N days ago") + weekday/time sub-label | User decided during design session |
| Feature flag | `WALK_SESSION_HISTORY_ENABLED` | Matches existing flag naming convention |
| Widget icon | `Footprints` from lucide-svelte | Confirmed in design |
| Nav label | "Walk History" | Confirmed in interview |
| Route | `/walk-history` | Confirmed in interview |

### Dependencies

- `ICrud<WalkSession>` — already wired up in `AppDbContext` and `WalkSessionController`.
- `Footprints` icon — available in lucide-svelte.
- Feature flag infrastructure — already in place (`FeatureFlagService`, `FeatureFlags.ts`, `widgetRegistry`, `Sidebar`).

## Open Questions

_None — all questions resolved before implementation._

## Out of Scope

- Filtering, searching, or paginating sessions.
- Per-device grouping or multi-device support.
- Deleting or editing sessions from the UI.
- Push notifications or real-time sync indicators.

## Success Metrics

- Walk sessions synced from the Android app are visible on the `/walk-history` page.
- The dashboard widget displays the 3 most recent sessions without errors.
- Feature is invisible when `WALK_SESSION_HISTORY_ENABLED` is disabled.

## Timeline / Milestones

_TBD_
