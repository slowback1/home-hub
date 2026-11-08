#!/usr/bin/env bash
# run-pipeline.sh — hourly unattended Kanboard pipeline orchestrator.
#
# Runs the pipeline stages in order, each as its own headless opencode
# session: triage -> refine -> implement -> review, then the chore-flow
# stages: triage-chores -> implement-chores -> review-chores. Each stage is
# skipped when `kb.py pending <stage>` finds nothing to act on. A flock lock
# prevents overlapping runs. Logs land in .agents/pipeline/logs/ (gitignored).
set -uo pipefail

# cron's PATH is only /usr/bin:/bin — make opencode, dotnet 10, and node/npm resolvable
export PATH="/home/projector-room/.opencode/bin:/home/projector-room/.local/bin:/home/projector-room/.dotnet:/home/projector-room/.dotnet/tools:/home/projector-room/.nvm/versions/node/v22.17.1/bin:/usr/local/bin:$PATH"

REPO="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
DIR="$REPO/.agents/pipeline"
LOGS="$DIR/logs"
LOCK="$DIR/.pipeline.lock"
STAMP="$(date +%Y-%m-%dT%H-%M)"

mkdir -p "$LOGS" "$DIR/tmp"
exec 9>"$LOCK"
if ! flock -n 9; then
  echo "[$(date +%H:%M:%S)] previous pipeline run still active — skipping" >&2
  exit 0
fi

cd "$REPO"

GUIDE='Unattended pipeline run (cron) — no human present. Never ask questions; work autonomously until the stage is complete or every remaining item is blocked. Stay strictly inside this repo: never read, list, or stat paths outside the workspace (no ~ paths, no system directories) — an out-of-workspace permission prompt is auto-rejected and kills the session. If any permission is rejected, treat that information as unavailable and continue; never stop. Commit locally; triage and implement stages never push — only the review stage pushes to origin, and only after all its checks and gates pass. If a gate fails 3 times on an item, comment the failure on the Kanboard card and move on. If Kanboard is unreachable, report the error and stop this stage.'

KB="$REPO/.agents/kanboard/kb.py"

# If Kanboard is down every stage would just report the error and stop — don't
# spin up seven opencode sessions to learn that.
if ! "$KB" ping >/dev/null 2>"$DIR/tmp/ping.err"; then
  echo "[$(date +%H:%M:%S)] Kanboard unreachable — skipping run: $(cat "$DIR/tmp/ping.err")" >&2
  exit 0
fi

# Prints the number of cards the stage would act on, or nothing on failure.
# Checked right before each stage (not up front) because earlier stages feed
# later columns within the same run (triage -> To refine -> refine, ...).
pending_count() {
  "$KB" pending "$1" 2>/dev/null \
    | python3 -c 'import json,sys; print(json.load(sys.stdin)["count"])' 2>/dev/null
}

run_stage() {
  local name="$1" skill="$2"
  local log="$LOGS/$name-$STAMP.log"
  local count
  count="$(pending_count "$name")"
  if [[ "$count" == "0" ]]; then
    echo "[$(date +%H:%M:%S)] stage: $name — nothing to process, skipping"
    return
  elif [[ -z "$count" ]]; then
    # Fail open: a precheck bug must not silently stall the pipeline.
    echo "[$(date +%H:%M:%S)] stage: $name — precheck failed, running anyway"
  fi
  echo "[$(date +%H:%M:%S)] stage: $name (pending=${count:-?})"
  opencode run "Load and follow the $skill skill exactly.

$GUIDE" >"$log" 2>&1
  echo "[$(date +%H:%M:%S)] stage: $name rc=$? log=$log"
}

run_stage triage triage-backlog
run_stage refine refine-ticket
run_stage implement implement-ready
run_stage review review-ready
run_stage triage-chores triage-chores
run_stage implement-chores implement-chores
run_stage review-chores review-chores

find "$LOGS" -name '*.log' -mtime +14 -delete
