# Add Dashboard Layout API

## Status

`done`

## Description

Implement `GET /api/dashboard/layout` and `PUT /api/dashboard/layout` endpoints. GET returns the current slot assignments for the active layout format; PUT fully replaces them. The active format is hardcoded as `"3x2"` for now.

## Acceptance Criteria

- [ ] `GET /api/dashboard/layout` returns `{ layoutFormat: "3x2", slots: [{ slotIndex: int, widgetType: string|null }] }` — absent DB rows are returned as `null` `widgetType` entries (all 6 slots always represented)
- [ ] `PUT /api/dashboard/layout` accepts the same shape and upserts: inserts new rows, updates existing, deletes rows not present in the payload
- [ ] Both endpoints are covered by integration tests (happy-path GET and PUT, plus a round-trip test)
- [ ] `dotnet build` and all backend tests pass

## Notes

Follow the existing controller/use-case pattern — see `BookmarkController.cs` and the logic layer for reference. Business logic lives in `backend/Logic/`. The PUT is a **full replace** for the given format (not a partial patch) — simplest correct implementation. Register the route under `[ApiController] [Route("api/dashboard")]`.
