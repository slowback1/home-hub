---
name: write-prd
description: Conducts a structured Q&A interview (via the refine-feature skill) and then writes a PRD into a Kanboard task. Use when the user wants to write a PRD, create a product requirements document, document a plan or design, write up the spec, or produce a feature brief.
---

# Write PRD

Promote a rough stub into a full PRD. The PRD lives in the **description of a task
in the Features swimlane**; writing it fills that description and moves the task
from **Backlog** (stub, open questions) to **Ready to implement** (specified,
ready to break down). All Kanboard access goes through `../../kanboard/kb.py`.

This is the interactive refinement path. The unattended equivalent is the
`refine-ticket` pipeline stage (Backlog → To refine → Review refinement, with a
human approval gate before Ready to implement).

## Quick start

1. Look for an existing stub matching this feature with `.agents/kanboard/kb.py prd list` (Backlog items are stubs). If one matches, note its **task id** — that task will be promoted in place.
2. If a `refine-feature` session hasn't already run in this conversation, load the `refine-feature` skill and run the full interview first. Seed it with the stub's current description (`kb.py prd show <id>`) if a stub was found.
3. Synthesize the Q&A into a PRD using [PRD_TEMPLATE.md](PRD_TEMPLATE.md).
4. Write the PRD into the task (Step 3) — promoting the stub, or creating a fresh PRD task if there was no stub.
5. Work through the PRD's Open Questions with the user (Step 3.5), folding each answer back into the description.
6. Tell the user the task URL and ask if any section needs revision.

## Workflow

### Step 0 — Stub check

```bash
.agents/kanboard/kb.py prd list
```

Scan the Backlog items for one whose title/summary matches the feature.

- **Match found**: tell the user "I found a stub for this: **[title]** (task #[id]). I'll promote it in place." Record the id; read its body with `kb.py prd show <id>` to seed the interview.
- **No match**: proceed; you'll create a fresh PRD task in Step 3.

### Step 1 — Interview (if needed)

Load the `refine-feature` skill. The interview must cover:
- Problem being solved and who it affects
- Goals and explicit non-goals
- Proposed solution and key technical decisions
- Open questions and unresolved risks
- Success criteria

Do not skip or cut short the interview. The PRD quality depends on it.

### Step 2 — Draft the PRD

Map interview answers to [PRD_TEMPLATE.md](PRD_TEMPLATE.md). Follow these rules:
- **Status** starts as `Draft` unless the user says otherwise.
- **Open Questions** should include anything unresolved from the interview — do not hide uncertainty.
- **Technical Approach** should capture key decisions made during the interview, not re-ask them.
- Leave optional sections (Timeline, Success Metrics) with a `_TBD_` placeholder if not covered; write `_N/A_` for a section with nothing meaningful. Do not pad.

The PRD title/name is the task title — keep the drafted body free of a redundant top-level title line if you like, but a `# PRD: …` heading inside the description is fine (Kanboard renders it).

### Step 3 — Write into the task

Write the drafted PRD to a temp file, then:

- **Promoting a stub** (Step 0 found one): this fills the description and moves it Backlog → Ready to implement.

  ```bash
  .agents/kanboard/kb.py prd write <id> --desc-file /path/to/prd.md
  ```

  If the feature's title changed materially during refinement, that's fine — the task keeps its id; mention the rename to the user (renaming the card itself is a manual tweak in the UI if they want it).

- **No stub** (fresh PRD): create the task straight into Ready to implement.

  ```bash
  .agents/kanboard/kb.py prd create --title "Feature Name" --desc-file /path/to/prd.md
  ```

Give the user the task `url` from the output.

### Step 3.5 — Resolve open questions

Immediately after writing, work through the PRD's **Open Questions** with the user — do not wait to be prompted. This is required.

- If there are none, say so and skip ahead.
- Otherwise ask them one at a time (or in small related groups), in priority order — the questions that most affect scope or technical approach first. Prefer `AskUserQuestion` for discrete choices; prose for open-ended ones.
- The user may defer a question ("leave that one open"). That's fine — leave it unchecked and move on.
- As each is answered, fold it back into the drafted PRD body: check off the Open Questions item and record the decision in the right section (usually **Technical Approach → Key Decisions**). Re-save the temp file and re-write the task description with `kb.py prd write <id> --desc-file …` (idempotent — the task stays in Ready to implement).
- After the pass, briefly summarize what resolved and what remains open.

Note on columns: the normal end state is **Ready to implement**. If the user deliberately leaves a *blocking* question open (something that must be answered before any work can start), say so and offer to leave/return the card to Backlog instead — Backlog is precisely "has an open question."

### Step 4 — Confirm

Tell the user the task URL and that the stub is now a Ready-to-implement PRD (no separate stub
to clean up — it was promoted in place). Ask if any section needs revision.

## Notes

- One task per feature carries the whole lifecycle: Backlog (stub) → Ready to
  implement (PRD) → In progress (implementing) → Ready for review → Done.
  `breakdown-prd` then hangs child tasks off this one; `tdd-task` works those
  children.
- Superseding a PRD is a manual call — update the description and, if you want it
  off the active board, move the card to Done in the UI.
