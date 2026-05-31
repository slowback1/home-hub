# Add GET /api/walk-sessions Endpoint

## Status

`done`

## Description

Add a list action to `WalkSessionController` that returns all walk sessions sorted by `StartedAt` descending. This is the prerequisite for every frontend piece of this feature.

## Acceptance Criteria

- [ ] `GET /api/walk-sessions` returns a `200 OK` with a JSON array of all `WalkSession` records
- [ ] Results are sorted by `StartedAt` descending (most recent first)
- [ ] Returns an empty array `[]` (not 404) when no sessions exist
- [ ] Integration test in `WalkSessionControllerTests` covers: multiple sessions returned in correct order, and empty-list case
- [ ] Existing `POST /api/walk-sessions` tests continue to pass

## Notes

- `ICrud<WalkSession>` does not have a `GetAllAsync` — use `QueryAsync(_ => true)` to retrieve all records, then sort with LINQ in the controller.
- Controller is at `backend/WebAPI/Controllers/WalkSessionController.cs`.
- Integration tests are at `backend/WebAPI.Integration.Tests/Controllers/WalkSessionControllerTests.cs`.
