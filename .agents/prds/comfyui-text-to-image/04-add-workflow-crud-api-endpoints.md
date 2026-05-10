# Add Workflow CRUD API Endpoints

## Status

`done`

## Description

Add a `ComfyUiController` with endpoints to list, create, and delete workflows. The list endpoint returns only `Id` and `Name` (never the full JSON), create accepts name and JSON, and delete removes by ID.

## Acceptance Criteria

- [ ] `GET /api/comfyui/workflows` returns a list of `{ id, name }` objects for all stored workflows
- [ ] `POST /api/comfyui/workflows` accepts `{ name, workflowJson }` and persists a new `ComfyUiWorkflow`, returning the created record
- [ ] `DELETE /api/comfyui/workflows/{id}` removes the workflow; returns 404 if not found
- [ ] Controller is registered and routes are reachable
- [ ] Tests cover all three endpoints including the 404 case for delete

## Notes

List response intentionally omits `WorkflowJson` to keep payloads small — the full JSON is only needed server-side at generation time.

Follow the existing controller patterns in `backend/WebAPI/Controllers/` for request/response DTO shape and error handling conventions.
