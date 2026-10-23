# Kanboard integration

Shared tooling that lets the agent skills store their work in a self-hosted
[Kanboard](https://kanboard.org/) instance instead of in local files.

- **`kb.py`** — a dependency-free (Python stdlib only) CLI over Kanboard's
  JSON-RPC API. Run `./kb.py --help`, or see the module docstring for the full
  command list and the board conventions (project / swimlane / column / priority
  mappings). Drives the **chores** flow (`log-chore`, `chore-task`,
  `deferred-chore-task`) and the **feature pipeline** (`triage-backlog`,
  `refine-ticket`, `implement-ready`, `review-ready`, plus the interactive
  `ideate-feature` / `write-prd` / `breakdown-prd` / `tdd-task`).
- **`setup_board.py`** — idempotent one-time setup of the **Home Hub** board:
  adds the *Features* and *Chores* swimlanes, renames/inserts the pipeline
  columns, and orders them. Requires the API user to be a **manager**
  of the project (a plain member gets 403).
- **`../pipeline/run-pipeline.sh`** — hourly cron orchestrator that runs the
  four pipeline stages headlessly (`opencode run`), flock-locked, with logs in
  `../pipeline/logs/`.
- **`../kanboard.local.json`** — connection config holding the endpoint and API
  token. **Gitignored** (it has a secret), so it is not in a fresh clone.

## The pipeline board

Project **Home Hub**, swimlanes **Features** and **Chores**. Features columns:

```
Backlog -> Blocked -> To refine -> Review refinement -> Ready to implement ->
In progress -> Ready for review -> Needs manual review -> Done
```

| Column | Meaning |
|---|---|
| Backlog | vague idea entered by the human |
| Blocked | card waits on unfinished "is blocked by" dependencies; triage parks it here and promotes it to To refine once blockers reach Done |
| To refine | triaged/split stub, awaiting AI refinement |
| Review refinement | refined PRD + open questions as comments, awaiting **human** review |
| Ready to implement | human-approved PRDs; also where rejected work returns for rework |
| In progress | being implemented (PRD parent or child task) |
| Ready for review | implemented, awaiting AI code review |
| Needs manual review | AI review passed; a stated `Manual check:` criterion awaits a **human** playtest — only a human moves a card out (straight to Done, or back to Ready to implement with a failure comment) |
| Done | approved / finished |

The human's role: drop ideas into **Backlog**, then answer questions and move
approved cards from **Review refinement** to **Ready to implement**. Everything
else runs unattended (cron hourly, or invoke the stage skills manually).

Chores use the same lane columns but skip the refinement/review gates:
Backlog (open question) → Ready to implement → In progress → Done.

## First-time setup (after cloning)

1. Create `.agents/kanboard.local.json` (one level up from this folder):

   ```json
   {
     "endpoint": "http://HOST:PORT/jsonrpc.php",
     "auth": {
       "username": "your-kanboard-login",
       "token": "your-personal-api-token"
     },
     "tls": { "verify": true }
   }
   ```

   - **auth** — a **personal** API token (Kanboard → profile → API) with
     `username` set to that user's login. The user must be a **manager** of the
     Home Hub project (Project → Permissions and users) for `setup_board.py`
     and column/swimlane changes; a plain member can still read/move/comment.
   - **tls.verify** — set `false` only for an https endpoint with a self-signed cert.

2. Verify and set up the board:

   ```bash
   ./kb.py ping          # -> {"connected": true, "user": ..., "version": ...}
   ./setup_board.py      # idempotent: swimlanes + columns + ordering
   ./kb.py resolve       # prints resolved project / swimlane / column ids
   ```

3. Install the hourly cron (if not already present):

   ```
   17 * * * * <repo>/.agents/pipeline/run-pipeline.sh >> <repo>/.agents/pipeline/logs/cron.log 2>&1
   ```

If you rename the project, the swimlanes, or the columns in Kanboard, update the
constants at the top of `kb.py` to match.
