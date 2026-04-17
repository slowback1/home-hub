---
name: breakdown-prd
description: Breaks a PRD into fine-grained commit-sized tasks, each representing one logical unit of change (one model, one endpoint, one component, etc.) with tests always included in the same task. Each task becomes a markdown file stored alongside the PRD. Use when the user wants to break down a PRD into tasks, generate a task list from a PRD, or plan implementation work.
---

# Breakdown PRD

Decompose an approved PRD into a sequenced list of coarser feature-slice tasks that group related concerns together. Each task becomes a markdown file in the same folder as the PRD.

## Quick Start

1. **Locate the PRD** — discover which PRD(s) exist under `.agents/prds/`.
   - If exactly one PRD folder exists, use it.
   - If multiple exist, list them and ask the user which one to break down.
2. **Check for existing tasks** — scan the PRD folder for any files matching `NN-*.md`.
   - If any exist, stop and warn the user. Do not overwrite. Tell the user to delete the existing task files if they want to regenerate.
3. **Read the PRD** — read the full PRD file at `.agents/prds/[slug]/[slug].md`.
4. **Propose the task breakdown** — generate a numbered list of proposed tasks (title + one-line summary each) and present it to the user for review.
5. **Wait for approval** — do not write any files until the user confirms the breakdown looks correct (or requests changes).
6. **Write task files** — write each approved task as a markdown file using [TASK_TEMPLATE.md](TASK_TEMPLATE.md).
7. **Confirm** — tell the user how many task files were written and their paths.

---

## Workflow

### Step 1 — Locate the PRD

Scan `.agents/prds/` for subdirectories. Each subdirectory is a PRD slug; the PRD file is at `.agents/prds/[slug]/[slug].md`.

- **One PRD found**: proceed automatically.
- **Multiple PRDs found**: list them (slug + first line of overview) and ask which to use.
- **No PRDs found**: stop and tell the user no PRDs exist yet. Suggest running the `write-prd` skill first.

### Step 2 — Guard: existing tasks

Before generating anything, check whether the PRD folder already contains files matching `NN-*.md` (e.g. `01-init-schema.md`).

If any are found:
- List the existing task files.
- Tell the user: "Task files already exist for this PRD. Delete them first if you want to regenerate the breakdown."
- Stop. Do not proceed.

### Step 3 — Read and analyze the PRD

Read the full PRD. Focus on these sections when generating tasks:
- **Goals** — what must be true when done
- **Non-Goals** — what to explicitly exclude from tasks
- **User Stories / Use Cases** — the behaviors to implement
- **E2E Scenarios** — Gherkin scenarios to be written and verified as part of the first task (if present)
- **Proposed Solution** — the approach
- **Technical Approach / Key Decisions** — architectural constraints
- **Open Questions** — flag unresolved items in task Notes

### Step 4 — Generate the task breakdown

Decompose the PRD into tasks following these rules:

**E2E-first rule**

If the PRD contains a non-empty `E2E Scenarios` section, **task 01 must always be "Write & verify E2E scenarios"**. This task:

- Writes the `.feature` file to `e2e/features/<prd-slug>.feature` using the scenarios from the PRD's `E2E Scenarios` section.
- Stubs the corresponding step definition file in `e2e/steps/<prd-slug>.steps.ts` and any required Page Objects in `e2e/pages/`. Stubs should throw a `not implemented` error or use a `todo()` call so the tests are discovered but fail meaningfully.
- Runs `task e2e:test` targeting the new scenarios. The acceptance criteria require the new scenarios to **FAIL** with a meaningful test assertion or `not implemented` error — not a tooling, configuration, or compilation error. This confirmed-RED state is the gate for task 01.

All other tasks are numbered 02, 03, … and implement the feature itself. The final implementation task's acceptance criteria must include running `task e2e:test` and all new scenarios passing GREEN.

If the PRD does not have an `E2E Scenarios` section (or it is empty), skip this rule and number tasks from 01 as normal.

**Granularity**
- Each task should represent **one logical unit of change** — one concept, one concern, one thing that naturally wants to be a single commit.
- Good examples of a right-sized task: one model, one migration, one endpoint, one component, one utility function, one configuration change, one route wiring.
- **Tests and implementation always land in the same task.** This is the primary rule. Never create a separate "add tests for X" task — tests for X belong in the task that implements X.
- There is no target task count per PRD. A substantial PRD producing 10–15 tasks is fine. Do not merge tasks to hit an artificial ceiling.
- A task is **too large** if you can describe two independently useful things it does — split it.
- A task is **too small** if it has no standalone value on its own (e.g. creating an empty placeholder file with no logic) — merge it into the task that gives it meaning.
- Avoid bundling truly unrelated concerns into one task (e.g. two independent features or two unrelated config changes).

**Ordering**
- Order tasks by dependency: foundational work first, dependent work later.
- Typical sequence: project scaffolding → data models/migrations → core business logic → API/service layer → UI/integration → tests → documentation.
- A task should be completable without needing to touch files owned by a later task.

**Acceptance criteria**
- Derive each task's acceptance criteria from the PRD's Goals, User Stories, and Success Metrics sections.
- Make criteria concrete and verifiable (e.g. "endpoint returns 401 for unauthenticated requests" not "auth works").

### Step 5 — Present for review

Output a numbered list in this format:

```
Proposed task breakdown for [PRD name]:

01 — [task title]: [one-line summary]
02 — [task title]: [one-line summary]
...

Does this look right? Reply with approval or describe any changes.
```

Do not write any files yet.

### Step 6 — Write task files

Once the user approves (or after incorporating requested changes):

- Name each file `NN-[slug].md` where `NN` is the 2-digit zero-padded task number and `[slug]` is a kebab-case version of the task title.
- Write files to `.agents/prds/[prd-slug]/`.
- Use [TASK_TEMPLATE.md](TASK_TEMPLATE.md) for the file structure.

### Step 7 — Confirm

After writing all files, output:

```
Wrote N task files to .agents/prds/[slug]/:
  01-[slug].md
  02-[slug].md
  ...
```

---

## File naming examples

| Task title | File name |
|---|---|
| Initialize database schema | `01-initialize-database-schema.md` |
| Add user authentication middleware | `02-add-user-authentication-middleware.md` |
| Implement product listing endpoint | `03-implement-product-listing-endpoint.md` |
