# Activity Entity and CRUD API

## Status

`done` <!-- pending | in-progress | done -->

## Description

Introduce the `Activity` entity as a first-class data entity wired into the existing ICRUD infrastructure. This covers the model definition, all three backend implementations (InMemory, FileData, EF), the EF migration, and the REST CRUD endpoints that the config page will call.

## Acceptance Criteria

- [ ] `Activity` model exists in `backend/Common/Models/` with fields: `Id` (Guid), `Name` (string), `Weight` (int, default 1)
- [ ] ICRUD implementation exists for all three backends: InMemory, FileData, and EntityFramework
- [ ] `Activity` is registered in `CrudFactoryConfigurator` so the correct implementation is injected based on config
- [ ] EF migration adds the `Activities` table with the correct schema
- [ ] `ActivityController` exposes the following endpoints:
  - `GET /api/activities` — returns all activities
  - `POST /api/activities` — creates a new activity
  - `PUT /api/activities/{id}` — updates name and/or weight
  - `DELETE /api/activities/{id}` — hard-deletes the activity
- [ ] All endpoints return appropriate HTTP status codes (404 on missing ID, 400 on invalid input)
- [ ] Controller is covered by unit or integration tests

## Notes

- Follow the existing pattern established by `SystemConfigController` and its provider/CRUD implementations
- Weight must be validated to be in range 1–5 at the API boundary
- Hard delete only — no soft delete, no `IsDeleted` flag
