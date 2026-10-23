---
name: implement-ready
description: Unattended pipeline stage 3. Starts with a recovery pass that resumes cards left In progress by a crashed run. Works the "Ready to implement" column: auto-breaks down approved PRDs into child tasks (no approval step), implements children with TDD, and parks finished work in "Ready for review". Handles rejected children sent back for rework. Use when the pipeline wrapper runs implement, or when the user says "implement the ready work".
---

# Implement Ready

Stage 3: implement everything the human approved in **Ready to implement**
(Features swimlane). This is `breakdown-prd` + `tdd-task` fused, with no
approval step and no human input. Finished children land in **Ready for
review**, not Done — the review stage approves.

Load the `tdd` skill and keep its principles active for the full session.

## Unattended rules

- **Never ask questions.** Ambiguity that blocks implementation → comment on the
  card explaining the blocker and leave it in Ready to implement.
- **Commit locally only** — this repo has no git remote. Never push.
- Stop retrying a failing gate after **3 attempts**; comment the failure on the
  card and move on to the next item.

## Project gate

Run the repo's checks before committing. Detect in order:

1. `Taskfile.yml` with a `precommit` task → `task precommit`
2. `*.sln` present → `dotnet build` + `dotnet test`
3. `package.json` with a test script → `npm test`
4. pytest config or a `tests/` dir → `pytest`
5. None of the above (the repo has no code gate yet) → syntax-check every
   changed file (e.g. `python -m py_compile`, `node --check`) and note in the
   card comment that no test gate exists yet.

## Workflow

### Step 0 — Recovery pass (before the column survey)

A crashed run leaves its work in "In progress", invisible to the Step 1 survey.
Scan it first and finish it:

```bash
.agents/kanboard/kb.py stage list "In progress" --lane Features
```

- **In-progress child** (`is_breakdown_child` true): resolve its parent via the
  "is a child of" link (`kb.py link list <id>`). Run `kb.py comment list <id>`:
  a reviewer rejection comment means it was rejected and re-sent — handle it
  through Step 2. **No rejection comment means crashed.** Resume it:
  1. Discard uncommitted tracked edits (`git restore .`): uncommitted work is
     unattributable; committed work is the recovery unit.
  2. Find what the crashed run already committed:
     `git log --grep="{child-title}"` (the `Addresses` footer + child URL).
     Continue from those commits — do not duplicate or contradict them.
  3. Comment "resumed after interrupted run" on the child.
  4. Resume the TDD loop per Step 3, run the gate, commit with the same
     `Addresses {child-title}` footer, then `kb.py task review <id>`.
  5. Gate fails 3× → comment the failure, leave it In progress, move on.
- **In-progress parent PRD**: run the normal Step 3 sequence (resume its
  In-progress children, then its lowest-`seq` Ready-to-implement children);
  when all children are Done or in Ready for review, `kb.py feature review`.
- Comment "resumed after interrupted run" on every card recovery touches.

**Feature-branch-per-ticket — decision: not adopted.** Per-child commit
discipline (the `Addresses {child-title}` footer + child URL, greppable via
`git log --grep`) is the recovery unit and attribution mechanism; review-ready
already depends on it. Branches would add merge/resume complexity without
improving attribution. Recovery always resumes on main.

**Retry policy:** recovery retries an In-progress card on every run; the
gate-fail-3× park (comment + leave In progress) is the only bail-out.

### Step 1 — Survey the column

```bash
.agents/kanboard/kb.py stage list "Ready to implement" --lane Features
```

Rows carry `children` (count) and `is_breakdown_child` (bool). Three kinds:

- **Rejected children** (`is_breakdown_child` true **and carrying a reviewer
  rejection comment** — verify with `kb.py comment list <id>`) — rework first
  (Step 2). A child in Ready to implement with **no** rejection comment is not
  rejected: it is an unstarted child of an interrupted run, reached through its
  parent in Step 3 — never rework it as if a reviewer had knocked it back.
- **PRDs with children** (`children > 0`) — continue their breakdown (Step 3).
- **PRDs without children** (`children = 0`) — auto-breakdown (Step 4), then
  continue at Step 3.

Process in priority order (high → low), oldest first.

### Step 2 — Rework a rejected child

A child sitting in Ready to implement is a rejected child only if it carries a
reviewer rejection comment (review-ready guarantees every rejection posts one).
If `kb.py comment list <id>` shows no rejection comment, this is a
misclassification — stop and handle the card through its parent in Step 3.

1. `kb.py comment list <id>` — read the reviewer's rejection comment.
2. `kb.py task start <id>` (moves to In progress, assigns it to you).
3. Fix per the rejection notes, run the gate, commit with the same
   `Addresses {child-title}` footer, then `kb.py task review <id>`.

### Step 3 — Implement a PRD's children

1. `kb.py task list <parent-id>` — children ordered by `seq`.
2. If any child is In progress, resume it (interrupted run).
3. Before starting the first child of a run, move the PRD itself:
   `kb.py feature start <parent-id>` (→ In progress).
4. For the lowest-`seq` child in Ready to implement:
   - `kb.py task start <child-id>`
   - Implement with TDD per the child's Description / Acceptance Criteria
     (`kb.py task show <child-id>`), staying strictly in scope.
   - Run the gate. Fail 3× → comment on the child, leave it In progress, move on.
   - Commit:

     ```
     {short summary of changes made}

     Addresses {child-title}
     {child url}
     ```

   - `kb.py task review <child-id>` (→ Ready for review).
5. Repeat until no children remain in Ready to implement or In progress.
6. When **all children are Done or in Ready for review**, move the PRD:
   `kb.py feature review <parent-id>` (→ Ready for review).

Godot-side / non-unit-testable work: green build plus a stated manual check
(the child's `Manual check:` criterion) — never a fabricated test.

### Step 4 — Auto-breakdown a childless PRD

This is `breakdown-prd` **without the approval step**:

1. `kb.py prd show <parent-id>` — read the approved PRD.
2. Decompose per breakdown-prd's granularity/ordering rules: commit-sized
   units, tests bundled with their implementation, dependency order.
   Children whose criteria require running the game (Godot-side /
   non-unit-testable work) must state an explicit `Manual check:` acceptance
   criterion — concrete steps + expected observations — so the review stage
   can detect it and park the finished child in "Needs manual review" for a
   human playtest. Never fabricate a test for such work.
3. Create children in order, landing them in Ready to implement:

   ```bash
   .agents/kanboard/kb.py task add <parent-id> --name "task title" --desc-file .agents/pipeline/tmp/task.md
   ```

   Body from [../breakdown-prd/TASK_TEMPLATE.md](../breakdown-prd/TASK_TEMPLATE.md).
4. If any result shows `linked: false` or a `warning`, stop and comment on the
   PRD — do not leave orphaned children.
5. Continue at Step 3.

### Step 5 — Summary

Print: cards recovered from an interrupted run (ids), PRDs broken down,
children implemented (ids), children reworked, items blocked with comments, and
anything parked for the review stage.
