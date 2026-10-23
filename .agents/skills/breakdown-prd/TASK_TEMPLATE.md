# Task description body (Kanboard child-task description)

This is the Markdown that goes in a breakdown child task's **description**. The
task **title** is `{parentId}-{NN} name` and its **column** tracks status
(Ready → Work in progress → Done), so don't repeat title/status here. Author only
the body below.

## Description

_What needs to be done and why. One short paragraph._

## Acceptance Criteria

- [ ] _Concrete, verifiable condition derived from PRD goals/user stories_
- [ ] _Another verifiable condition_

For work that cannot be verified by unit tests (e.g. Godot-side rendering or
input), state an explicit `Manual check:` criterion — steps + expected
observations. The review stage reads it and parks the finished card in
"Needs manual review" for a human playtest instead of approving it to Done.

## Notes

_Relevant context for the implementer: file paths, API names, architectural
constraints, the PRD section this comes from, open questions that may affect this
task. Delete this section if there is nothing to add._
