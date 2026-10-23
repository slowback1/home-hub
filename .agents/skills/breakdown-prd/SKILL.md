---
name: breakdown-prd
description: Breaks a PRD into fine-grained commit-sized tasks, each representing one logical unit of change (one model, one endpoint, one component, etc.) with tests always included in the same task. Each task becomes a linked child task under the PRD in Kanboard. Use when the user wants to break down a PRD into tasks, generate a task list from a PRD, or plan implementation work.
---

# Breakdown PRD

Decompose an approved PRD into a sequenced list of commit-sized tasks. Each task
becomes its own **child task in the Features swimlane**, titled `{parentId}-{NN}
name`, linked "is a child of" the PRD task, and starting in **Ready to
implement**. All Kanboard access goes through `../../kanboard/kb.py`.

## Quick Start

1. **Locate the PRD** — `.agents/kanboard/kb.py prd list`. PRDs ready to break down are in the **Ready to implement** column.
2. **Check for existing children** — `kb.py task list <parent-id>`. If any exist, stop and warn; do not regenerate over them.
3. **Read the PRD** — `kb.py prd show <parent-id>` (its description is the PRD).
4. **Propose the breakdown** — a numbered list (title + one-line summary) for the user to review.
5. **Wait for approval** — write nothing until the user confirms.
6. **Create child tasks** — one `kb.py task add` per approved item.
7. **Confirm** — `kb.py task list <parent-id>`.

---

## Workflow

### Step 1 — Locate the PRD

```bash
.agents/kanboard/kb.py prd list
```

Each row is a PRD/stub with its column and a `children` count.

- **User named one**: use that task id.
- **One PRD in Ready to implement**: use it.
- **Multiple in Ready to implement**: list them (id + title) and ask which.
- **Only Backlog items exist**: these are unrefined stubs — stop and suggest running `write-prd` first to promote one to Ready to implement.

### Step 2 — Guard: existing children

```bash
.agents/kanboard/kb.py task list <parent-id>
```

If it returns any children, list them and tell the user: "This PRD already has a
breakdown (N child tasks). Delete them in Kanboard first if you want to
regenerate." Stop. Do not proceed.

### Step 3 — Read and analyze the PRD

Read the full PRD from `kb.py prd show <parent-id>`. Focus on:
- **Goals** — what must be true when done
- **Non-Goals** — what to exclude
- **User Stories / Use Cases** — the behaviors to implement
- **Proposed Solution** — the approach
- **Technical Approach / Key Decisions** — architectural constraints
- **Open Questions** — flag unresolved items in a task's Notes

### Step 4 — Generate the task breakdown

**Granularity**
- Each task is **one logical unit of change** — one concept, one concern, one thing that naturally wants to be a single commit (one model, one endpoint, one component, one utility, one wiring change).
- **Tests and implementation always land in the same task.** Never a separate "add tests for X" task — tests for X belong with X.
- No target count. A substantial PRD producing 10–15 tasks is fine; don't merge to hit a ceiling.
- **Too large** if you can describe two independently useful things it does — split it. **Too small** if it has no standalone value — merge it into the task that gives it meaning.
- Don't bundle unrelated concerns into one task.

**Ordering**
- Order by dependency: foundational work first, dependent work later (scaffolding → data models → core logic → service layer → UI/integration → docs).
- A task should be completable without touching files owned by a later task. The `NN` sequence is the order `tdd-task` will follow.

**Acceptance criteria**
- Derive each task's criteria from the PRD's Goals, User Stories, and Success Metrics. Make them concrete and verifiable ("returns 401 for unauthenticated requests", not "auth works").

### Step 5 — Present for review

```
Proposed task breakdown for [PRD title] (task #[parent-id]):

01 — [task title]: [one-line summary]
02 — [task title]: [one-line summary]
...

Does this look right? Reply with approval or describe any changes.
```

Do not create anything yet.

### Step 6 — Create child tasks

Once the user approves, create the children **in order** (sequence auto-increments,
so create 01 first, then 02, …). For each, write the body with
[TASK_TEMPLATE.md](TASK_TEMPLATE.md) (Description / Acceptance Criteria / Notes) to
a temp file, then:

```bash
.agents/kanboard/kb.py task add <parent-id> --name "task title" --desc-file /path/to/body.md
```

- The command names the task `{parent-id}-{NN} task title`, lands it in **Ready to implement**, tags it `task`, and links it "is a child of" the PRD. It prints the child `id`, `title`, and `linked: true`.
- If any result shows `linked: false` or a `warning`, stop and surface it — the child/parent link id in `kb.py` may need updating (see its `CHILD_OF_LINK_ID`). Don't leave orphaned children.
- Pass `--priority` if a task is unusually high/low; otherwise the default is fine.

### Step 7 — Confirm

```bash
.agents/kanboard/kb.py task list <parent-id>
```

Report: "Created N child tasks under **[PRD title]** (#[parent-id]): `NN-01 …`
through `NN-NN …`, all in Ready to implement. Run `tdd-task` to implement them."

---

## Naming examples (parent PRD is task #11)

| Task title | Kanboard child title |
|---|---|
| Initialize database schema | `11-01 Initialize database schema` |
| Add user authentication middleware | `11-02 Add user authentication middleware` |
| Implement product listing endpoint | `11-03 Implement product listing endpoint` |
