# PRD: Dashboard Widgets

## Status

`Draft`

## Overview

Replace the seven placeholder widget stubs in the 3×2 dashboard grid with real, data-driven implementations. Five widgets (Weather, Tasks, Activity Picker, Audiobook, Bookmarks) get meaningful content; two (ComfyUI, RetroAchievements) are removed from the registry until their backing features have enough data to surface. Each widget fetches its own data, polls at a cadence appropriate to how often that data changes, and handles loading and error states consistently.

## Problem Statement

The dashboard grid is fully wired — users can pin, remove, and reorder slots — but every widget body is a placeholder showing only the feature name. The dashboard provides no actual value day-to-day. Users who visit the home page see an empty shell and have to navigate to individual feature pages to get any information.

## Goals

- Each of the five implemented widgets shows real, useful data at a glance without requiring the user to navigate to the feature page.
- All widgets share a consistent loading (spinner) and error (muted dash + "Unavailable") state.
- The Tasks widget supports marking a task done directly from the dashboard, with an undo toast.
- Widgets stay reasonably fresh via per-widget polling rather than stale single-fetch-on-mount.
- Adding a new widget for a future feature requires only: a new `.svelte` file + a `WIDGET_REGISTRY` entry.
- The general-purpose `ToastService` gains action-callback support and becomes the single toast system; the dashboard's bespoke undo toast is migrated onto it.

## Non-Goals

- Full feature UIs inside the widget (no add/edit/delete flows — those belong on the feature page).
- Real-time streaming or WebSocket updates — polling is sufficient.
- ComfyUI and RetroAchievements widgets — deferred until those features have query APIs worth surfacing.
- Per-widget refresh button or manual re-fetch trigger.
- Drag-to-reorder within a session (already handled by the dashboard skeleton PRD).

## User Stories / Use Cases

- **As a** HomeHub user, **I want to** see today's due chores on the dashboard, **so that** I know what needs doing without opening the Tasks page.
- **As a** HomeHub user, **I want to** mark a chore done from the dashboard widget, **so that** I don't have to navigate away for a quick tick-off.
- **As a** HomeHub user, **I want to** see today's suggested activity on the dashboard, **so that** I always have something to do when I glance at the home screen.
- **As a** HomeHub user, **I want to** see current weather conditions at a glance, **so that** I can decide how to dress without opening a separate app.
- **As a** HomeHub user, **I want to** see the status of my audiobook conversion, **so that** I know whether the job is still running.
- **As a** HomeHub user, **I want to** see my starred bookmarks on the dashboard, **so that** I can open frequently-used links in one click.

## E2E Scenarios

```gherkin
@dashboard-widgets
Feature: Dashboard Widgets

  @weather-widget-shows-conditions
  Scenario: Weather widget displays current conditions
    Given the weather widget is placed in slot 0
    And I am on the dashboard
    Then I should see the temperature in the weather widget
    And I should see the condition label in the weather widget
    And I should see the humidity in the weather widget
    And I should see the wind speed in the weather widget

  @tasks-widget-shows-due-tasks
  Scenario: Tasks widget shows tasks due today
    Given there are due tasks in the system
    And the tasks widget is placed in slot 0
    And I am on the dashboard
    Then I should see the due task names in the tasks widget

  @tasks-widget-mark-done
  Scenario: Marking a task done from the tasks widget shows an undo toast
    Given there is a due task named "Take out trash"
    And the tasks widget is placed in slot 0
    And I am on the dashboard
    When I mark "Take out trash" as done in the tasks widget
    Then I should see an undo toast

  @tasks-widget-overflow
  Scenario: Tasks widget shows overflow count when more than 5 tasks are due
    Given there are 7 due tasks in the system
    And the tasks widget is placed in slot 0
    And I am on the dashboard
    Then I should see 5 task rows in the tasks widget
    And I should see "and 2 more" in the tasks widget

  @activity-widget-shows-current-pick
  Scenario: Activity widget shows the current activity pick
    Given there is a current activity pick
    And the activity widget is placed in slot 0
    And I am on the dashboard
    Then I should see the activity name in the activity widget

  @audiobook-widget-active-job
  Scenario: Audiobook widget shows an active conversion job
    Given there is an in-progress audiobook job for "my-book.epub"
    And the audiobook widget is placed in slot 0
    And I am on the dashboard
    Then I should see "my-book.epub" in the audiobook widget
    And I should see the "in_progress" status badge in the audiobook widget

  @audiobook-widget-no-jobs
  Scenario: Audiobook widget shows empty state when there are no jobs
    Given there are no audiobook jobs
    And the audiobook widget is placed in slot 0
    And I am on the dashboard
    Then I should see "No conversions yet" in the audiobook widget

  @bookmarks-widget-starred
  Scenario: Bookmarks widget shows starred bookmarks as clickable links
    Given there is a starred bookmark named "GitHub"
    And the bookmarks widget is placed in slot 0
    And I am on the dashboard
    Then I should see "GitHub" as a link in the bookmarks widget

  @bookmarks-widget-fallback
  Scenario: Bookmarks widget falls back to recent bookmarks when none are starred
    Given there are unstarred bookmarks in the system
    And the bookmarks widget is placed in slot 0
    And I am on the dashboard
    Then I should see bookmark links in the bookmarks widget

  @widget-error-state
  Scenario: Weather widget shows an error indicator when its API fails
    Given the weather API is unavailable
    And the weather widget is placed in slot 0
    And I am on the dashboard
    Then I should see the error state in the weather widget
```

## Proposed Solution

Implement real Svelte component bodies for the five active widgets. Each widget is a self-contained `.svelte` file that fetches its own data in `onMount`, sets up a `setInterval` poll at its designated cadence, and renders one of three states: loading, error, or data. No props are passed from the dashboard page — the widget is fully responsible for its own data lifecycle.

Remove ComfyUI and RetroAchievements from `WIDGET_REGISTRY` until those features have query APIs worth surfacing.

Extend `ToastService` with an optional `action: { label: string; onClick: () => void }` field on `ToastConfig`, wire up `ToastWrapper` to render it, and migrate the dashboard page's bespoke widget-removal undo toast to use it. Extract task-complete/undo logic into a shared `TaskCompletionService` (or composable) so both `TasksWidget` and the full Tasks page share the same API call pattern without duplication.

## Design

**Handoff:** https://api.anthropic.com/v1/design/h/AtzNT0hwaDz9B5lJWPHL4w?open_file=Widget+Bodies.html

Key visual decisions from the design session:

- **Card padding:** 24px all around (the design system's `--space-5` token was resolving to 0 in the prototype; 24px is the correct target).
- **Weather:** Temperature + condition label are centered horizontally and vertically in the upper area of the card. Humidity and wind speed live in a two-column stat row (`<dl>`) pinned to the bottom of the body, separated by a hairline.
- **Tasks:** Compact rows with tight vertical padding (≈5px). Each row is `[task name] [DONE pill-button]` — no checkboxes, no per-row borders. Marking done strikes through the name and tints the row green. Overflow renders as `and N more…` in muted text with no divider. No 8-task variant; the max visible is 5.
- **Activity Picker:** Activity name displayed large and bold, vertically centered. Muted monospace micro-text `picked <date>` sits beneath it.
- **Audiobook:** Monospaced filename, status badge with `white-space: nowrap` (prevents badge text wrapping). No progress bar (the `AudiobookJob` API has no `progress` field). Empty state: muted italic "No conversions yet."
- **Bookmarks:** Vertical list — each row is `★ name ›`. Names carry a subtle hairline underline. On hover: star turns warning-yellow, underline turns brand-blue, chevron nudges right. Empty state shown when no bookmarks exist.
- **Shared states:** Spinner + italic "Loading…" for in-flight requests; `—` dash + italic "Unavailable" for errors.

## Technical Approach

### Widget data contracts

| Widget | API call | Data shown |
|--------|----------|------------|
| Weather | `WeatherApi.getCurrent()` | `temperature`, `conditionLabel`, `humidityPercent`, `windSpeed`, `units` |
| Tasks | `TasksApi.listTasks()` | Tasks where `doDate <= today` or `doDate` is null, sorted by name, max 5 visible |
| Activity | `ActivityPickApi.getCurrent()` | `activityName`, `pickedAt` |
| Audiobook | `AudiobookApi.listJobs()` | Most recent `queued`/`in_progress` job; fallback to most recent `completed`; else empty state |
| Bookmarks | `BookmarksApi.listBookmarks()` | Starred bookmarks up to 5; fallback to 5 most-recently-added if none starred |

### Polling cadences

| Widget | Interval | Rationale |
|--------|----------|-----------|
| Weather | 1 hour | Conditions are day-granularity |
| Tasks | 1 hour | Due dates are day-granularity |
| Bookmarks | 1 hour | Rarely changes |
| Activity Picker | 5 minutes | Pick may be re-rolled on the full page |
| Audiobook | 5 minutes | Job status progresses over minutes |

Polling is implemented with `setInterval` in `onMount`, cleared in the returned cleanup function.

### Toast system extension

`ToastConfig` gains an optional `action?: { label: string; onClick: () => void }` field. `ToastWrapper` renders an action button when present. The dashboard page's existing bespoke `undoToast` state + custom `.toast` CSS is removed; widget-removal undo is re-expressed through `ToastService.AddToast({ message: '...', action: { label: 'Undo', onClick: ... } })`.

### Task completion extraction

A `TaskCompletionService` living at `src/lib/services/Tasks/TaskCompletionService.ts` encapsulates: calling `TasksApi.completeTask()`, updating local task state, firing the undo toast via `ToastService`, handling the 5-second undo window, and calling `TasksApi.undoCompletion()`. Both `TasksWidget` and `tasks/+page.svelte` import and use this service.

### Key Decisions

| Decision | Chosen Approach | Rationale |
|----------|----------------|-----------|
| Data fetching | Each widget fetches its own data | No props from parent; widgets are self-contained |
| Error/loading state | Inline spinner; muted `—` + "Unavailable" on error; no retry | Consistent, low-complexity; full page is one click away |
| Polling | `setInterval` per widget, cleaned up on destroy | Simplest approach; no shared scheduler needed |
| Task undo in widget | Fire global undo toast via `ToastService` | Toast is screen-level; no need for in-widget undo UI |
| ComfyUI / Retro | Remove from registry | No query API to surface; cluttering picker with dead entries |
| Toast system | Extend `ToastService` with optional action callback | Eliminates duplicate toast implementation; one system for all toasts |

### Dependencies

- Existing API clients: `WeatherApi`, `TasksApi`, `ActivityPickApi`, `AudiobookApi`, `BookmarksApi`
- `ToastService` + `ToastWrapper` (to be extended)
- `WIDGET_REGISTRY` in `widgetRegistry.ts` (ComfyUI + Retro entries removed)

## Open Questions

_None — all questions resolved during PRD review._

## Out of Scope

- ComfyUI widget (no status API exists)
- RetroAchievements widget (feature not yet built)
- Drag-to-reorder widget positions within a session
- Widget-level "Refresh" button
- Any write operations other than task completion (no add/edit/delete from the dashboard)

## Success Metrics

- All five widget bodies display real data when the relevant feature flag is enabled.
- No widget throws an unhandled exception when its API endpoint is down.
- The Tasks "Done" button fires a toast with a working "Undo" within the 5-second window.
- E2E suite passes for all ten scenarios above.

## Timeline / Milestones

_TBD_
