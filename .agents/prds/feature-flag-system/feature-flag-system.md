# PRD: Feature Flag System

## Status

`Draft` <!-- Draft | Review | Approved | Superseded -->

## Overview

A database-backed feature flag system that allows individual features to be toggled on or off at runtime without a code deploy. Flags are stored in PostgreSQL, read via a dedicated API endpoint, and consumed by the frontend at app load to conditionally show or hide UI and navigation elements.

## Problem Statement

As HomeHub grows with new features, the navigation and UI will accumulate items that are not always relevant or ready. Without a flag system, incomplete or disabled features must be removed from the codebase entirely or hidden with hardcoded compile-time conditions. A lightweight flag system decouples feature visibility from deployments and provides a clean foundation for future rollout capabilities.

## Goals

- Feature flags are stored in the database and readable at runtime via a REST endpoint
- The frontend consumes flags at app load and can conditionally render UI elements
- Adding a flag for a new feature requires a single EF migration — no code changes to the flag system itself
- The system is simple enough to manage via direct DB manipulation (no admin UI needed at this stage)

## Non-Goals

- Admin UI for managing flags
- Write endpoints (`POST`/`PATCH`/`DELETE`) for flags via the API
- Per-user or percentage-based flag targeting (deferred to a future stub)
- String or numeric flag variants — boolean on/off only
- Server-side caching of flag values

## User Stories / Use Cases

- **As a developer**, I want to add a feature flag for a new feature by creating a DB migration, so that the feature can be deployed but kept hidden until ready.
- **As a developer**, I want to enable or disable a feature flag by updating a row in the database, so that I can control feature visibility without a redeploy.
- **As the app**, I want to fetch all feature flags from the backend at startup, so that UI components can conditionally render based on flag state.

## E2E Scenarios

_N/A — this is an infrastructure feature with no directly user-visible UI. Feature-specific visibility scenarios belong to each feature's own PRD._

## Proposed Solution

Add a `FeatureFlags` table to PostgreSQL via an EF Core migration. Expose a `GET /feature-flags` endpoint that returns all flags. On the frontend, replace the config-based flag provider with a new API-backed provider that fetches from this endpoint at app load.

## Technical Approach

The backend follows the existing layered pattern: a new `EntityFrameworkFeatureFlagProvider` reads from the DB, a `GetFeatureFlagsUseCase` wraps it, and a `FeatureFlagController` exposes the endpoint. The frontend adds an `ApiFeatureFlagProvider` that extends `BaseApi` and implements the existing `IFeatureFlagProvider` interface, then wires it into `+layout.svelte`.

### Key Decisions

| Decision | Chosen Approach | Rationale |
|---|---|---|
| Flag granularity | Boolean only (`IsEnabled`) | No variant support needed; existing model is sufficient |
| Primary key | `Name` (string, natural key) | Flag names are unique by definition; no surrogate ID needed |
| Caching | None | Frontend fetches once at app load; DB hit per page load is negligible at this scale |
| Management interface | Direct DB manipulation | Flags change infrequently; no admin UI warranted at this stage |
| New flags convention | New EF migration per flag | Gives a full version-controlled audit trail of flag additions |
| Backend provider placement | `backend/EntityFramework/` | Keeps EF-specific code co-located with `AppDbContext`; `Logic` stays EF-agnostic |
| DI registration | Conditional on `CrudFactory:Implementation` config | Mirrors existing CRUD factory pattern; app remains runnable without a DB |
| Frontend provider | `ApiFeatureFlagProvider` extends `BaseApi` | Consistent with existing API layer; base URL handled by `UrlMiddleware` automatically |
| Fallback provider | `ConfigFeatureFlagProvider` retained | Useful for tests and offline dev when backend is unavailable |

### Dependencies

- `database-infrastructure` — PostgreSQL + EF Core already established (`AppDbContext`, migrations, auto-migration at startup)
- `IFeatureFlagProvider` interface — already exists in `Common/Interfaces/`
- `FeatureFlagService` (backend) — already exists in `Logic/FeatureFlags/`; no changes required
- `FeatureFlagService` (frontend) — already exists; no changes required
- `BaseApi` — already exists in `frontend/src/lib/api/`; `ApiFeatureFlagProvider` extends it

## Open Questions

_None — all decisions resolved during planning session._

## Out of Scope

- Per-user or percentage-based flag targeting (deferred to a future stub if multi-user support is added)
- String/numeric flag variants
- Admin UI or write API endpoints for flag management
- Server-side caching

## Success Metrics

- `GET /feature-flags` returns the correct flag list from the database
- Frontend reads flags at app load and correctly shows/hides elements based on `isEnabled`
- `DEMO_FEATURE_FLAG` is present in the DB after initial migration with `IsEnabled = false`
- Adding a new flag requires only a new EF migration — no changes to the flag system code

## Timeline / Milestones

_TBD_
