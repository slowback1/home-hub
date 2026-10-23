---
name: refine-ticket
description: Unattended pipeline stage 2. Autonomously refines each "To refine" ticket into a full PRD (exploring the codebase to answer questions where possible), posts remaining open questions as a ticket comment, and moves the card to "Review refinement" for human approval. Never interviews the user. Use when the pipeline wrapper runs refine, or when the user says "refine the tickets".
---

# Refine Ticket

Stage 2: turn each **To refine** card into a reviewable PRD without a human
interview. The human review happens later in **Review refinement** — this stage
must leave open questions visible, never silently guess product decisions.

All Kanboard access goes through `../../kanboard/kb.py`.

## Unattended rules

- **Never ask questions.** If an answer isn't in the card, its comments, or the
  codebase, record it as an open question.
- **Explore only inside the repo.** Never read or list paths outside the
  workspace (no `~` paths, no system directories like `~/.local/share`); in
  unattended runs an out-of-workspace permission prompt is auto-rejected and
  kills the session. Environment facts you can't confirm from the repo go in
  Open Questions instead.
- Process every card in To refine (oldest first).
- A card sent back from Review refinement with human comments is a
  **re-refinement**: fold those answers in before anything else.

## Workflow

### Step 1 — List tickets

```bash
.agents/kanboard/kb.py stage list "To refine" --lane Features
```

### Step 2 — Read the card and its comments

```bash
.agents/kanboard/kb.py prd show <id>
.agents/kanboard/kb.py comment list <id>
```

Comments with `"mine": false` are **human answers/feedback** — authoritative
input; fold them into the PRD. Comments from this user are its own earlier
open questions — re-check whether they are now answerable.

### Step 3 — Write the PRD autonomously

This is the `write-prd` flow minus the interview:

1. **Explore the codebase** to answer as much as possible: architecture,
   existing patterns, where this feature would plug in, what already exists.
2. Draft the PRD with
   [../write-prd/PRD_TEMPLATE.md](../write-prd/PRD_TEMPLATE.md):
   Goals / Non-Goals / User Stories / Proposed Solution / Technical Approach.
   Status: `Draft`.
3. **Open Questions**: everything genuinely unresolved — scope, UX, and priority
   decisions belong here, not guesses. Do not hide uncertainty.

### Step 4 — Write the PRD and post open questions

Write the PRD body to a scratch file under `.agents/pipeline/tmp/` (create the
dir if missing — keep scratch files inside the repo so unattended runs never
hit out-of-workspace permission prompts), then (this moves the card to Review
refinement):

```bash
.agents/kanboard/kb.py prd write <id> --desc-file .agents/pipeline/tmp/prd.md --column "Review refinement"
```

If there are open questions, post them as one comment (numbered list; the human
answers by replying in the Kanboard UI):

```bash
.agents/kanboard/kb.py comment add <id> --text-file .agents/pipeline/tmp/questions.md
```

### Step 5 — Summary

Print: tickets refined (ids + urls), tickets re-refined after human feedback,
and the count of open questions posted.
