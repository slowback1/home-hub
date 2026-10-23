---
name: triage-backlog
description: Unattended pipeline stage 1. Reads vague idea cards from the Kanboard Backlog column, splits oversized ones into focused stubs, parks cards with unmet "is blocked by" dependencies in Blocked, and queues the rest into "To refine" for the refinement stage; every run starts with an unblock scan of Blocked. Never asks questions. Use when the pipeline wrapper runs triage, or when the user says "triage the backlog".
---

# Triage Backlog

Stage 1 of the Kanboard pipeline:

```
Backlog -> [triage] -> To refine -> [refine] -> Review refinement ->
(human approves) -> Ready to implement -> [implement] -> In progress ->
Ready for review -> [review] -> Done
   \-> Blocked (card waits on unfinished "is blocked by" dependencies;
       re-scanned for unblocking at the start of every triage run)
```

Reads the **Features swimlane Backlog** column and turns each card into one or
more focused tickets in **To refine**. All Kanboard access goes through
`../../kanboard/kb.py`.

## Unattended rules

- **Never ask questions** — there is no human present.
- **Judge only.** Do not refine, spec, or implement anything in this stage.
- Process at most **5 cards per run** (oldest first); leftovers are picked up by
  the next run. The Step 0 unblock scan is **unlimited** and does not count
  against this cap.

## Workflow

### Step 0 — Unblock scan

```bash
.agents/kanboard/kb.py stage list "Blocked" --lane Features
```

For each Blocked card, read its dependencies:

```bash
.agents/kanboard/kb.py link list <id>
```

If every "is blocked by" row shows `column: Done` (or the card has no
"is blocked by" links at all), promote it:

```bash
.agents/kanboard/kb.py comment add <id> --text "unblocked by #<dep-id>[, #<dep-id>...] — all dependencies Done."
.agents/kanboard/kb.py move <id> "To refine"
```

Cards with at least one non-Done dependency stay in Blocked.

### Step 1 — List the Backlog

```bash
.agents/kanboard/kb.py stage list "Backlog" --lane Features
```

Each row has `id`, `title`, `priority`, `children`, `is_breakdown_child`.

- Skip rows with `children > 0` or `is_breakdown_child` true — already triaged
  or structured.
- Take the oldest ≤ 5 remaining rows. If empty, print "nothing to triage" and
  stop.

### Step 2 — Read each card

```bash
.agents/kanboard/kb.py prd show <id>
```

### Step 3 — Decide: keep or split

Keep as one ticket when it is a single concern completable as one PRD.

Split when (same signals as `ideate-feature`):

- it depends on infrastructure or platform work that doesn't exist yet
- there are clearly separable concerns that would be sequenced or owned
  differently
- it is large enough that one PRD would be unwieldy

Split into **2–4 stubs** maximum. Each stub: one concern, its own imperative
title.

### Step 4a — Keep: dependency gate, then move to To refine

Before moving, check declared dependencies:

```bash
.agents/kanboard/kb.py link list <id>
```

If any "is blocked by" row shows a column other than Done, park the card in
Blocked instead — comment naming the unmet dependencies, then move:

```bash
.agents/kanboard/kb.py comment add <id> --text "Blocked by #<dep-id> (<dep-title>) — not Done yet."
.agents/kanboard/kb.py move <id> "Blocked"
```

Otherwise:

```bash
.agents/kanboard/kb.py move <id> "To refine"
```

### Step 4b — Split: create stubs, supersede the original

For each stub, write a short body using
[../ideate-feature/STUB_TEMPLATE.md](../ideate-feature/STUB_TEMPLATE.md) to a
scratch file under `.agents/pipeline/tmp/` (create the dir if missing), then:

```bash
.agents/kanboard/kb.py stub create --title "..." --column "To refine" --priority low|medium|high --desc-file .agents/pipeline/tmp/stub.md
```

Stubs **inherit** the original card's "is blocked by" links (documented
decision): for each such link on the original,

```bash
.agents/kanboard/kb.py link add <stub-id> --blocked-by <dep-id>
```

Then apply the Step 4a gate to each stub: a stub with a non-Done dependency
moves to Blocked (with the unmet-dependency comment) instead of staying in
To refine.

After all stubs exist and are linked, supersede the original:

```bash
.agents/kanboard/kb.py comment add <original-id> --text "Split into #<a>, #<b> — superseded by those cards."
.agents/kanboard/kb.py move <original-id> "Done"
```

The original card is the traceability anchor: every stub links "relates to" it.

### Step 5 — Guard against re-triage

Before touching a card, run `kb.py comment list <id>`: if it already carries a
"Split into" comment from this user, skip it (a previous run crashed mid-split).

### Step 6 — Summary

Print: cards promoted out of Blocked (with their now-Done dependencies), cards
triaged as-is, cards parked in Blocked (with unmet dependencies), splits
created (original → new ids), and cards left for the next run.
