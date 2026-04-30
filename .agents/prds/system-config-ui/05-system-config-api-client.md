# SystemConfigApi Client

## Status

`pending`

## Description

Add a TypeScript API client class for the system config endpoints. It wraps the two calls the admin page needs: fetch all entries and update a single entry.

## Acceptance Criteria

- [ ] `SystemConfigApi` class exists, extends `BaseApi`, and is covered by unit tests
- [ ] `getAll()` calls `GET /api/system-config` and returns `SystemConfig[]`
- [ ] `update(namespace, key, value)` calls `PUT /api/system-config/{namespace}/{key}` with `{ value }` as the JSON body and returns the updated `SystemConfig`
- [ ] A `SystemConfig` TypeScript type is defined matching the backend model (`id`, `namespace`, `key`, `value`, `type`, `isSecret`)
- [ ] Unit tests use the existing `getFetchMock` test helper and cover the happy path for both methods

## Notes

Follow the pattern in `frontend/src/lib/api/baseApi.ts`. Existing API clients (if any) in `frontend/src/lib/api/` are the reference for file structure and test conventions.

The `getFetchMock` helper is at `frontend/src/lib/testHelpers/getFetchMock.ts`.

Place the new file at `frontend/src/lib/api/SystemConfigApi.ts` (and its spec alongside it).
