---
name: review-chores
description: Unattended pipeline stage 3 of the chore flow. Reviews each Chores-swimlane card in Ready for review against its Plan comment and committed diff (approve to Done, park chores with a stated manual check in Needs manual review with a playtest script, or reject with a rework comment back to In progress), re-running the project gate. Never asks questions; no card ever leaves Needs manual review via the pipeline. Use when the pipeline wrapper runs review-chores, or when the user says "review the chores".
---

# Review Chores

Stage 3 of the chore pipeline:

```
Backlog -> [triage-chores #347] -> In progress -> [implement-chores #348] ->
Ready for review -> [review-chores] -> Done
```

This is `review-ready`'s review principles applied to single chore cards:
skepticism, commit-grep lookup, gate re-run, the comment-before-move guarantee,
and Needs-manual-review routing — but none of its PRD/child machinery (no
children, no parent mirroring, no seq ordering). All Kanboard access goes
through `../../kanboard/kb.py`.

## Unattended rules

- **Never ask questions.** Review strictly against the chore's own criteria
  and its `Plan:` comment — not personal taste.
- **Be skeptical.** A self-written implementation is not automatically
  correct. Check the diff, re-run the gate, look for scope creep and
  fabricated tests.
- The review stage commits nothing; it only reads history and moves cards.
- **Push approved work**: once every review check and the gate pass for a card,
  push main to origin. Never push when any check or gate fails.

## Project gate

Re-run the repo's checks for every reviewed chore. Detect in order (same as
`implement-chores`):

1. `Taskfile.yml` with a `precommit` task → `task precommit`
2. `*.sln` present → `dotnet build` + `dotnet test` (this repo:
   `backend/HomeHubBackend.sln`)
3. `package.json` with a test script → `npm test`
4. pytest config or a `tests/` dir → `pytest`
5. None of the above → syntax-check every changed file and note in the card
   comment that no test gate exists yet.

## Workflow

### Step 1 — List the column

```bash
.agents/kanboard/kb.py stage list "Ready for review" --lane Chores
```

Process in priority order (high → low), oldest first. If the list is empty,
print "no chores to review" and stop.

**Only this column is workable.** Never touch chores in Backlog, Review
refinement, In progress, Done, or Needs manual review.

### Step 2 — Review one chore

1. Read the chore and its plan:

   ```bash
   .agents/kanboard/kb.py chore show <id>
   .agents/kanboard/kb.py comment list <id>
   ```

   The triage `Plan:` comment (#347) and the chore's Details / Proposed Fix
   are the review brief.

2. Find its commits (the implement stage footers `Addresses {chore-title}`):

   ```bash
   git log --grep="{chore-title}" --format="%H %s"
   ```

   No commits found → **reject** (Step 2.5).

3. Review each commit (`git show`) against the plan and criteria:
   - plan actually implemented?
   - every stated criterion met?
   - tests real (they exercise behavior, not fabricated)?
   - scope respected (one small change, no feature creep)?
   - gate re-run passes (see Project gate above).

4. **Manual-check routing.** If every automatable check passes **and** the
   chore's plan or implement-stage comment states a manual check
   (non-unit-testable work: UI-side, visual, operational), do **not**
   approve to Done. Comment a concrete playtest script — the stated manual
   check steps plus expected observations, expanded so a human can follow
   them verbatim — then park the chore:

   ```bash
   .agents/kanboard/kb.py comment add <id> --text-file .agents/pipeline/tmp/playtest.md
   .agents/kanboard/kb.py move <id> "Needs manual review"
   ```

   A chore in "Needs manual review" has passed AI review but is not yet
   human-confirmed. **No chore ever leaves that column via the pipeline**:
   only a human moves it (to Done, or back for rework).

5. **Reject** → comment + send back for rework:

   ```bash
   .agents/kanboard/kb.py comment add <id> --text-file .agents/pipeline/tmp/rejection.md
   .agents/kanboard/kb.py move <id> "In progress"
   ```

   The rejection comment lists each failed criterion concretely and what to
   change — implement-chores reworks it verbatim next run.

   **Guarantee: every rejection posts its comment before the card moves
   back.** implement-chores' recovery pass distinguishes a rejected chore
   from a crashed one solely by the presence of this reviewer comment; a
   rejected card without one is indistinguishable from an interrupted run.

6. **Approve** (all checks pass, no manual check) →

   ```bash
   .agents/kanboard/kb.py chore done <id>
   ```

7. Return to **Step 1** and pick the next chore. Do not pause for human input.

### Step 3 — Summary

When the column is empty, print: chores approved to Done (with their `#id`s),
chores parked in Needs manual review (with their `#id`s), and chores rejected
back to In progress with a one-line reason each.

## Rules

- **One chore at a time.** Finish routing one before starting the next.
- **Comment before move** on every rejection — no exceptions.
- **Never move a card out of Needs manual review.** Human-only exit.
- **Never review Features cards** — that stays `review-ready`.
- **Never spot-check Done chores.**
- **Be honest about verification.** Fabricated tests are a rejection reason.
