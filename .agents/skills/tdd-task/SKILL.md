---
name: tdd-task
description: Implements a PRD's breakdown tasks one at a time using TDD. Reads the PRD's child tasks from Kanboard, picks the next one, implements it with the red-green-refactor loop, runs precommit checks, then auto-commits and continues without human input. Use when the user wants to implement a PRD, work through a task list, or start coding from a breakdown.
---

# TDD Task

Implement a PRD's breakdown tasks one at a time using TDD. Each iteration: pick the
next child task → implement with red-green-refactor → run precommit → auto-commit →
mark it Done → repeat. No human input is required between tasks.

The PRD is a task in the **Features swimlane**; its breakdown items are **child
tasks** (`{parentId}-{NN} name`, linked "is a child of" the PRD). All Kanboard
access goes through `../../kanboard/kb.py`. Column = status: **Ready to
implement** (not started), **In progress**, **Ready for review** (implemented,
awaiting the `review-ready` gate), **Done** (approved).

Load the `tdd` skill now and keep its principles active for the full session.

---

## Workflow

### Step 1 — Locate the PRD and its tasks

```bash
.agents/kanboard/kb.py prd list
```

- **User named a PRD**: use that task id.
- **A PRD is in In progress**: an interrupted run — resume it.
- **Otherwise the highest-priority Ready to implement PRD that has children** (`children > 0`): use it.
- **No PRD has children**: stop. Tell the user to run `breakdown-prd` first (or `write-prd` then `breakdown-prd` if only stubs exist in Backlog).

Then read the child list:

```bash
.agents/kanboard/kb.py task list <parent-id>
```

Rows come back ordered by `seq`, each with `id`, `column`, `done`, `title`.

---

### Step 2 — Select the next child task

Priority order:

1. **Any child in In progress** — an interrupted session; resume it.
2. **Lowest-`seq` child not yet Done or in review** (i.e. in Ready to implement) — the next task.
3. **All children Done or in Ready for review** — go to **Step 6**.

Before starting the first task of a run, put the PRD itself into In progress
(no-op if already there):

```bash
.agents/kanboard/kb.py feature start <parent-id>
```

Announce:

```
Starting task [title]  (#[child-id])
[one-line description from the task body]
```

Move the child to In progress and read its full body:

```bash
.agents/kanboard/kb.py task start <child-id>
.agents/kanboard/kb.py task show <child-id>
```

---

### Step 3 — Implement using TDD

Follow the `tdd` skill's full workflow:

1. **Plan** — read the task's Description, Acceptance Criteria, and Notes (from `task show`). Identify the behaviors to test and the public interfaces. If anything is ambiguous in a blocking way, ask the user before writing code.
2. **Tracer bullet** — one test for the first behavior → minimal code to pass.
3. **Incremental loop** — for each remaining acceptance criterion: write test → make it pass.
4. **Refactor** — once green, clean up; run tests after each step.

Stay within this task's scope. Do not implement anything belonging to a later
task, even if convenient. Some tasks are Godot-side and not unit-testable — say so
plainly and rely on a green build plus a stated manual check rather than a
fabricated test.

---

### Step 4 — Run precommit

```bash
task precommit
```

- **Pass** → Step 5.
- **Fail** → fix autonomously and re-run, up to **3 attempts**. If still failing, stop, show the user the failures, and ask how to proceed. Leave the child in In progress — do not park it for review.

---

### Step 5 — Commit, park for review, and continue

1. Park the child for the review gate:

   ```bash
   .agents/kanboard/kb.py task review <child-id>
   ```

2. Commit all changes:

   ```
   {short summary of changes made}

   {optional longer description — only when the change is non-obvious}

   Addresses {child-title}          # e.g. "Addresses 11-03 Implement listing endpoint"
   {child url}
   ```

   End with the project's required commit trailer if it has one.
3. Return to **Step 2** immediately and pick the next child. Do not pause for human input.

---

### Step 6 — Finish the PRD

Triggered when Step 2 finds all children Done or in Ready for review.

1. Park the PRD for its whole-feature review:

   ```bash
   .agents/kanboard/kb.py feature review <parent-id>
   ```

2. Print a short summary of the tasks completed this run (with their `#id`s),
   flag any that left a manual verification step (Godot-side visual changes),
   and tell the user to run `review-ready` (or wait for the hourly pipeline) to
   approve the parked work to Done.

---

## Rules

- **One task at a time.** Never begin the next child until the current one is committed and moved to Ready for review.
- **Stay in scope.** Do not implement behaviors belonging to future tasks.
- **Never skip precommit.** Every task must pass `task precommit` before being committed.
- **No human input between tasks.** Commit and continue automatically until every child is Done or parked in Ready for review, or a failure needs human guidance.
- **Resume gracefully.** A child (or the PRD) already in In progress on startup is an interrupted session — pick up the TDD loop from context.
- **Be honest about verification.** Don't fabricate tests for inherently visual/Godot-side tasks; use build + a stated manual check.
