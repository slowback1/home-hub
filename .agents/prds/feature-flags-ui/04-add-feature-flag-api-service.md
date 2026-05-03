# Add FeatureFlagApi Frontend Service

## Status

`pending` <!-- pending | in-progress | done -->

## Description

Add a typed frontend API service for feature flags, covering both the existing read endpoint and the new toggle endpoint. This is the only frontend networking layer change and keeps the page component free of raw fetch calls.

## Acceptance Criteria

- [ ] `frontend/src/lib/api/FeatureFlagApi.ts` exists and exports a `FeatureFlagApi` class extending `BaseApi`
- [ ] `getAll()` calls `GET /api/feature-flags` and returns a typed array of `{ name: string; isEnabled: boolean }`
- [ ] `toggle(name: string, isEnabled: boolean)` calls `PATCH /api/feature-flags/{name}` with `{"isEnabled": bool}` and returns the updated flag
- [ ] Unit tests cover `getAll()` and `toggle()` using the existing API test patterns

## Notes

- `BaseApi` is in `frontend/src/lib/api/` — extend it the same way other API services do (e.g. `SystemConfigApi`).
- The `FeatureFlag` type may already exist in `frontend/src/lib/services/FeatureFlag/`; reuse or import it rather than defining a duplicate.
- URL construction and base URL injection are handled by `UrlMiddleware` via `BaseApi` — no manual URL wiring needed.
