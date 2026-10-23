---
name: review-ready
description: Unattended pipeline stage 4. Reviews everything parked in "Ready for review": child tasks are checked against their acceptance criteria and committed diffs (approve to Done, park children with a Manual check criterion in "Needs manual review" with a playtest script, or reject with a comment back to Ready to implement); PRD parents get a whole-feature review and mirror their least-far child's column. Use when the pipeline wrapper runs review, or when the user says "review the ready work".
---

# Review Ready

Stage 4: the code-review gate. Reviews cards in **Ready for review** (Features
swimlane) and either approves them to **Done**, parks playtest-gated work in
**Needs manual review**, or rejects them back to **Ready to implement** with an
explanatory comment. All Kanboard access goes through `../../kanboard/kb.py`.

## Unattended rules

- **Never ask questions.** Review strictly against the card's own acceptance
  criteria / PRD goals — not personal taste.
- **Be skeptical.** A self-written implementation is not automatically correct.
  Check the diff, re-run the tests, look for scope creep and fabricated tests.
- **Push approved work**: once every review check and the gate pass for a card,
  push main to origin. Never push when any check or gate fails.

## Workflow

### Step 1 — List the column

```bash
.agents/kanboard/kb.py stage list "Ready for review" --lane Features
```

Split rows into children (`is_breakdown_child` true) and PRD parents.

### Step 2 — Review each child task

1. `kb.py task show <id>` — Description + Acceptance Criteria.
2. Find its commits (the implement stage footers `Addresses {child-title}`):

   ```bash
   git log --grep="{child-title}" --format="%H %s"
   ```

   No commits found → reject.
3. Review those commits (`git show`) against the acceptance criteria:
   - every criterion actually implemented **and tested**?
   - tests real (they exercise behavior, not fabricated)?
   - scope respected (nothing belonging to later tasks)?
   - gate evidence: re-run the project gate (see `implement-ready`) — must pass.
4. **Manual-check routing.** If every automatable check passes **and** the
   acceptance criteria contain a `Manual check:` line, do **not** approve to
   Done — the evidence for that criterion is a human playtest you cannot
   perform. Comment the concrete playtest script (the `Manual check:` steps
   plus the expected observations, expanded so a human can follow them
   verbatim), then park the child:

   ```bash
   .agents/kanboard/kb.py comment add <id> --text-file .agents/pipeline/tmp/playtest.md
   .agents/kanboard/kb.py move <id> "Needs manual review"
   ```

   Fully-automatable children (no `Manual check:` line) go to Done as before:
   `kb.py task done <id>`.

   A card in "Needs manual review" has passed AI review but is not yet
   confirmed playable. **No child ever leaves that column except by a human
   action**: the human playtests and either `kb.py task done <id>` (straight to
   Done) or comments the failure and moves it back to "Ready to implement" for
   rework. The review stage never moves cards out of "Needs manual review".

5. **Reject** → comment + send back:

   ```bash
   .agents/kanboard/kb.py comment add <id> --text-file .agents/pipeline/tmp/rejection.md
   .agents/kanboard/kb.py move <id> "Ready to implement"
   ```

   The rejection comment lists each failed criterion concretely and what to
   change — the implement stage reworks it verbatim next run.

   **Guarantee: every rejection posts its comment before the card moves
   back.** The implement stage's recovery pass distinguishes a rejected child
   from a crashed one solely by the presence of this reviewer comment; a
   rejected card without one is indistinguishable from an interrupted run.

### Step 3 — Review each PRD parent

Triggered when a parent sits in Ready for review. A parked child counts as
implemented-awaiting-human: the parent is reviewable once every child is Done
or in "Needs manual review".

1. `kb.py prd show <parent-id>` — PRD Goals / User Stories + `children` list.
2. Verify the whole feature: `kb.py task list <parent-id>` must show every
   child Done or in "Needs manual review"; skim the feature's commits
   (`git log --grep="{parent-title}"` plus the child titles) against the PRD's
   Goals.
3. **Approve, mirroring the least-far child:**
   - any child parked in "Needs manual review" → park the parent there too
     (`kb.py move <parent-id> "Needs manual review"`) so one glance shows what
     still needs human sign-off;
   - all children Done → `kb.py feature done <parent-id>`.

   When the human approves the last parked child to Done, the next review run
   sees all children Done and approves the parent.
4. **Reject** → comment listing what is missing, then:

   ```bash
   .agents/kanboard/kb.py move <parent-id> "Ready to implement"
   ```

   Same guarantee as child rejections: the comment is posted before the move.

   Optionally create a fix child (`kb.py task add <parent-id> --name "Fix ..."`)
   so the gap is tracked as work, not prose.

### Step 4 — Summary

Print: children approved/parked-for-manual-review/rejected (ids), PRDs
approved/parked/rejected, and a one-line reason for every rejection.
