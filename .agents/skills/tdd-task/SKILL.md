---
name: tdd-task
description: Implements tasks from a PRD task list one at a time using TDD. Reads the task list, picks the next pending task, implements it with the red-green-refactor loop, runs precommit checks, then auto-commits and continues to the next task without human input. Use when the user wants to implement a PRD, work through a task list, or start coding from a breakdown.
---

# TDD Task

Implement PRD tasks one at a time using TDD. Each iteration: pick the next task → implement with red-green-refactor → run precommit → auto-commit → mark done → repeat. No human input is required between tasks.

Load the `tdd` skill now and keep its principles active for the full session. All implementation work follows the TDD workflow defined there.

---

## Workflow

### Step 1 — Locate the PRD and task list

Scan `.agents/prds/` for subdirectories.

- **One PRD found**: use it automatically.
- **Multiple PRDs found**: list them and ask the user which to work from.
- **No PRDs found**: stop. Tell the user no PRDs exist yet and suggest running `write-prd` followed by `breakdown-prd`.

Once the PRD is identified, list all `NN-*.md` files in the PRD folder — these are the task files. Note the PRD slug (the folder name under `.agents/prds/`) — it is used in commit messages.

- **No task files found**: stop. Tell the user no tasks exist yet and suggest running `breakdown-prd`.

---

### Step 2 — Select the next task

Determine the current task using this priority order:

1. **Any task with status `in-progress`**: an earlier session was interrupted. Resume it.
2. **Lowest-numbered task with status `pending`**: this is the next task to implement.
3. **All tasks are `done`**: proceed to **Step 6 — E2E final gate** (if applicable), otherwise print "All tasks complete." and stop.

Before starting, announce to the user:

```
Starting task NN: [task title]
[one-line description from task file]
```

Update the task file's status field from `pending` to `in-progress`.

---

### Step 3 — Implement using TDD

Follow the `tdd` skill's full workflow for this task:

1. **Plan** — read the task's description, acceptance criteria, and notes. Identify the behaviors to test and the public interfaces involved. If anything is ambiguous, ask the user before writing code.
2. **Tracer bullet** — write one test for the first behavior → make it pass with minimal code.
3. **Incremental loop** — for each remaining acceptance criterion: write test → make it pass.
4. **Refactor** — once all criteria are green, clean up. Run tests after each refactor step.

Stay within the scope of this task. Do not implement anything belonging to a later task, even if it seems convenient.

---

### Step 4 — Run precommit

When the TDD cycle is complete and all tests are green, run:

```
task precommit
```

**If it passes**: proceed to Step 5.

**If it fails**: fix the failures autonomously and re-run. Repeat up to **3 attempts** total.

- After each fix, re-run `task precommit`.
- If it passes within 3 attempts: proceed to Step 5, noting what was fixed.
- If it still fails after 3 attempts: stop, show the user the remaining failures, and ask how to proceed. Do not mark the task done.

---

### Step 5 — Auto-commit and continue

Once precommit passes:

1. Update the task file's status from `in-progress` to `done`.
2. Commit all changes using the following message format:

   ```
   {short summary of changes made}

   {optional: longer description explaining why — only include this paragraph when the changes are non-obvious or have potential hidden side effects}

   Addresses {prd-slug}-{NN}
   ```

   Where `{prd-slug}` is the PRD folder name (e.g. `user-auth`) and `{NN}` is the zero-padded task number (e.g. `01`, `02`). Omit the optional body paragraph entirely when the changes are self-explanatory.

3. Return to **Step 2** immediately and pick the next task. Do not pause for human input.

---

### Step 6 — E2E final gate

Run this step after all implementation tasks are `done`, but only if the PRD contained an `E2E Scenarios` section (i.e. task 01 was the "Write & verify E2E scenarios" task).

1. Run `task e2e`.
2. **If all pass**: report success to the user. The feature is complete.
3. **If any fail**: diagnose and fix (up to **3 attempts**), running `task e2e` after each fix.
   - If green within 3 attempts: commit the fix using the same commit message format (use `Addresses {prd-slug}-e2e` as the footer line), then report success.
   - If still failing after 3 attempts: stop. Surface the full test failure output to the user and ask for guidance. Do not mark anything further as done.

---

## Rules

- **One task at a time.** Never begin the next task until the current one is committed and marked done.
- **Stay in scope.** Do not implement behaviors belonging to future tasks.
- **Never skip precommit.** Every task must pass `task precommit` before being committed.
- **No human input between tasks.** Commit and continue automatically until all tasks are done or a failure requires human guidance.
- **Resume gracefully.** If a task is `in-progress` on startup, treat it as an interrupted session and pick up the TDD loop from context.
