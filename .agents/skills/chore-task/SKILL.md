---
name: chore-task
description: Works off captured Chores one at a time using TDD. Reads the Kanboard Chores lane, picks the highest-priority Ready to implement chore (resuming any in In progress), fixes it with the red-green-refactor loop (or a build plus manual gate when not unit-testable), runs the project checks, auto-commits, moves it to Done, and continues. Use when the user wants to clear the chore list, knock out logged bugs, or "work the chores".
---

# Chore Task

Implement captured Chores one at a time. Each iteration: pick the next workable
chore → move it to In progress → fix with TDD → run the project checks →
auto-commit → move it to Done → repeat. No human input is required between chores.

Chores live in the **Chores swimlane** of the Kanboard project, reached through
the shared helper `../../kanboard/kb.py`. Columns carry meaning:

- **Backlog** — has an open question; NOT workable without asking the user first.
- **Ready to implement** — actionable cold; safe to pick up now.
- **In progress** — currently being worked (an interrupted run to resume).
- **Done** — finished. (Chores skip the Features pipeline's review columns.)

Load the `tdd` skill now and keep its principles active for the full session.
This is the small-change sibling of `tdd-task` (which works a PRD's task list);
chores are standalone single changes rather than a sequenced breakdown.

## Workflow

### Step 1 — List chores

```bash
.agents/kanboard/kb.py chore list
```

This returns open chores (everything except Done) as JSON, already ordered
In-progress → Ready to implement → Backlog, then by priority (high→low), then
oldest first. Each row has `id`, `title`, `column`, `priority`, `type`, and `url`.

If the command errors, show the user the message and stop — do not fall back to
files.

### Step 2 — Select the next chore

From the list, take the first chore whose column is **In progress**
(an interrupted session — resume it) or **Ready to implement**.

- Chores in **Backlog are not workable** — they still have an open question. Skip
  them.
- If the only open chores are in Backlog (nothing in In progress or Ready to
  implement), **stop** and tell the user those chores need their open questions
  answered (list them with their `url`s) before they can be picked up, then go to
  **Step 6**.
- If there are no open chores at all, go to **Step 6**.

Announce:

```
Starting chore: [title] ([type], [priority])  #[id]
[one-line summary from the description]
```

Move it to In progress (this also assigns it to you):

```bash
.agents/kanboard/kb.py chore start [id]
```

Read the full description for its Details / Proposed Fix:

```bash
.agents/kanboard/kb.py chore show [id]
```

### Step 3 — Fix using TDD

Follow the `tdd` workflow:
- Read the chore's Details and Proposed Fix / Notes from `chore show`. If
  something blocking is ambiguous, ask the user before writing code.
- **Reproduce first.** For a bug, write a failing test that captures the wrong
  behavior (RED), then make it pass (GREEN). For a small change, write a test
  for the new behavior, then implement it.
- Stay strictly within this one chore's scope — a chore is a single small change.

**Not everything is unit-testable** (e.g. UI rendering, a visual tweak, an
asset change). When a red-green test genuinely isn't possible, say so plainly,
make the change, and rely on a successful build plus a clearly-stated manual
verification step instead. Do **not** fabricate a test that doesn't really
exercise the behavior.

### Step 4 — Run the project checks

Run the project's precommit/test gate — `task precommit` if the project defines
it (this repo defines it: backend + frontend + gpu-service checks), otherwise
the project's build-and-test (`dotnet build` + `dotnet test` against
`backend/HomeHubBackend.sln` for the C# backend, `npm test` for the frontend).

- **Pass** → Step 5.
- **Fail** → fix autonomously and re-run, up to **3 attempts**. If still failing,
  stop, show the user the failures, and ask how to proceed. Leave the chore in
  In progress — do not move it to Done.
- For a chore that wasn't unit-testable, the gate is a successful build; note any
  manual visual check that remains for the user rather than claiming it passed.

### Step 5 — Commit, mark Done, and continue

1. Move the chore to Done:

   ```bash
   .agents/kanboard/kb.py chore done [id]
   ```

2. Commit all changes:

   ```
   {short summary of the fix}

   Addresses chore #{id}: {title}
   {kanboard url}
   ```

   End the commit message with the project's required trailer if it has one.
3. Return to **Step 1** immediately and pick the next chore. Do not pause for
   human input.

### Step 6 — Done

When no workable chores remain, print a short summary of the chores completed
this run (with their `#id`s, and flag any that left a manual verification step),
note any Backlog chores that were skipped for open questions, then stop.

## Rules

- **One chore at a time.** Never begin the next until the current one is committed
  and moved to Done.
- **Never work a Backlog chore.** Backlog means an unanswered question — surface it
  to the user instead of guessing.
- **Stay in scope.** A chore is a small, single change — do not let it grow into
  feature work. If it turns out to be large, stop and suggest `write-prd`.
- **Never skip the project checks.**
- **Be honest about verification.** Don't fabricate tests for inherently visual or
  UI-side changes; use build + a stated manual check instead.
