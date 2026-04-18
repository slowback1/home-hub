# Add task new-migration to Backend Taskfile

## Status

`pending` <!-- pending | in-progress | done -->

## Description

Add a `new-migration` task to the backend `Taskfile.yml` that wraps `dotnet ef migrations add` with the correct `--project` and `--startup-project` flags. The migration name is passed as a `NAME` CLI variable (e.g. `task new-migration NAME=AddUserTable`).

## Acceptance Criteria

- [ ] `task new-migration NAME=Test` runs successfully and produces a new migration file under `backend/EntityFramework/Migrations/`
- [ ] Running `task new-migration` without `NAME=` produces a clear error (either a Taskfile `requires` guard or the `dotnet ef` error is surfaced)
- [ ] The task is documented with a `desc:` field visible in `task --list`

## Notes

- Backend Taskfile is at `backend/Taskfile.yml`. Check its structure before adding the task to ensure the new entry follows existing conventions (indentation, `vars:`, `cmds:` style).
- The underlying command to wrap:
  ```
  dotnet ef migrations add {{.NAME}} \
    --project EntityFramework \
    --startup-project WebAPI
  ```
- Run the command from the `backend/` directory (set `dir: .` or rely on the Taskfile's working directory).
- Use Taskfile's `requires: vars: [NAME]` to enforce that `NAME` is provided, so the task fails with a clear message rather than passing an empty string to `dotnet ef`.
