# Implement ApiFeatureFlagProvider

## Status

`pending` <!-- pending | in-progress | done -->

## Description

Add a new `IFeatureFlagProvider` implementation in the frontend that fetches feature flags from the backend `GET /feature-flags` endpoint. The provider extends `BaseApi` so the base URL is handled automatically by `UrlMiddleware`.

## Acceptance Criteria

- [ ] `ApiFeatureFlagProvider` exists at `frontend/src/lib/services/FeatureFlag/ApiFeatureFlagProvider.ts`
- [ ] Implements `IFeatureFlagProvider` and extends `BaseApi`
- [ ] `getFeatureFlags()` calls `GET /feature-flags` and returns a `FeatureFlag[]`
- [ ] Returns an empty array (does not throw) when the request fails
- [ ] Unit tests exist covering: returns mapped flags on success, returns empty array on fetch failure

## Notes

- `IFeatureFlagProvider` interface is at `frontend/src/lib/services/FeatureFlag/IFeatureFlagProvider.ts`
- `BaseApi` is at `frontend/src/lib/api/baseApi.ts` — use `this.Get<FeatureFlag[]>('/feature-flags')`
- The existing `ConfigFeatureFlagProvider.ts` is a useful reference for the interface contract and error handling pattern
- Depends on task 04 (endpoint must exist for integration; mock in unit tests)
