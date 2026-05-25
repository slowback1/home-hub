# Backend CRUD API + Test Helper Endpoint

## Status

`done`

## Description

Implement the `BookmarkController` with all five routes and add a `DELETE /api/test/bookmarks` test-helper endpoint used by the E2E Before/After hooks. Include integration tests for all routes.

## Acceptance Criteria

- [ ] `GET /api/bookmarks` returns all bookmarks as a JSON array (unsorted — client sorts alphabetically)
- [ ] `POST /api/bookmarks` creates a bookmark; returns 400 if `Url` is missing or blank
- [ ] `PUT /api/bookmarks/{id}` updates `Name`, `Url`, `Description`; returns 404 if not found
- [ ] `DELETE /api/bookmarks/{id}` deletes the bookmark; returns 404 if not found, 204 on success
- [ ] `PATCH /api/bookmarks/{id}/star` toggles `Starred` and returns the updated bookmark; returns 404 if not found
- [ ] `DELETE /api/test/bookmarks` deletes all bookmarks (test helper, mirroring `/api/test/tasks`)
- [ ] Integration tests cover: list, create, update, delete, star toggle, 404 cases, and the test-helper endpoint
- [ ] All integration tests pass

## Notes

- Follow the existing controller pattern — see `ChoreTaskController.cs` and `WebAPI.Integration.Tests/Controllers/ChoreTaskControllerTests.cs`
- The star toggle follows the pattern of `POST /api/tasks/{id}/complete`: dedicated sub-resource endpoint, returns the updated entity
- The test-helper endpoint lives in `TestHelperController.cs` (already exists — add a new action there)
- `Url` is the only required field; `Name` and `Description` may be blank (frontend fills in name default before saving)
- Route: `[Route("api/bookmarks")]`
