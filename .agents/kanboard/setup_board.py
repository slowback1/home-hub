#!/usr/bin/env python3
"""setup_board.py — idempotent one-time setup of the "Home Hub" board.

Adds the "Features" and "Chores" swimlanes, renames the stock columns and
inserts the pipeline columns, then orders the columns:

    Backlog -> To refine -> Review refinement -> Ready to implement ->
    In progress -> Ready for review -> Done

Safe to re-run: existing swimlane/column names are left alone. Requires the
API user to be a MANAGER of the project (member role gets 403).

Usage: ./setup_board.py [--dry-run]
"""
import argparse
import os
import sys

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import kb  # noqa: E402


def try_rpc(label, method, **params):
    """Call rpc(); return (ok, result_or_error). rpc() sys.exits on failure."""
    try:
        return True, kb.rpc(method, **params)
    except SystemExit as exc:
        print(f"  !! {label}: {exc}")
        return False, None


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--dry-run", action="store_true",
                    help="print what would change without calling write APIs")
    args = ap.parse_args()

    pid = kb.project_id()
    print(f"project: {kb.PROJECT_NAME} (id {pid})")

    # --- swimlanes ---------------------------------------------------------- #
    lanes = kb.rpc("getAllSwimlanes", project_id=pid) or []
    lane_names = [l["name"] for l in lanes]
    print(f"swimlanes: {lane_names}")

    if kb.FEATURE_SWIMLANE not in lane_names:
        if not args.dry_run:
            ok, _ = try_rpc(f"add swimlane {kb.FEATURE_SWIMLANE}",
                            "addSwimlane", project_id=pid, name=kb.FEATURE_SWIMLANE)
            if not ok:
                return 1
            print(f"  added swimlane {kb.FEATURE_SWIMLANE}")
        else:
            print(f"  would add swimlane {kb.FEATURE_SWIMLANE}")

    if kb.CHORE_SWIMLANE not in lane_names:
        if not args.dry_run:
            ok, _ = try_rpc(f"add swimlane {kb.CHORE_SWIMLANE}",
                            "addSwimlane", project_id=pid, name=kb.CHORE_SWIMLANE)
            if not ok:
                return 1
            print(f"  added swimlane {kb.CHORE_SWIMLANE}")
        else:
            print(f"  would add swimlane {kb.CHORE_SWIMLANE}")

    # --- columns ------------------------------------------------------------ #
    cols = kb.rpc("getColumns", project_id=pid) or []
    by_title = {c["title"]: int(c["id"]) for c in cols}
    print(f"columns: {[c['title'] for c in cols]}")

    renames = {"Ready": kb.COL_READY, "Work in progress": kb.COL_WIP}
    for old, new in renames.items():
        if old in by_title and new not in by_title:
            if not args.dry_run:
                ok, _ = try_rpc(f"rename column '{old}' -> '{new}'",
                                "updateColumn", column_id=by_title[old], title=new)
                if not ok:
                    return 1
            by_title[new] = by_title.pop(old)
            print(f"  {'would rename' if args.dry_run else 'renamed'} "
                  f"'{old}' -> '{new}'")

    for title in kb.PIPELINE_COLUMNS:
        if title not in by_title:
            if not args.dry_run:
                ok, cid = try_rpc(f"add column '{title}'",
                                  "addColumn", project_id=pid, title=title)
                if not ok:
                    return 1
                by_title[title] = int(cid)
            else:
                by_title[title] = 0  # placeholder so ordering can be previewed
            print(f"  {'would add' if args.dry_run else 'added'} column '{title}'")

    # --- order columns into the pipeline sequence ---------------------------- #
    for pos, title in enumerate(kb.PIPELINE_COLUMNS, start=1):
        cid = by_title.get(title)
        if cid is None:
            print(f"  !! column '{title}' missing — cannot order")
            return 1
        if not args.dry_run:
            ok, _ = try_rpc(f"order '{title}' at position {pos}",
                            "changeColumnPosition", project_id=pid,
                            column_id=cid, position=pos)
            if not ok:
                return 1
    print(f"  {'would order' if args.dry_run else 'ordered'} columns: "
          + " -> ".join(kb.PIPELINE_COLUMNS))

    print("done." if not args.dry_run else "dry run complete.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
