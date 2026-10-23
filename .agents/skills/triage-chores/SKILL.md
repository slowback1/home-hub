---
name: triage-chores
description: Unattended pipeline stage 1 of the chore flow. Reads Chores-swimlane Backlog cards, plans obvious chores into In progress with a Plan comment, sends unclear chores to Review refinement with a numbered question comment, and re-queues human-answered cards back to Backlog; never asks questions. Use when the pipeline wrapper runs triage-chores, or when the user says "triage the chores".
---

# Triage Chores

Stage 1 of the chore pipeline:

```
Backlog -> [triage-chores] -> In progress -> [implement-chores #348] ->
Ready for review -> [review-chores #349] -> Done
   \-> Review refinement (unclear chore waits for a human answer,
       then returns to Backlog for the next run)
```

Reads the **Chores swimlane Backlog** column and turns each card into either a
planned In-progress chore or a questioned Review-refinement chore. All Kanboard
access goes through `../../kanboard/kb.py`.

## Unattended rules

- **Never ask questions** — there is no human present.
- **Judge only.** Do not implement, refine, or review any chore in this stage.
- Process at most **5 cards per run** (oldest first); leftovers are picked up by
  the next run. The Step 0 human-answer scan is **unlimited** and does not count
  against this cap.

## Workflow

### Step 0 — Human-answer scan

```bash
.agents/kanboard/kb.py stage list "Review refinement" --lane Chores
```

For each Review refinement card, read its comments:

```bash
.agents/kanboard/kb.py comment list <id>
```

If the card carries a human answer comment (a comment with `mine: false` posted
after the pipeline's `Questions:` comment), return it to the triage queue so the
next run folds the answer in:

```bash
.agents/kanboard/kb.py comment add <id> --text "Answered — returning to Backlog for re-triage."
.agents/kanboard/kb.py move <id> "Backlog"
```

Cards still carrying only the pipeline's `Questions:` comment stay in Review
refinement.

### Step 1 — List the Backlog

```bash
.agents/kanboard/kb.py stage list "Backlog" --lane Chores
```

Each row has `id`, `title`, `priority`, `children`, `is_breakdown_child`.

Take the oldest ≤ 5 rows. If empty, print "nothing to triage" and stop.

### Step 2 — Read each card

```bash
.agents/kanboard/kb.py chore show <id>
```

### Step 3 — Obviousness heuristic

A chore is **obvious** when ALL hold:

- it is a single concern (one small change, one commit-sized);
- the description contains a repro, or a concrete proposed fix, or points at
  the file/area to change;
- acting on it requires no product or priority decision (no "which behavior is
  correct?" ambiguity).

Anything failing the heuristic is **unclear**. The heuristic is deterministic:
there is no human present to judge judgment calls, so borderline cards are
unclear.

### Step 4a — Obvious: plan it into In progress

Write a plan comment starting `Plan:` with 2–4 numbered steps and the test/gate
to use (per the repo gate: Taskfile `precommit`, `dotnet build`/`dotnet test`,
`npm test`, or `pytest`):

```bash
.agents/kanboard/kb.py comment add <id> --text "Plan:
1. <step>
2. <step>
3. <step>
Gate: <test/gate to run>"
.agents/kanboard/kb.py chore start <id>
```

`chore start` moves the card to In progress and assigns it to the pipeline
user, matching chore-task semantics. The implement stage (#348) reads the plan
via `kb.py comment list`.

### Step 4b — Unclear: question it into Review refinement

Post a numbered question comment listing **exactly what is missing** from the
heuristic (e.g. no repro, ambiguous desired behavior, multiple concerns), then
move:

```bash
.agents/kanboard/kb.py comment add <id> --text "Questions:
1. <question or missing-information item>
2. <...>"
.agents/kanboard/kb.py move <id> "Review refinement"
```

The human answers the comment and the Step 0 scan returns the card to Backlog
on the next run.

### Step 5 — Guard against re-triage

Before touching a card, run `kb.py comment list <id>`: if it already carries a
`Plan:` or `Questions:` comment from this user, skip it (a previous run already
triaged it, or crashed mid-triage).

### Step 6 — Summary

Print: cards returned from Review refinement to Backlog (Step 0), chores planned
into In progress (with plan), chores questioned into Review refinement (with
questions), chores skipped by the guard, and cards left for the next run.
