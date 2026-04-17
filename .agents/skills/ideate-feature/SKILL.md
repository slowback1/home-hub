---
name: ideate-feature
description: Capture a rough feature idea as one or more PRD Stubs without going through full refinement. Use when the user wants to brain-dump an idea, add something to the backlog, or capture a future feature they aren't ready to fully spec yet.
---

# Ideate Feature

Turn a rough idea into one or more lightweight PRD Stubs that can be refined into full PRDs later.

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

Wait for the user to confirm the split (or collapse it back to one) before proceeding. Each confirmed stub gets its own slug and file.

### Step 3 — Write the Stub(s)

For each stub:

1. Derive a kebab-case slug from the title (e.g. `notification-preferences`).
2. Write the file to `.agents/stubs/[slug].md` using [STUB_TEMPLATE.md](STUB_TEMPLATE.md).
   - Fill **Title**, **Summary**, **Problem / Opportunity**, **Success Looks Like**, and **Notes & Open Questions** from the conversation.
   - Set **Created** to today's date.
   - Leave any genuinely unknown fields as their placeholder text — do not invent content.
   - Create the `.agents/stubs/` directory if it does not exist.

### Step 4 — Update the Stub Index

Update `.agents/stubs/README.md` to include a row for each new stub.

- If the file does not exist, create it with this structure:

```markdown
# PRD Stubs

Ideas captured for future refinement. Run the `write-prd` skill to promote a stub to a full PRD.

| Stub | Title | Summary | Created |
|------|-------|---------|---------|
```

- Append one row per new stub:

```
| [slug](slug.md) | Title | One-sentence summary | YYYY-MM-DD |
```

### Step 5 — Confirm

Tell the user:
- The file path(s) written
- How many stubs are now in `.agents/stubs/README.md`

Example: "Written to `.agents/stubs/notification-preferences.md`. You now have 3 stubs awaiting refinement."
