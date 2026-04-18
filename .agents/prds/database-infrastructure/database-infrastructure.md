# PRD: Database Infrastructure

## Status

`Draft` <!-- Draft | Review | Approved | Superseded -->

## Overview

Wire up PostgreSQL as the real persistence layer for HomeHub, replacing the current `InMemory` and `FileData` stubs. This involves creating an EF Core `EntityFramework` implementation of `ICrud<T>`, updating the `ICrud<T>` interface for EF Core compatibility, adding a Postgres container to the dev environment, and ensuring migrations run automatically on startup.

## Problem Statement

All current persistence implementations are either ephemeral (`InMemory`) or fragile (`FileData` flat JSON). Every future feature that needs reliable storage must bolt on its own ad-hoc solution. Establishing a proper database layer now gives all future features a consistent, well-managed foundation to build on.

## Goals

- Application connects to PostgreSQL on startup in all non-test environments
- Schema migrations are version-controlled and run automatically on app startup
- An EF Core query layer is available via the existing `ICrud<T>` / `ICrudFactory` abstraction
- Local development setup is reproducible: one `docker compose up` starts the full dev stack
- `ICrud<T>` query methods use `Expression<Func<T, bool>>` for SQL-translatable queries

## Non-Goals

- Development seed data (deferred — add via `ConsoleUtilities` when needed)
- Fixing the static-state leak in `InMemory` between integration tests (pre-existing issue, separate task)
- Managing the production Postgres container (handled externally via Portainer)
- Per-feature or bounded-context `DbContext` splitting

## User Stories / Use Cases

- **As a developer**, I want a Postgres container to start alongside the app in dev so that I can work against a real database without any manual setup.
- **As a developer**, I want schema migrations to apply automatically on startup so that I never need to remember to run `dotnet ef database update` after pulling changes.
- **As a developer**, I want a `task new-migration NAME=...` command so that I can create new migrations without memorising the full `dotnet ef` invocation.
- **As a future feature implementor**, I want a reliable `ICrud<T>` backed by Postgres so that I can persist data without designing a storage strategy from scratch.

## E2E Scenarios

_N/A — infrastructure/backend only._

## Proposed Solution

Add a new `EntityFramework` class library project to the backend solution that implements `ICrud<T>` and `ICrudFactory` using EF Core with a single `AppDbContext`. Register it in `CrudFactoryConfigurator` alongside the existing `InMemory` and `FileData` implementations. Update `ICrud<T>`'s query methods to use `Expression<Func<T, bool>>` and update all existing implementations accordingly. Add a Postgres container to the dev `docker-compose.yml`, configure connection strings in `appsettings.json`, and add an `appsettings.Test.json` to keep integration tests on `InMemory`.

## Technical Approach

### Key Decisions

| Decision | Chosen Approach | Rationale |
|----------|----------------|-----------|
| Database engine | PostgreSQL | Free, lightweight container, excellent EF Core support, community standard for self-hosted |
| Dev topology | Postgres container added to `backend/development/docker-compose.yml` | Keeps the full dev stack in one `docker compose up` |
| Prod topology | Standalone Postgres container managed externally (Portainer) | Home server ops preference; app only needs a valid connection string |
| Prod credentials delivery | Bind-mounted `appsettings.Production.json` | ASP.NET Core natively layers this file; no repo changes to `docker-compose.prod.yml` needed |
| Migrations | `MigrateAsync()` on app startup | Simple, no extra tooling, appropriate for home-server context |
| ORM approach | EF Core Code First | Schema version-controlled with code; standard for greenfield .NET |
| Project structure | New `EntityFramework` class library | Consistent with existing `InMemory` / `FileData` pattern |
| DbContext | Single shared `AppDbContext` | Standard EF Core pattern; straightforward to extend |
| Generic CRUD | `DbContext.Set<T>()` in `EfCrud<T>` | One generic class handles all entities; mirrors existing pattern |
| DI lifetime | All three `ICrudFactory` implementations registered as **Scoped** | Prevents captive-dependency runtime errors when EF's scoped `DbContext` is involved |
| `ICrud<T>` query signature | `Expression<Func<T, bool>>` | Allows EF Core to translate queries to SQL; `InMemory`/`FileData`/`TestCrud` use `.Compile()` to preserve existing behaviour |
| `CrudFactoryType` enum | Add `EntityFramework` entry; explicit `case "entityframework":` branch; `default:` handles InMemory | Removes silent fallthrough; eliminates linter warning for duplicate case actions |
| Dev connection string | `appsettings.json` → `localhost:5432`, db/user/password all `homehub` | Committed safe dev credentials; works with `dotnet run` once dev compose is up |
| Test environment | `appsettings.Test.json` sets `CrudFactory:Implementation = "InMemory"` | Integration tests use `UseEnvironment("Test")`; prevents them hitting a real DB |
| Default implementation | `appsettings.json` sets `CrudFactory:Implementation = "EntityFramework"` | EF is the default for all environments; tests and local overrides opt out explicitly |
| Initial migration | `InitialCreate` covering `AppUser` and `ExampleData` committed to repo | `MigrateAsync()` works immediately on first deploy with no manual steps |
| Dev tooling | `task new-migration NAME=<MigrationName>` in backend Taskfile | Convenience wrapper around `dotnet ef migrations add` with correct project flags |
| Design-time package | `Microsoft.EntityFrameworkCore.Design` in `EntityFramework` project | Self-contained tooling; no global EF CLI install required |
| Seeding | None | Deferred; `ConsoleUtilities` is the natural home when needed |

### Dependencies

- `Microsoft.EntityFrameworkCore` (EF Core runtime)
- `Npgsql.EntityFrameworkCore.PostgreSQL` (Postgres EF Core provider)
- `Microsoft.EntityFrameworkCore.Design` (design-time tooling for migrations)
- `docker` / `docker compose` (dev Postgres container)
- ASP.NET Core configuration layering (`appsettings.{Environment}.json`)

## Open Questions

_None — all decisions resolved during planning session._

## Out of Scope

- Development seed data
- Static `InMemory` state leak between integration tests
- Production Postgres container management
- Performance optimisation of query materialisation (all `Expression` queries execute against the DB via EF Core translation; no known bottlenecks at home-server scale)

## Success Metrics

- App starts and connects to Postgres in dev with no manual steps beyond `docker compose up`
- `dotnet test` passes with all existing tests green (integration tests use `InMemory` via `appsettings.Test.json`)
- Migrations table exists in the dev Postgres DB after first startup
- `task new-migration NAME=Test` produces a new migration file in the `EntityFramework` project

## Timeline / Milestones

_TBD_
