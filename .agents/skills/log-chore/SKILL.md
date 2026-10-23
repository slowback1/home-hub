---
name: log-chore
description: Capture a bug or minor change as a lightweight Chore in the Kanboard Chores lane without stopping to fix it now. Use when the user spots a bug, wants to jot down a small fix or tweak for later, or says "log this", "note this bug", "add a chore", or "capture this for later".
---

# Log Chore

Quickly record a bug or small change as a Chore in Kanboard so it isn't lost,
without breaking the current flow. The sister `chore-task` skill works these off
later. Chores live in the **Chores swimlane** of the Kanboard project — not in
files. All Kanboard access goes through the shared helper:

```
.agents/kanboard/kb.py
```

A Chore is small by definition — a bug, a minor tweak, or a small enhancement
that does not warrant a PRD. For anything larger (a real feature), use
`ideate-feature` / `write-prd` instead.

## Workflow

### Step 1 — Capture

Listen to what the user describes. Because chores are small, do **not** run a
full interview. If the gist is clear, reflect it back in one line and confirm.
Ask at most one clarifying question, and only if you otherwise cannot write a
record that future-you could act on cold (e.g. you don't know where a bug
appears or what the intended behavior is).

Classify each item:
- **Type**: `bug` | `chore` | `enhancement`
- **Priority**: `low` | `medium` | `high` — only ask if it's unclear and matters;
  otherwise default `medium`.
- **Column**: decide where it lands —
  - **Ready to implement** — actionable cold; someone (or `chore-task`) could
    pick it up now with no further questions. This is the default for a
    well-captured chore.
  - **Backlog** — the capture still has an genuinely open question that must be
    answered before work can start. Only use this when you truly left something
    unresolved; note the open question in the description.

A single description may contain several distinct items — split them into
separate chores rather than cramming them into one.

Prefer recording what's genuinely non-obvious (repro, the surprising behavior,
the file you suspect) over restating what the code already makes clear.

### Step 2 — Create the chore(s) in Kanboard

For each item, write the description body using
[CHORE_TEMPLATE.md](CHORE_TEMPLATE.md) (Summary / Details / Proposed Fix —
Markdown; Kanboard renders it). Type, priority, status, and date are NOT in the
body — they are native Kanboard fields set by the flags below.

Write the body to a temp file, then create the task:

```bash
.agents/kanboard/kb.py chore create \
  --title "Short imperative title" \
  --type bug|chore|enhancement \
  --priority low|medium|high \
  --column "Ready to implement"|Backlog \
  --desc-file /path/to/body.md
```

The command prints JSON with the new task `id`, `column`, and `url`. If it
errors (e.g. connection/config), show the user the message — do not silently
fall back to files.

### Step 3 — Confirm

Tell the user, for each chore: the title, the Kanboard `url`, and which column
it landed in (and, if Backlog, the open question that put it there). Then report
how many chores are now open in the Chores lane:

```bash
.agents/kanboard/kb.py chore list
```

Do **not** start fixing anything — capturing is the whole job.
