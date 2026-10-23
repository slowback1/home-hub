#!/usr/bin/env python3
"""kb.py — thin CLI over the Kanboard JSON-RPC API.

Shared helper for the Kanboard-backed skills: the chore flow and the
unattended feature pipeline (Backlog -> Blocked -> To refine -> Review
refinement -> Ready to implement -> In progress -> Ready for review ->
Needs manual review -> Done). Reads connection secrets from
../kanboard.local.json (gitignored). This file holds NO secrets, safe to
commit.

Board conventions (edit here if you rename things in Kanboard):
  - Project .............. "Home Hub"
  - Chores swimlane ...... "Chores"
  - Features swimlane .... "Features"
  - Columns .............. Backlog -> Blocked -> To refine ->
                           Review refinement -> Ready to implement ->
                           In progress -> Ready for review ->
                           Needs manual review -> Done
        Backlog ........... vague idea, entered by the human
        Blocked ........... card waits on unfinished "is blocked by"
                            dependencies; triage parks it here and an
                            unblock scan promotes it once blockers are Done
        To refine ......... triaged/split stub, awaiting AI refinement
        Review refinement . refined PRD + open questions as comments,
                            awaiting human review
        Ready to implement  human-approved PRDs; also where rejected work
                            is sent back for rework
        In progress ....... being implemented (PRD parent or child task)
        Ready for review .. implemented, awaiting AI code review
        Needs manual review AI review passed; a stated Manual check
                            criterion awaits a human playtest (only a human
                            moves a card out of here — to Done or back for
                            rework)
        Done .............. approved / finished
  - Priority (native) .... low=1  medium=2  high=3   (0 = unset)
  - Type ................. a tag: bug | chore | enhancement
  - Breakdown child ...... a Features task titled "{parentId}-{NN} name",
                           tagged "task", linked "is a child of" its PRD.
  - Triage split ......... stubs linked "relates to" the superseded original
                           (the original moves to Done).

The pipeline (Features swimlane), one stage per unattended run:
   triage-backlog  -> Backlog cards split/queued   (stub create --column)
   refine-ticket   -> PRD body + question comments (move to Review refinement)
   implement-ready -> task add + TDD               (task start / review)
   review-ready    -> approve task done | reject   (move back)

Usage:
  kb.py ping | resolve
  kb.py stage list "<column>" [--lane Features|Chores]
  kb.py move <id> "<column>"
  kb.py comment add <id> --text-file PATH | --text T
  kb.py comment list <id>
   kb.py chore list [--all] | show <id> | create ... | start <id> | done <id>
   kb.py link add <id> --blocked-by <other-id>   # declare a dependency
   kb.py link list <id>                          # links + opposite card columns
   kb.py stub create --title T [--column Backlog|To refine] [--priority ...] [--desc-file PATH]
  kb.py stub link <id> --parent <id>
  kb.py prd list [--all] | show <id>
  kb.py prd write <id> --desc-file PATH        # set description, move to Ready to implement
  kb.py prd create --title T --desc-file PATH
  kb.py feature start <id> | review <id> | done <id>
  kb.py task add <parent> --name N [--seq NN] [--column ...] [--priority ...] [--desc-file PATH]
  kb.py task list <parent> | show <id>
  kb.py task start <id> | review <id> | done <id>
"""
import argparse
import base64
import json
import os
import re
import ssl
import sys
import urllib.request

HERE = os.path.dirname(os.path.abspath(__file__))
CONFIG_PATH = os.path.normpath(os.path.join(HERE, "..", "kanboard.local.json"))

PROJECT_NAME = "Home Hub"
CHORE_SWIMLANE = "Chores"
FEATURE_SWIMLANE = "Features"
COL_BACKLOG = "Backlog"
COL_BLOCKED = "Blocked"
COL_TO_REFINE = "To refine"
COL_REVIEW_REFINE = "Review refinement"
COL_READY = "Ready to implement"
COL_WIP = "In progress"
COL_READY_REVIEW = "Ready for review"
COL_MANUAL_REVIEW = "Needs manual review"
COL_DONE = "Done"

PIPELINE_COLUMNS = [COL_BACKLOG, COL_BLOCKED, COL_TO_REFINE, COL_REVIEW_REFINE,
                    COL_READY, COL_WIP, COL_READY_REVIEW, COL_MANUAL_REVIEW,
                    COL_DONE]

PRIORITY_TO_INT = {"low": 1, "medium": 2, "high": 3}
INT_TO_PRIORITY = {0: "none", 1: "low", 2: "medium", 3: "high"}
TYPE_TAGS = {"bug", "chore", "enhancement"}

# Marks a breakdown child task (vs. a PRD/stub) in the Features lane.
TASK_TAG = "task"
# "is a child of" link type. getAllLinks is admin-only (403 for personal
# tokens), so this id is pinned empirically rather than looked up; task add
# verifies the label after linking and warns if this id ever stops meaning
# "is a child of".
CHILD_OF_LINK_ID = 6
CHILD_OF_LABEL = "is a child of"
PARENT_OF_LABEL = "is a parent of"
# Used by triage to link split stubs to the superseded original card.
RELATED_LINK_ID = 1
RELATED_LABEL = "relates to"
# "is blocked by" / "blocks" link types, pinned empirically (same pattern as
# CHILD_OF_LINK_ID): verified by creating a throwaway link with link_id 3 and
# reading the labels back via getAllTaskLinks. link add verifies the label
# after creation and warns if this id ever stops meaning "is blocked by".
BLOCKED_BY_LINK_ID = 3
BLOCKED_BY_LABEL = "is blocked by"
BLOCKS_LINK_ID = 2
BLOCKS_LABEL = "blocks"


# --------------------------------------------------------------------------- #
# JSON-RPC plumbing
# --------------------------------------------------------------------------- #
def _load_config():
    try:
        with open(CONFIG_PATH) as fh:
            cfg = json.load(fh)
    except FileNotFoundError:
        sys.exit(f"kb: config not found at {CONFIG_PATH}")
    except json.JSONDecodeError as exc:
        sys.exit(f"kb: config is not valid JSON ({exc})")
    if not cfg.get("endpoint"):
        sys.exit("kb: config missing 'endpoint'")
    # Only real value fields count as placeholders — ignore prose in _readme.
    values = [cfg.get("endpoint", ""),
              cfg.get("auth", {}).get("username", ""),
              cfg.get("auth", {}).get("token", "")]
    if any("FILL_ME" in str(v) for v in values):
        sys.exit("kb: config still has FILL_ME placeholders — fill it in first")
    return cfg


_CFG = None
_CACHE = {}


def rpc(method, **params):
    global _CFG
    if _CFG is None:
        _CFG = _load_config()
    payload = json.dumps(
        {"jsonrpc": "2.0", "id": 1, "method": method, "params": params}
    ).encode()
    auth = _CFG["auth"]
    token = base64.b64encode(
        f"{auth['username']}:{auth['token']}".encode()
    ).decode()
    req = urllib.request.Request(
        _CFG["endpoint"],
        data=payload,
        headers={
            "Content-Type": "application/json",
            "Authorization": f"Basic {token}",
        },
    )
    ctx = None
    if _CFG["endpoint"].startswith("https") and not _CFG.get("tls", {}).get(
        "verify", True
    ):
        ctx = ssl.create_default_context()
        ctx.check_hostname = False
        ctx.verify_mode = ssl.CERT_NONE
    try:
        with urllib.request.urlopen(req, timeout=20, context=ctx) as resp:
            body = json.load(resp)
    except urllib.error.HTTPError as exc:
        sys.exit(f"kb: HTTP {exc.code} calling {method} — check endpoint/auth")
    except urllib.error.URLError as exc:
        sys.exit(f"kb: cannot reach Kanboard ({exc.reason}) — is it up / on the LAN?")
    if body.get("error"):
        err = body["error"]
        sys.exit(f"kb: API error on {method}: {err.get('message')} ({err.get('code')})")
    return body.get("result")


# --------------------------------------------------------------------------- #
# id resolution (cached per run)
# --------------------------------------------------------------------------- #
def project_id():
    if "pid" not in _CACHE:
        # getProjectByName is application-API only (403 for personal tokens);
        # getMyProjects lists what this user can actually see.
        projects = rpc("getMyProjects") or []
        proj = next((p for p in projects if p["name"] == PROJECT_NAME), None)
        if not proj:
            names = ", ".join(p["name"] for p in projects) or "(none)"
            sys.exit(f"kb: project '{PROJECT_NAME}' not visible to this user (have: {names})")
        _CACHE["pid"] = int(proj["id"])
    return _CACHE["pid"]


def swimlane_id(name):
    key = f"sw:{name}"
    if key not in _CACHE:
        lanes = rpc("getAllSwimlanes", project_id=project_id()) or []
        match = next((l for l in lanes if l["name"] == name), None)
        if not match:
            sys.exit(f"kb: swimlane '{name}' not found in project")
        _CACHE[key] = int(match["id"])
    return _CACHE[key]


def columns():
    if "cols" not in _CACHE:
        cols = rpc("getColumns", project_id=project_id()) or []
        _CACHE["cols"] = {c["title"]: int(c["id"]) for c in cols}
        _CACHE["cols_by_id"] = {int(c["id"]): c["title"] for c in cols}
    return _CACHE["cols"]


def column_id(title):
    cols = columns()
    if title not in cols:
        sys.exit(f"kb: column '{title}' not found (have: {', '.join(cols)})")
    return cols[title]


def column_name(cid):
    columns()
    return _CACHE["cols_by_id"].get(int(cid), f"col{cid}")


def me():
    if "me" not in _CACHE:
        _CACHE["me"] = rpc("getMe")
    return _CACHE["me"]


def task_url(task_id):
    # Web base is the endpoint with jsonrpc.php stripped off.
    base = _CFG["endpoint"].rsplit("/", 1)[0] if _CFG else ""
    return (f"{base}/?controller=TaskViewController&action=show"
            f"&task_id={int(task_id)}&project_id={project_id()}")


def task_type(task_id):
    tags = rpc("getTaskTags", task_id=task_id) or {}
    for name in tags.values():
        if name in TYPE_TAGS:
            return name
    return None


def feature_lane():
    return swimlane_id(FEATURE_SWIMLANE)


def _read_text(args):
    for file_attr, text_attr in (("desc_file", "desc"), ("text_file", "text")):
        if getattr(args, file_attr, None):
            with open(getattr(args, file_attr)) as fh:
                return fh.read()
        if getattr(args, text_attr, None):
            return getattr(args, text_attr)
    return ""


def parse_seq(title, parent_id):
    """Pull NN out of a "{parent_id}-{NN} name" child title; unparseable sorts last."""
    m = re.match(rf"^{int(parent_id)}-(\d+)", title or "")
    return int(m.group(1)) if m else 10**9


def _do_start(task_id):
    """Assign the task to me and move it to In progress."""
    rpc("updateTask", id=int(task_id), owner_id=int(me()["id"]))
    _move(task_id, COL_WIP)
    return me()


def _feature_row(task):
    return {
        "id": int(task["id"]),
        "title": task["title"],
        "column": column_name(task["column_id"]),
        "priority": INT_TO_PRIORITY.get(int(task.get("priority", 0)), "none"),
        "priority_int": int(task.get("priority", 0)),
        "created": int(task.get("date_creation", 0)),
        "url": task_url(int(task["id"])),
    }


def _child_row(task, parent_id):
    row = _feature_row(task)
    row["seq"] = parse_seq(task["title"], parent_id)
    row["done"] = row["column"] == COL_DONE
    return row


def _links_of(task_id):
    return rpc("getAllTaskLinks", task_id=int(task_id)) or []


def _children_of(parent_id):
    """Ordered breakdown children of a PRD task (via the 'is a parent of' links)."""
    rows = []
    for link in _links_of(parent_id):
        if link.get("label") != PARENT_OF_LABEL:
            continue
        child = rpc("getTask", task_id=int(link["task_id"]))
        if child:
            rows.append(_child_row(child, parent_id))
    rows.sort(key=lambda r: r["seq"])
    return rows


# --------------------------------------------------------------------------- #
# commands
# --------------------------------------------------------------------------- #
def cmd_ping(_):
    print(json.dumps({"connected": True, "user": me().get("username"),
                      "name": me().get("name"), "version": rpc("getVersion")}))


def cmd_resolve(_):
    out = {"project_id": project_id(),
           "chore_swimlane_id": swimlane_id(CHORE_SWIMLANE),
           "feature_swimlane_id": feature_lane(),
           "child_of_link_id": CHILD_OF_LINK_ID,
           "columns": columns()}
    missing = [c for c in PIPELINE_COLUMNS if c not in out["columns"]]
    if missing:
        out["missing_pipeline_columns"] = missing
    print(json.dumps(out, indent=2))


def cmd_stage_list(args):
    lane = swimlane_id(args.lane)
    cid = column_id(args.column)
    tasks = rpc("getAllTasks", project_id=project_id(), status_id=1) or []
    rows = []
    for task in tasks:
        if int(task["swimlane_id"]) != lane or int(task["column_id"]) != cid:
            continue
        row = _feature_row(task)
        links = _links_of(int(task["id"]))
        row["children"] = sum(1 for l in links if l.get("label") == PARENT_OF_LABEL)
        row["is_breakdown_child"] = any(
            l.get("label") == CHILD_OF_LABEL for l in links)
        rows.append(row)
    rows.sort(key=lambda r: (-r["priority_int"], r["created"]))
    print(json.dumps(rows, indent=2))


def cmd_move(args):
    task = _move(args.id, args.column)
    print(json.dumps({"moved": int(args.id), "from": column_name(task["column_id"]),
                      "to": args.column, "url": task_url(args.id)}))


def cmd_comment_add(args):
    content = _read_text(args)
    if not content.strip():
        sys.exit("kb: comment add needs --text or --text-file")
    cid = rpc("createComment", task_id=int(args.id), user_id=int(me()["id"]),
              content=content)
    if not cid:
        sys.exit("kb: createComment returned no id")
    print(json.dumps({"comment": int(cid), "task": int(args.id),
                      "author": me().get("username"), "url": task_url(args.id)}))


def cmd_comment_list(args):
    comments = rpc("getAllComments", task_id=int(args.id)) or []
    my_name = me().get("username")
    rows = [{"id": int(c["id"]),
             "author": c.get("username"),
             "mine": c.get("username") == my_name,
             "created": int(c.get("date_creation", 0)),
             "content": c.get("comment", "")}
            for c in comments]
    rows.sort(key=lambda r: r["created"])
    print(json.dumps(rows, indent=2))


# --- chores ----------------------------------------------------------------- #
def _chore_row(task, with_type=True):
    return {
        "id": int(task["id"]),
        "title": task["title"],
        "column": column_name(task["column_id"]),
        "priority": INT_TO_PRIORITY.get(int(task.get("priority", 0)), "none"),
        "priority_int": int(task.get("priority", 0)),
        "type": task_type(int(task["id"])) if with_type else None,
        "created": int(task.get("date_creation", 0)),
        "url": task_url(int(task["id"])),
    }


def cmd_chore_list(args):
    lane = swimlane_id(CHORE_SWIMLANE)
    tasks = rpc("getAllTasks", project_id=project_id(), status_id=1) or []
    rows = [_chore_row(t) for t in tasks if int(t["swimlane_id"]) == lane]
    if not args.all:
        rows = [r for r in rows if r["column"] != COL_DONE]
    order = {COL_WIP: 0, COL_READY: 1, COL_BACKLOG: 2, COL_DONE: 3}
    rows.sort(key=lambda r: (order.get(r["column"], 9),
                             -r["priority_int"], r["created"]))
    print(json.dumps(rows, indent=2))


def cmd_chore_show(args):
    task = rpc("getTask", task_id=args.id)
    if not task:
        sys.exit(f"kb: task {args.id} not found")
    row = _chore_row(task)
    row["description"] = task.get("description", "")
    print(json.dumps(row, indent=2))


def cmd_chore_create(args):
    col = COL_BACKLOG if args.column == COL_BACKLOG else COL_READY
    tid = rpc(
        "createTask",
        title=args.title,
        project_id=project_id(),
        swimlane_id=swimlane_id(CHORE_SWIMLANE),
        column_id=column_id(col),
        priority=PRIORITY_TO_INT[args.priority],
        tags=[args.type],
        description=_read_text(args),
    )
    if not tid:
        sys.exit("kb: createTask returned no id")
    print(json.dumps({"created": int(tid), "column": col,
                      "priority": args.priority, "type": args.type,
                      "url": task_url(int(tid))}))


def _move(task_id, to_column):
    task = rpc("getTask", task_id=task_id)
    if not task:
        sys.exit(f"kb: task {task_id} not found")
    rpc("moveTaskPosition", project_id=project_id(), task_id=int(task_id),
        column_id=column_id(to_column), position=1,
        swimlane_id=int(task["swimlane_id"]))
    return task


def cmd_chore_start(args):
    rpc("updateTask", id=int(args.id), owner_id=int(me()["id"]))
    _move(args.id, COL_WIP)
    print(json.dumps({"started": int(args.id), "column": COL_WIP,
                      "owner": me().get("username"), "url": task_url(args.id)}))


def cmd_chore_done(args):
    _move(args.id, COL_DONE)
    print(json.dumps({"done": int(args.id), "column": COL_DONE,
                      "url": task_url(args.id)}))


# --- features: stubs, PRDs, breakdown children ------------------------------ #
def cmd_stub_create(args):
    col = args.column or COL_BACKLOG
    tid = rpc(
        "createTask",
        title=args.title,
        project_id=project_id(),
        swimlane_id=feature_lane(),
        column_id=column_id(col),
        priority=PRIORITY_TO_INT[args.priority],
        description=_read_text(args),
    )
    if not tid:
        sys.exit("kb: createTask returned no id")
    print(json.dumps({"created": int(tid), "column": col,
                      "priority": args.priority, "url": task_url(int(tid))}))


def cmd_stub_link(args):
    """Link a stub to the card it was split from ('relates to')."""
    rpc("createTaskLink", task_id=int(args.id), opposite_task_id=int(args.parent),
        link_id=RELATED_LINK_ID)
    links = _links_of(args.id)
    linked = any(l.get("label") == RELATED_LABEL and int(l["task_id"]) == int(args.parent)
                 for l in links)
    print(json.dumps({"task": int(args.id), "relates_to": int(args.parent),
                      "linked": linked}))


def cmd_link_add(args):
    """Declare a dependency: <id> is blocked by <other-id>."""
    rpc("createTaskLink", task_id=int(args.id), opposite_task_id=int(args.blocked_by),
        link_id=BLOCKED_BY_LINK_ID)
    links = _links_of(args.id)
    linked = any(l.get("label") == BLOCKED_BY_LABEL and int(l["task_id"]) == int(args.blocked_by)
                 for l in links)
    out = {"task": int(args.id), "blocked_by": int(args.blocked_by), "linked": linked}
    if not linked:
        out["warning"] = (f"link id {BLOCKED_BY_LINK_ID} did not resolve to "
                          f"'{BLOCKED_BY_LABEL}' — update BLOCKED_BY_LINK_ID in kb.py")
    print(json.dumps(out))


def cmd_link_list(args):
    """All links of a task with the opposite card's title and column."""
    rows = []
    for link in _links_of(args.id):
        opposite = rpc("getTask", task_id=int(link["task_id"]))
        rows.append({
            "label": link.get("label"),
            "task_id": int(link["task_id"]),
            "title": opposite["title"] if opposite else None,
            "column": column_name(opposite["column_id"]) if opposite else None,
        })
    print(json.dumps(rows, indent=2))


def cmd_prd_list(args):
    lane = feature_lane()
    tasks = rpc("getAllTasks", project_id=project_id(), status_id=1) or []
    rows = []
    for task in tasks:
        if int(task["swimlane_id"]) != lane:
            continue
        links = _links_of(int(task["id"]))
        if any(l.get("label") == CHILD_OF_LABEL for l in links):
            continue  # a breakdown child, not a PRD
        row = _feature_row(task)
        row["children"] = sum(1 for l in links if l.get("label") == PARENT_OF_LABEL)
        rows.append(row)
    if not args.all:
        rows = [r for r in rows if r["column"] != COL_DONE]
    order = {COL_WIP: 0, COL_READY: 1, COL_BACKLOG: 2, COL_DONE: 3}
    rows.sort(key=lambda r: (order.get(r["column"], 9),
                             -r["priority_int"], r["created"]))
    print(json.dumps(rows, indent=2))


def cmd_prd_show(args):
    task = rpc("getTask", task_id=args.id)
    if not task:
        sys.exit(f"kb: task {args.id} not found")
    row = _feature_row(task)
    row["description"] = task.get("description", "")
    row["children"] = _children_of(args.id)
    print(json.dumps(row, indent=2))


def cmd_prd_write(args):
    col = args.column or COL_READY
    rpc("updateTask", id=int(args.id), description=_read_text(args))
    _move(args.id, col)
    print(json.dumps({"updated": int(args.id), "column": col,
                      "url": task_url(args.id)}))


def cmd_prd_create(args):
    tid = rpc(
        "createTask",
        title=args.title,
        project_id=project_id(),
        swimlane_id=feature_lane(),
        column_id=column_id(COL_READY),
        description=_read_text(args),
    )
    if not tid:
        sys.exit("kb: createTask returned no id")
    print(json.dumps({"created": int(tid), "column": COL_READY,
                      "url": task_url(int(tid))}))


def cmd_task_add(args):
    parent = int(args.parent)
    if not rpc("getTask", task_id=parent):
        sys.exit(f"kb: parent task {parent} not found")
    existing = _children_of(parent)
    if args.seq is not None:
        seq = args.seq
    else:
        seqs = [r["seq"] for r in existing if r["seq"] < 10**9]
        seq = (max(seqs) + 1) if seqs else 1
    title = f"{parent}-{seq:02d} {args.name}"
    col = args.column or COL_READY
    cid = rpc(
        "createTask",
        title=title,
        project_id=project_id(),
        swimlane_id=feature_lane(),
        column_id=column_id(col),
        priority=PRIORITY_TO_INT[args.priority],
        tags=[TASK_TAG],
        description=_read_text(args),
    )
    if not cid:
        sys.exit("kb: createTask returned no id")
    rpc("createTaskLink", task_id=int(cid), opposite_task_id=parent,
        link_id=CHILD_OF_LINK_ID)
    links = _links_of(cid)
    linked = any(l.get("label") == CHILD_OF_LABEL and int(l["task_id"]) == parent
                 for l in links)
    out = {"created": int(cid), "title": title, "seq": seq, "parent": parent,
           "column": col, "linked": linked, "url": task_url(int(cid))}
    if not linked:
        out["warning"] = (f"link id {CHILD_OF_LINK_ID} did not resolve to "
                          f"'{CHILD_OF_LABEL}' — update CHILD_OF_LINK_ID in kb.py")
    print(json.dumps(out))


def cmd_task_list(args):
    print(json.dumps(_children_of(args.parent), indent=2))


def cmd_task_start(args):
    _do_start(args.id)
    print(json.dumps({"started": int(args.id), "column": COL_WIP,
                      "owner": me().get("username"), "url": task_url(args.id)}))


def cmd_task_review(args):
    _move(args.id, COL_READY_REVIEW)
    print(json.dumps({"review": int(args.id), "column": COL_READY_REVIEW,
                      "url": task_url(args.id)}))


def cmd_task_done(args):
    _move(args.id, COL_DONE)
    print(json.dumps({"done": int(args.id), "column": COL_DONE,
                      "url": task_url(args.id)}))


# --------------------------------------------------------------------------- #
def build_parser():
    p = argparse.ArgumentParser(prog="kb.py", description="Kanboard CLI helper")
    sub = p.add_subparsers(dest="cmd", required=True)

    sub.add_parser("ping").set_defaults(func=cmd_ping)
    sub.add_parser("resolve").set_defaults(func=cmd_resolve)

    stage = sub.add_parser("stage").add_subparsers(dest="stage_cmd", required=True)
    s_list = stage.add_parser("list", help="cards in one column of a swimlane")
    s_list.add_argument("column")
    s_list.add_argument("--lane", default=FEATURE_SWIMLANE,
                        choices=[FEATURE_SWIMLANE, CHORE_SWIMLANE])
    s_list.set_defaults(func=cmd_stage_list)

    mv = sub.add_parser("move", help="move a card to a named column")
    mv.add_argument("id", type=int)
    mv.add_argument("column")
    mv.set_defaults(func=cmd_move)

    comment = sub.add_parser("comment").add_subparsers(dest="comment_cmd", required=True)
    c_add = comment.add_parser("add")
    c_add.add_argument("id", type=int)
    c_add.add_argument("--text-file")
    c_add.add_argument("--text")
    c_add.set_defaults(func=cmd_comment_add)
    c_list = comment.add_parser("list")
    c_list.add_argument("id", type=int)
    c_list.set_defaults(func=cmd_comment_list)

    chore = sub.add_parser("chore").add_subparsers(dest="chore_cmd", required=True)

    c_list = chore.add_parser("list")
    c_list.add_argument("--all", action="store_true", help="include the Done column")
    c_list.set_defaults(func=cmd_chore_list)

    c_show = chore.add_parser("show")
    c_show.add_argument("id", type=int)
    c_show.set_defaults(func=cmd_chore_show)

    c_new = chore.add_parser("create")
    c_new.add_argument("--title", required=True)
    c_new.add_argument("--type", required=True, choices=sorted(TYPE_TAGS))
    c_new.add_argument("--priority", required=True, choices=["low", "medium", "high"])
    c_new.add_argument("--column", default=COL_READY, choices=[COL_BACKLOG, COL_READY],
                       help=f"{COL_READY} (actionable) or {COL_BACKLOG} (has open questions)")
    c_new.add_argument("--desc-file")
    c_new.add_argument("--desc")
    c_new.set_defaults(func=cmd_chore_create)

    c_start = chore.add_parser("start")
    c_start.add_argument("id", type=int)
    c_start.set_defaults(func=cmd_chore_start)

    c_done = chore.add_parser("done")
    c_done.add_argument("id", type=int)
    c_done.set_defaults(func=cmd_chore_done)

    # --- link (dependency declaration) --- #
    link = sub.add_parser("link", help="declare task dependencies")
    link_sub = link.add_subparsers(dest="link_cmd", required=True)
    l_add = link_sub.add_parser("add", help="link <id> 'is blocked by' <other-id>")
    l_add.add_argument("id", type=int)
    l_add.add_argument("--blocked-by", type=int, required=True)
    l_add.set_defaults(func=cmd_link_add)
    l_list = link_sub.add_parser("list", help="links of a task with opposite card status")
    l_list.add_argument("id", type=int)
    l_list.set_defaults(func=cmd_link_list)

    # --- stub (ideate-feature / triage splits) --- #
    stub = sub.add_parser("stub").add_subparsers(dest="stub_cmd", required=True)
    s_new = stub.add_parser("create")
    s_new.add_argument("--title", required=True)
    s_new.add_argument("--column", default=COL_BACKLOG,
                       choices=[COL_BACKLOG, COL_TO_REFINE])
    s_new.add_argument("--priority", default="medium",
                       choices=["low", "medium", "high"])
    s_new.add_argument("--desc-file")
    s_new.add_argument("--desc")
    s_new.set_defaults(func=cmd_stub_create)
    s_link = stub.add_parser("link", help="link a split stub to its superseded original")
    s_link.add_argument("id", type=int)
    s_link.add_argument("--parent", type=int, required=True)
    s_link.set_defaults(func=cmd_stub_link)

    # --- prd (write-prd / refine-ticket) --- #
    prd = sub.add_parser("prd").add_subparsers(dest="prd_cmd", required=True)

    p_list = prd.add_parser("list")
    p_list.add_argument("--all", action="store_true", help="include Done PRDs")
    p_list.set_defaults(func=cmd_prd_list)

    p_show = prd.add_parser("show")
    p_show.add_argument("id", type=int)
    p_show.set_defaults(func=cmd_prd_show)

    p_write = prd.add_parser("write", help="set description and move (default: Ready to implement)")
    p_write.add_argument("id", type=int)
    p_write.add_argument("--desc-file", required=True)
    p_write.add_argument("--column", choices=[COL_READY, COL_REVIEW_REFINE],
                         help="where to move the card after writing")
    p_write.set_defaults(func=cmd_prd_write)

    p_create = prd.add_parser("create", help="fresh PRD straight into Ready to implement")
    p_create.add_argument("--title", required=True)
    p_create.add_argument("--desc-file", required=True)
    p_create.set_defaults(func=cmd_prd_create)

    # --- feature (parent PRD column moves) --- #
    feat = sub.add_parser("feature").add_subparsers(dest="feature_cmd", required=True)
    f_start = feat.add_parser("start")
    f_start.add_argument("id", type=int)
    f_start.set_defaults(func=cmd_task_start)
    f_review = feat.add_parser("review", help="move the PRD to Ready for review")
    f_review.add_argument("id", type=int)
    f_review.set_defaults(func=cmd_task_review)
    f_done = feat.add_parser("done")
    f_done.add_argument("id", type=int)
    f_done.set_defaults(func=cmd_task_done)

    # --- task (breakdown children) --- #
    task = sub.add_parser("task").add_subparsers(dest="task_cmd", required=True)

    t_add = task.add_parser("add")
    t_add.add_argument("parent", type=int, help="parent PRD task id")
    t_add.add_argument("--name", required=True, help="task name (sans NN prefix)")
    t_add.add_argument("--seq", type=int, help="override auto NN sequence")
    t_add.add_argument("--column", default=COL_READY, choices=[COL_READY, COL_WIP])
    t_add.add_argument("--priority", default="medium",
                       choices=["low", "medium", "high"])
    t_add.add_argument("--desc-file")
    t_add.add_argument("--desc")
    t_add.set_defaults(func=cmd_task_add)

    t_list = task.add_parser("list")
    t_list.add_argument("parent", type=int, help="parent PRD task id")
    t_list.set_defaults(func=cmd_task_list)

    t_show = task.add_parser("show", help="a single task's description (parent or child)")
    t_show.add_argument("id", type=int)
    t_show.set_defaults(func=cmd_prd_show)

    t_start = task.add_parser("start")
    t_start.add_argument("id", type=int)
    t_start.set_defaults(func=cmd_task_start)

    t_review = task.add_parser("review", help="move the child to Ready for review")
    t_review.add_argument("id", type=int)
    t_review.set_defaults(func=cmd_task_review)

    t_done = task.add_parser("done")
    t_done.add_argument("id", type=int)
    t_done.set_defaults(func=cmd_task_done)

    return p


if __name__ == "__main__":
    args = build_parser().parse_args()
    args.func(args)
