---
name: ideate-feature
description: Capture a rough feature idea as one or more PRD Stubs without going through full refinement. Use when the user wants to brain-dump an idea, add something to the backlog, or capture a future feature they aren't ready to fully spec yet.
---

# Ideate Feature

Turn a rough idea into one or more lightweight PRD Stubs that can be refined into
full PRDs later. Stubs live as tasks in the **Features swimlane** of the Kanboard
project, in the **Backlog** column (Backlog = still has open questions, not ready
to pick up — exactly what a stub is). The hourly `triage-backlog` pipeline stage
picks Backlog cards up, splits them if needed, and queues them for refinement.
All Kanboard access goes through the shared helper `../../kanboard/kb.py`.

## Workflow

### Step 1 — Capture the Idea

Listen to the user's description of the idea. Assess how much clarity already exists:

- **If the core idea is obvious** (problem, rough shape, who benefits are all clear): briefly reflect it back in one or two sentences and confirm — "It sounds like you want X in order to Y. Is that right?" — then move straight to Step 2.
- **If key context is missing**: ask a small number of targeted follow-up questions, one at a time, until the idea is clear enough to write down. Focus on:
  - What problem does this solve, and who experiences it?
  - What would success look like?
  - Any known constraints or dependencies?

Do not over-interview. The goal is a stub, not a finished spec. Stop when you have enough to write it down meaningfully.

### Step 2 — Check for Required Splits

Before writing anything, consider whether this idea naturally decomposes into multiple PRDs. Common signals:

- The feature depends on infrastructure or platform work that doesn't exist yet (e.g. a new API, a data model, an integration)
- There are clearly separable concerns that would be sequenced differently or owned differently
- The idea is large enough that one PRD would be unwieldy

If a split makes sense, propose it explicitly:

> "This looks like it might need two stubs: one for [infra/prerequisite], and one for [the feature itself]. Does that split make sense, or would you prefer to keep it as one?"

Wait for the user to confirm the split (or collapse it back to one) before proceeding. Each confirmed stub becomes its own task.

### Step 3 — Write the Stub(s)

For each stub, write the description body using
[STUB_TEMPLATE.md](STUB_TEMPLATE.md) (Summary / Problem / Success Looks Like /
Notes & Open Questions — Markdown; Kanboard renders it). Do not repeat the title
or a status/date line in the body — the task title and Backlog column carry those.

Write the body to a temp file, then create the task:

```bash
.agents/kanboard/kb.py stub create --title "Feature Name" --desc-file /path/to/body.md
```

`stub create` lands the task in **Backlog** by default. If the command errors
(connection/config), show the user the message — do not fall back to files.

### Step 4 — Confirm

Tell the user, for each stub: the title and the Kanboard `url` from the command
output. Then report how many stubs/PRDs are now open in the Features lane:

```bash
.agents/kanboard/kb.py prd list
```

Example: "Captured **Notification Preferences** → [task #27]. You now have 14
items in the Features lane awaiting refinement."
