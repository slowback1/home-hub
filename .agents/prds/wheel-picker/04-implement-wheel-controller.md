# Implement `WheelController` CRUD

## Status

`pending`

## Description

Expose the wheel CRUD API backed by `ICrud<Wheel>`, so the frontend can list, create, edit, and delete wheels.

## Acceptance Criteria

- [ ] `GET /api/wheels` returns all wheels.
- [ ] `POST /api/wheels` creates a wheel: rejects a missing/whitespace `Name` with `400`; assigns a new `Id` (GUID) and `CreatedAt`; persists `Items` as given (a wheel may be created with zero items).
- [ ] `PUT`/`PATCH /api/wheels/{id}` updates a wheel's `Name` and `Items`; returns `404` for an unknown id.
- [ ] `DELETE /api/wheels/{id}` removes a wheel; returns an appropriate status for an unknown id.
- [ ] Name uniqueness is **not** enforced; duplicate items within `Items` are allowed.
- [ ] Integration tests cover create (happy + empty-name `400`), list, update, and delete.

## Notes

- Mirror `backend/WebAPI/Controllers/WalkSessionController.cs` (route attribute, `ApplicationController` base, `Factory.GetCrud<Wheel>()`) and `backend/WebAPI.Integration.Tests/Controllers/WalkSessionControllerTests.cs`.
- Route: `[Route("api/wheels")]`.
- Validation mirrors the `Activity` controller: name required/trimmed, uniqueness not enforced (PRD Key Decisions).
- No spin endpoint — random selection happens in the frontend (PRD Non-Goals / Key Decisions).
