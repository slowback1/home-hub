---
name: deferred-tdd-task
description: Schedule a /tdd-task run for a PRD at a future time using a local one-shot cron job. Use when the user wants to defer or schedule implementation work for later ("kick off the PRD tonight", "run tdd-task in 3 hours", "implement this overnight"), rather than starting it now.
---

# Deferred TDD Task

Schedule a future, unattended `/tdd-task` run against a PRD. The user names a PRD
(or it's inferred) and a time ("in 3 hours", "tonight at 11pm"); this skill
schedules a one-shot local cron job whose prompt is `/tdd-task` plus the
unattended-run guidance, firing at the resolved time.

This is a scheduling wrapper around the `tdd-task` skill — it does **not** do the
implementation itself. When the job fires, `/tdd-task` runs and works the PRD's
task list to completion using the red-green-refactor loop.

## Workflow

### Step 1 — Resolve the PRD

PRDs live in the Kanboard **Features swimlane**; reach them through
`../../kanboard/kb.py`. Determine which PRD to run:

- If the user named one, use its task id.
- Otherwise `.agents/kanboard/kb.py prd list`. If exactly one PRD has unfinished
  children (`children > 0` and not all Done), use it. If several do, **ask** the
  user which one.
- Confirm the PRD has a breakdown with `.agents/kanboard/kb.py task list <parent-id>`
  — these are the child tasks `tdd-task` consumes. If it has none, say so and
  suggest `breakdown-prd` first.

### Step 2 — Resolve the fire time

1. **Re-fetch the current time** with `date` — do not trust a remembered clock.
2. Resolve the user's relative or absolute phrase against it ("in 3 hours",
   "tonight at 11pm") to a concrete local time.
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
- `prompt`: `/tdd-task` followed by the unattended-run guidance block below.

### Step 4 — Confirm back

Echo to the user:
- The PRD that will run.
- The resolved local fire time **and** the exact cron expression.
- The session-must-stay-open caveat.

## Unattended-run guidance (embed in the scheduled prompt)

Append this to the `/tdd-task` prompt so the deferred run behaves correctly with no
human present:

```
/tdd-task

Unattended overnight run — no human will be present. Apply this guidance:
- Gate every task with the repo's checks, detected in order (see the
  implement-ready skill's "Project gate"): `task precommit` if a Taskfile
  defines it, else `dotnet build` + `dotnet test` for a .sln, else `npm test`,
  else `pytest`; if the repo defines no gate yet, syntax-check every changed
  file and note that in the final summary.
- For inherently visual or UI-side changes (Svelte rendering, styling, asset
  changes), a green build is the gate. Flag each such change in the final
  summary as needing a manual check in the browser/app.
- Do NOT push — this repo has no git remote. Commit locally only, and park each
  finished child task in Ready for review (`.agents/kanboard/kb.py task review
  <id>`); move the PRD itself to Ready for review (`feature review <id>`) when
  all its children are Done or awaiting review. The `review-ready` stage approves
  parked work to Done.
- Work tasks one at a time to completion; do not stop for input between tasks.
- In the final summary, list completed tasks and call out anything left needing
  manual verification.
```

## Notes

- This repo defines `task precommit` (`Taskfile.yml`): it runs the backend,
  frontend, and gpu-service checks.
- Use `CronList` to see scheduled jobs and `CronDelete <id>` to cancel one.
