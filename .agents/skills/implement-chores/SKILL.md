---
name: implement-chores
description: Unattended pipeline stage 2 of the chore flow. Works Chores-swimlane cards in In progress (resuming crashed runs, reworking reviewer-rejected ones) and Ready to implement to completion with the TDD red-green-refactor loop, runs the project gate, commits on main with the Addresses footer, and parks finished chores in Ready for review for the review stage (#349) to approve. Never asks questions. Use when the pipeline wrapper runs implement-chores, or when the user says "implement the chores".
---

# Implement Chores

Stage 2 of the chore pipeline:

```
Backlog -> [triage-chores #347] -> In progress -> [implement-chores] ->
Ready for review -> [review-chores #349] -> Done
```

This is `chore-task`'s workflow fused with `implement-ready`'s unattended
rules: no human input, recovery pass first, finished chores land in **Ready
for review** (not Done — the review stage approves). All Kanboard access goes
through `../../kanboard/kb.py`.

Load the `tdd` skill now and keep its principles active for the full session.

## Unattended rules

- **Never ask questions.** Ambiguity that blocks implementation → comment on
  the card explaining the blocker and leave it In progress.
- **Commit locally; never push** — only the review stage pushes to origin, and
  only when its checks and gates pass.
- Stop retrying a failing gate after **3 attempts**; comment the failure on the
  card, leave it In progress, and move on to the next chore.

## Project gate

Run the repo's checks before committing. Detect in order:

1. `Taskfile.yml` with a `precommit` task → `task precommit`
2. `*.sln` present → `dotnet build` + `dotnet test` (this repo:
   `backend/HomeHubBackend.sln`)
3. `package.json` with a test script → `npm test`
4. pytest config or a `tests/` dir → `pytest`
5. None of the above → syntax-check every changed file and note in the card
   comment that no test gate exists yet.

## Workflow

### Step 0 — Recovery pass (before the survey)

A crashed run leaves chores in **In progress**, invisible to a
Ready-to-implement-only survey. Scan them first and finish them:

```bash
.agents/kanboard/kb.py stage list "In progress" --lane Chores
```

For each card, read its comments:

```bash
.agents/kanboard/kb.py comment list <id>
```

- **Reviewer rejection comment present** (posted by #349): the chore was
  rejected and re-sent — rework it per the rejection notes (it is already In
  progress; go straight to the Step 2 fix loop).
- **No rejection comment**: the card was left In progress by a crashed run.
  Resume it:
  1. Discard uncommitted tracked edits (`git restore .`): uncommitted work is
     unattributable; committed work is the recovery unit.
  2. Find what the crashed run already committed:
     `git log --grep="{chore-title}"` (the `Addresses` footer + chore URL).
     Continue from those commits — do not duplicate or contradict them.
  3. Comment "resumed after interrupted run" on the card.
  4. Resume the Step 2 fix loop from the current state, run the gate, commit
     with the same `Addresses {chore-title}` footer, then move the card to
     Ready for review.
  5. Gate fails 3× → comment the failure, leave it In progress, move on.

Comment "resumed after interrupted run" on every card the recovery pass
touches.

**Branching:** main branch only. The `Addresses {chore-title}` footer + chore
URL (greppable via `git log --grep`) is the recovery unit and attribution
mechanism — the same discipline the Features pipeline uses. Per-chore feature
branches are not adopted.

**Retry policy:** recovery retries an In-progress chore on every run; the
gate-fail-3× park (comment + leave In progress) is the only bail-out.

### Step 1 — Survey the chore lane

```bash
.agents/kanboard/kb.py stage list "In progress" --lane Chores
.agents/kanboard/kb.py stage list "Ready to implement" --lane Chores
```

Process In-progress cards first (recovery/rework), then Ready-to-implement
cards (log-chore lands chores there; triage moves planned ones to In
progress). Within each group: priority high → low, oldest first.

**Chores in Backlog or Review refinement are not workable** — they carry open
questions. Never touch them here.

If both lists are empty, print "no chores to implement" and stop.

### Step 2 — Work one chore

1. Move it to In progress (this also assigns it to you):

   ```bash
   .agents/kanboard/kb.py chore start <id>
   ```

   (Skip if the card is already In progress from the recovery pass.)

2. Read the full chore and its triage plan:

   ```bash
   .agents/kanboard/kb.py chore show <id>
   .agents/kanboard/kb.py comment list <id>
   ```

   Use the `Plan:` comment from triage-chores (#347) as the implementation
   brief, alongside the chore's Details / Proposed Fix.

3. Fix using the `tdd` red-green-refactor loop:
   - **Reproduce first.** For a bug, write a failing test that captures the
     wrong behavior (RED), then make it pass (GREEN). For a small change,
     write a test for the new behavior, then implement it. Refactor only at
     GREEN.
   - Stay strictly within this one chore's scope — a chore is a single small
     change. If it turns out to be feature-sized, comment on the card
     suggesting `write-prd` and leave it In progress.
   - If a blocking detail is ambiguous, do **not** ask: comment on the card
     explaining the ambiguity and leave it In progress. Move on.

   **Not everything is unit-testable** (UI rendering, visual tweaks, asset
   changes). When a red-green test genuinely isn't possible, say so
   plainly, make the change, and rely on a successful build plus a
   clearly-stated manual verification step written into the card comment. Do
   **not** fabricate a test that doesn't really exercise the behavior.

4. Run the project gate.
   - **Pass** → step 5.
   - **Fail** → fix autonomously and re-run, up to **3 attempts**. If still
     failing, comment the failure output on the card, leave it In progress,
     and move on to the next chore.

5. Commit all changes on main:

   ```
   {short summary of the fix}

   Addresses {chore-title}
   {kanboard url}
   ```

6. Move the finished chore to Ready for review (not Done — #349 approves):

   ```bash
   .agents/kanboard/kb.py move <id> "Ready for review"
   ```

   For a chore that wasn't unit-testable, the card comment must state the
   manual check steps so the review stage can park it in Needs manual review.

7. Return to **Step 1** and pick the next chore. Do not pause for human input.

### Step 3 — Summary

When no workable chores remain, print: chores completed this run (with their
`#id`s), chores resumed from an interrupted run, chores reworked after
rejection, chores parked In progress with a blocker or gate-failure comment,
and anything left with a stated manual check for the review stage.

## Rules

- **One chore at a time.** Never begin the next until the current one is
  committed and moved to Ready for review.
- **Never work a Backlog or Review-refinement chore.** Those columns mean an
  unanswered question.
- **Stay in scope.** A chore is a small, single change — do not let it grow
  into feature work.
- **Never skip the project checks.**
- **Be honest about verification.** Don't fabricate tests for inherently
  visual or UI-side changes; use build + a stated manual check instead.
- **Ready for review, never Done.** The review stage (#349) owns approval.
