# Add `WheelApi` frontend client

## Status

`pending`

## Description

Add the typed frontend API client the Wheels page and widget use to talk to `WheelController`.

## Acceptance Criteria

- [ ] `frontend/src/lib/api/WheelApi.ts` exports a `Wheel` type (`id`, `name`, `items`, `createdAt`) and methods `getAll()`, `create(name, items)`, `update(id, name, items)`, `delete(id)`.
- [ ] `frontend/src/lib/api/WheelApi.spec.ts` covers each method (request shape + response mapping), mirroring `WalkSessionApi.spec.ts` / `ActivityApi.spec.ts`.
- [ ] `items` is carried as the newline-delimited string returned by the backend (parsing into lines is the page/widget's concern, via the shared spin util where relevant).
- [ ] Frontend unit tests pass.

## Notes

- Mirror `frontend/src/lib/api/WalkSessionApi.ts` (+ its `.spec.ts`) and `ActivityApi` for shape and error handling conventions.
- Depends on task 04 for the endpoint contract.
