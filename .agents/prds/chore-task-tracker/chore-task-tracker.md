# PRD: Chore / Task Tracker

## Status

`Draft`

## Overview

A personal task tracker built into HomeHub that handles both one-off tasks (e.g. "plan vacation") and recurring tasks (e.g. "sweep floors every 7 days"). Tasks surface on a to-do list only when they are due, keeping the view focused on what needs attention now. Recurring tasks automatically advance their due date on completion, with full completion history preserved. This replaces a previous standalone version with a more capable, polished implementation backed by the shared HomeHub database and UI design system.

## Problem Statement

The previous task tracker lacked polish and functionality. Rewriting it into HomeHub gives it access to shared infrastructure (persistent DB, design system, auth) and allows recurrence to be handled properly. Without a working tracker, chores and one-off to-dos must be managed in external tools that don't live alongside the rest of the household dashboard.

## Goals

- One-off tasks can be created, completed, and soft-deleted (hidden from view, history preserved)
- Recurring tasks can be defined with a simple interval (every N days) and automatically advance their due date on completion
- A "do date" model controls task visibility: tasks appear only when today ≥ DoDate (or when no DoDate is set)
- All completions (one-off and recurring) are logged for future history use
- A clear UI separates due tasks from upcoming tasks on a single page
- Recurrence logic lives behind an interface to allow future rule formats without a schema change

## Non-Goals

- Push notifications, email reminders, or browser alerts (v1)
- Priority levels, tags, or categories (v1)
- Per-user task ownership — tasks are global to the HomeHub instance
- A history/archive view for completed tasks (data is preserved, UI deferred)
- Complex recurrence rules (e.g. "every Monday and Thursday", iCal RRULE)

## User Stories / Use Cases

- **As a** HomeHub user, **I want to** see only the tasks that are due today or overdue, **so that** I focus on what needs doing now without future tasks cluttering the view.
- **As a** HomeHub user, **I want to** create a recurring chore with an interval, **so that** it automatically reappears after I complete it without me having to recreate it.
- **As a** HomeHub user, **I want to** mark a task done and undo that action within a few seconds, **so that** accidental completions don't silently shift my recurring tasks forward.
- **As a** HomeHub user, **I want to** see upcoming tasks below the due list, **so that** I have visibility into what's coming without it being in my face.
- **As a** HomeHub user, **I want to** add and edit tasks in a modal, **so that** I can manage my task list without navigating away from the page.

## E2E Scenarios

```gherkin
@tasks
Feature: Chore / Task Tracker

  @tasks-due-list-visibility
  Scenario: Due list shows only tasks whose DoDate is today or earlier
    Given a task "Sweep floors" with DoDate 5 days ago
    And a task "Rake leaves" with DoDate 5 days from now
    When I visit the Tasks page
    Then I see "Sweep floors" in the Due section
    And I do not see "Rake leaves" in the Due section
    And I see "Rake leaves" in the Upcoming section

  @tasks-no-dodate-always-visible
  Scenario: Tasks with no DoDate always appear in the Due section
    Given a task "Buy more soap" with no DoDate
    When I visit the Tasks page
    Then I see "Buy more soap" in the Due section

  @tasks-complete-one-off
  Scenario: Completing a one-off task removes it from the Due list
    Given a task "Plan vacation" with no DoDate and no recurrence
    When I click "Done" on "Plan vacation"
    Then "Plan vacation" disappears from the Due section
    And an undo toast appears

  @tasks-undo-completion
  Scenario: Undoing a completion restores the task to the Due list
    Given a task "Plan vacation" with no DoDate and no recurrence
    When I click "Done" on "Plan vacation"
    And I click "Undo" on the toast before it expires
    Then "Plan vacation" reappears in the Due section

  @tasks-complete-recurring
  Scenario: Completing a recurring task bumps its DoDate and moves it to Upcoming
    Given a recurring task "Sweep floors" with DoDate today and interval 7 days
    When I click "Done" on "Sweep floors"
    Then "Sweep floors" disappears from the Due section
    And "Sweep floors" appears in the Upcoming section with DoDate 7 days from now

  @tasks-create-one-off
  Scenario: Creating a one-off task with a future DoDate places it in Upcoming
    When I open the Add Task modal
    And I enter name "Call dentist" and DoDate 3 days from now
    And I submit the form
    Then "Call dentist" appears in the Upcoming section
    And "Call dentist" is not visible in the Due section

  @tasks-create-recurring
  Scenario: Creating a recurring task with no DoDate places it immediately in Due
    When I open the Add Task modal
    And I enter name "Take out trash", toggle recurring on, and set interval to 7 days
    And I submit the form
    Then "Take out trash" appears in the Due section

  @tasks-edit-task
  Scenario: Editing a task's name updates it in the list
    Given a task "Sweep flors" in the Due section
    When I open the Edit modal for "Sweep flors"
    And I correct the name to "Sweep floors" and save
    Then I see "Sweep floors" in the Due section
    And I do not see "Sweep flors"

  @tasks-delete-task
  Scenario: Deleting a task from the Edit modal removes it permanently
    Given a task "Old task" in the Due section
    When I open the Edit modal for "Old task"
    And I click Delete and confirm
    Then "Old task" is no longer visible on the Tasks page
```

## Proposed Solution

Replace the "Coming soon" placeholder at `/tasks` with a fully functional task tracker page. The page has two sections:

1. **Due** — tasks where `DoDate IS NULL OR DoDate <= TODAY`, sorted past-due ascending then no-DoDate by creation date. Each row has a "Done" button that triggers a completion + undo toast.
2. **Upcoming** — tasks where `DoDate > TODAY`, sorted ascending by DoDate.

An "Add Task" button opens a modal form. Each task row has an edit icon that opens the same modal pre-populated (Edit mode adds a Delete button).

The backend exposes standard CRUD endpoints for tasks plus a dedicated `POST /api/tasks/{id}/complete` endpoint that handles the completion logic (write log entry, update DoDate or stamp CompletedAt) and a `DELETE /api/tasks/{id}/completions/latest` endpoint to support undo.

## Technical Approach

### Data Model

```
Task
  Id              string (GUID)
  Name            string
  IsRecurring     bool
  IntervalDays    int? (nullable; only set when IsRecurring = true)
  DoDate          DateOnly? (nullable; null = always visible in Due)
  CompletedAt     DateTime? (nullable; set on completion of one-off tasks)
  CreatedAt       DateTime

TaskCompletion
  Id              string (GUID)
  TaskId          string (FK → Task.Id)
  CompletedAt     DateTime
  PreviousDoDate  DateOnly? (nullable; stores pre-completion DoDate for undo)
```

### Recurrence Abstraction

Recurrence next-date calculation is encapsulated behind an `IRecurrenceCalculator` interface:

```csharp
public interface IRecurrenceCalculator
{
    DateOnly GetNextDoDate(Task task, DateTime completedAt);
}
```

A `IntervalDaysRecurrenceCalculator` implements this for the simple interval case. The interface is registered via DI, making the engine swappable for future rule formats (cron, iCal, etc.) without touching completion logic.

### Completion Flow

**Complete:**
1. Write a `TaskCompletion` record
2. If `IsRecurring`: update `Task.DoDate = IRecurrenceCalculator.GetNextDoDate(task, now)`
3. If not `IsRecurring`: set `Task.CompletedAt = now`

**Undo (within toast window):**
1. Delete the latest `TaskCompletion` record for the task
2. If `IsRecurring`: restore the previous `Task.DoDate` (stored on the completion record or recalculated)
3. If not `IsRecurring`: clear `Task.CompletedAt`

### API Endpoints

| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/tasks` | Returns all non-completed tasks (CompletedAt IS NULL) |
| GET | `/api/tasks/history` | Returns completed one-off tasks (future history view) |
| POST | `/api/tasks` | Create a task |
| PUT | `/api/tasks/{id}` | Update name, DoDate, IsRecurring, IntervalDays |
| DELETE | `/api/tasks/{id}` | Hard delete task and all completions |
| POST | `/api/tasks/{id}/complete` | Complete a task (write log + advance DoDate or stamp CompletedAt) |
| DELETE | `/api/tasks/{id}/completions/latest` | Undo the most recent completion |

### Frontend

- SvelteKit page at `/tasks` replacing the current stub
- Two reactive sections driven by a single `tasks` array fetched on mount, split client-side by DoDate
- Modal component for Add/Edit (shared form, Edit mode shows Delete button)
- Toast using existing `Toast` component from `$lib/ui/containers/toast` for undo

### Key Decisions

| Decision | Chosen Approach | Rationale |
|----------|----------------|-----------|
| Recurrence format | Simple interval (every N days) | Covers all real household chore cadences; complex rules deferred |
| Recurrence engine | `IRecurrenceCalculator` interface | Keeps engine swappable without schema or logic changes |
| Task instance model | Single row, bumped DoDate | Avoids template/instance complexity; completion log provides history |
| One-off completion | Soft delete (`CompletedAt`) + log entry | Data preserved for future history UI; clean from active view |
| Due date concept | "Do date" — visibility filter, not hard deadline | Tasks appear when you should act, not as overdue alarms |
| Delete | Hard delete from Edit modal only | Keeps row UI clean; modal provides natural confirmation step |
| Auth/ownership | Global (no UserId FK) | Consistent with rest of app; single-household use case |
| Undo mechanism | Toast (~5s) + `DELETE /completions/latest` | Low-friction recovery for accidental completions |

### Dependencies

- Existing `ICrudFactory` / `ICrud<T>` pattern for EF Core persistence
- `AppDbContext` — two new `DbSet`s added (`ChoreTask`, `TaskCompletion`)
- EF Core migration generated via `dotnet ef migrations add`
- Existing Toast component (`$lib/ui/containers/toast`)
- Existing modal/form UI components

## Open Questions

_None — all questions resolved during planning._

## Out of Scope

- Notifications and reminders
- Priority, tags, and categories
- Per-user task ownership
- History/archive view for completed tasks
- Complex recurrence rules (iCal RRULE, day-of-week patterns)

## Success Metrics

- One-off and recurring tasks can be created, completed, and managed without errors
- Completing a recurring task correctly advances DoDate by the configured interval
- Undo correctly restores task state within the toast window
- Due / Upcoming sections reflect correct visibility rules on page load

## Timeline / Milestones

_TBD_
