---
name: deferred-chore-task
description: Schedule a /chore-task run at a future time using a local one-shot cron job. Use when the user wants to defer or schedule working off the captured chores for later ("clear the chores tonight", "run chore-task in 3 hours", "knock out the chores after my limit resets"), rather than starting now.
---

# Deferred Chore Task

Schedule a future, unattended `/chore-task` run against the captured chore queue.
The user names a time ("in 3 hours", "tonight at 11pm", "20 minutes after my
limit resets"); this skill schedules a one-shot local cron job whose prompt is
`/chore-task` plus the unattended-run guidance, firing at the resolved time.

This is a scheduling wrapper around the `chore-task` skill — it does **not** do
the work itself. When the job fires, `/chore-task` runs and works the open chores
one at a time to completion (highest priority first), each with its own TDD loop,
commit, and move to Done.

## Workflow

### Step 1 — Confirm there are chores to work

Chores live in the Kanboard **Chores swimlane**; list the workable ones with the
shared helper:

```bash
.agents/kanboard/kb.py chore list
```

This returns open chores (everything except Done) as JSON. Only **Ready to
implement** and **In progress** chores are workable unattended — **Backlog**
chores have an unanswered open question and `/chore-task` will skip them.

- If there are workable (Ready to implement / In progress) chores, note how many
  will be in scope for the run.
- If there are **none** — the lane is empty, or every open chore is stuck in
  Backlog on an open question — stop and tell the user there is nothing to
  schedule (list any Backlog chores and their questions). Suggest `log-chore` to
  capture some, or answer the open questions and move them to Ready to implement.

No PRD or task-list breakdown is required — chores are standalone. Do not block on
one.

### Step 2 — Resolve the fire time

1. **Re-fetch the current time** with `date` — do not trust a remembered clock.
2. Resolve the user's relative or absolute phrase against it ("in 3 hours",
   "tonight at 11pm", "after my session limit resets") to a concrete local time.
   If the user references a session-limit reset without a concrete time, ask when
   that reset is (or have them confirm the target time) — don't guess.
3. Build a one-shot 5-field cron expression pinned to that minute/hour/day-of-month/
   month: `"M H DoM Mon *"`. Leave day-of-week as `*`.
4. Avoid landing on minute `:00`/`:30` unless the user named that exact time — nudge
   a few minutes off so the fleet doesn't synchronize.

### Step 3 — Schedule the job

Call **`CronCreate`** (the local, in-session scheduler) — **not** the cloud
`/schedule` skill, which needs a GitHub remote this repo does not have.

- `cron`: the one-shot expression from Step 2.
- `recurring: false` — fire once, then auto-delete.
- `durable`: `true` if the run must survive Claude restarting (persists to
  `.claude/scheduled_tasks.json`); omit/`false` for session-only. **Either way, the
  job only fires while a Claude session is alive and idle at the REPL** — if the
  machine is off or no session is running at fire time, it will not run. With
  `durable: true` it persists across restarts but still needs a live session to
  fire. Tell the user this caveat explicitly when scheduling.
- `prompt`: `/chore-task` followed by the unattended-run guidance block below.

### Step 4 — Confirm back

Echo to the user:
- How many workable chores are queued to be worked (and that new chores logged
  before the fire time will be picked up too, since `/chore-task` re-queries the
  Kanboard Chores lane when it fires).
- The resolved local fire time **and** the exact cron expression.
- The session-must-stay-open caveat.

## Unattended-run guidance (embed in the scheduled prompt)

Append this to the `/chore-task` prompt so the deferred run behaves correctly with
no human present:

```
/chore-task

Unattended run — no human will be present. Apply this guidance:
- Work every open chore, one at a time, highest priority first, to completion —
  do not stop for input between chores.
- Gate every chore with the repo's checks, detected in order (see the
  implement-ready skill's "Project gate"): `task precommit` if a Taskfile
  defines it, else `dotnet build` + `dotnet test` for a .sln, else `npm test`,
  else `pytest`; if the repo defines no gate yet, syntax-check every changed
  file and note that in the final summary.
- For inherently visual or UI-side changes (Svelte rendering, styling, asset
  changes), do NOT fabricate a test — a green build is the gate. Flag each such
  chore in the final summary as needing a manual check in the browser/app.
- If a chore turns out to be larger than a small single change, skip it, leave it
  open, note it in the summary, and move on — do not let it grow into feature work.
- If a chore is genuinely ambiguous in a blocking way, skip it and note why rather
  than guessing; a human will resolve it later.
- Do NOT push — this repo has no git remote. Commit locally only, and move each
  finished chore to the Done column (`.agents/kanboard/kb.py chore done <id>`).
- In the final summary, list completed chores, any skipped/left-open chores with
  the reason, and anything needing manual verification.
```

## Notes

- Sibling skill: `deferred-tdd-task` (same mechanism, but targets a PRD's task
  list via `/tdd-task`). This one targets the chore queue via `/chore-task`.
- This repo defines `task precommit` (`Taskfile.yml`): it runs the backend,
  frontend, and gpu-service checks.
- Use `CronList` to see scheduled jobs and `CronDelete <id>` to cancel one.
- Because `/chore-task` re-queries the Kanboard Chores lane when it fires, you can
  keep logging chores right up until the fire time and they'll be included.
