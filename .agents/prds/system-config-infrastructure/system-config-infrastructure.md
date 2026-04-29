# PRD: System Config Infrastructure

## Status

`Draft` <!-- Draft | Review | Approved | Superseded -->

## Overview

A database-backed key/value store for runtime application configuration. Rather than requiring a redeploy to change non-critical settings, this system allows config values to be read and updated at runtime via a service layer and REST API. Infrastructure-critical settings (DB connection strings, CORS origins) remain in `appsettings.json`; everything else can live in the database.

## Problem Statement

Config file-based settings require a redeploy to change and cannot be managed at runtime. Most application configuration — integration settings, display preferences, API keys for third-party services — doesn't need to be tied to the file system. Features like the weather widget will need runtime-configurable settings (e.g. API keys, location) that should be manageable without touching files or restarting the app.

## Goals

- Provide a `system_config` DB table that stores typed key/value pairs
- Provide an `ISystemConfigProvider` service interface usable by any backend feature
- Expose a REST API for reading (with secret masking) and updating config values
- Support secrets (API keys, credentials) stored safely with masked read responses
- Allow new config entries to be introduced via EF migrations, not the API

## Non-Goals

- No runtime create or delete of config entries via the API (migrations only)
- No caching — DB round-trips are acceptable at this scale
- No access control / auth — consistent with the rest of the current API
- No default-value fallback mechanism — missing keys throw
- No bulk namespace GET endpoint
- No config values that replace infrastructure-critical settings (DB URL, CORS)

## User Stories / Use Cases

- **As a backend feature** (e.g. weather widget), **I want to** read a config value by namespace and key via `ISystemConfigProvider`, **so that** I can retrieve runtime settings like API keys without hardcoding them.
- **As an admin UI** (system-config-ui), **I want to** fetch all config entries with secrets masked and update individual values, **so that** settings can be managed at runtime without a redeploy.
- **As a developer**, **I want to** introduce a new config entry via an EF migration, **so that** the app has a well-defined default state on a fresh deployment.

## E2E Scenarios

_N/A — backend infrastructure only._

## Proposed Solution

Introduce a `SystemConfig` model with a composite-style string ID (`"{namespace}::{key}"`), a dedicated `ISystemConfigProvider` interface in `Common`, an EntityFramework implementation, and a `SystemConfigController` exposing three REST endpoints. Config rows are seeded and managed exclusively via EF migrations.

## Technical Approach

### Data Model

`SystemConfig` implements `IIdentifyable` with `Id = "{namespace}::{key}"` (using `::` as separator to avoid ambiguity with hyphenated names). Fields:

| Column | Type | Notes |
|--------|------|-------|
| `Id` | `string` | `"{namespace}::{key}"`, primary key |
| `Namespace` | `string` | snake_case grouping (e.g. `weather`) |
| `Key` | `string` | snake_case identifier (e.g. `api_key`) |
| `Value` | `string` | All values stored as strings |
| `Type` | `string` | Discriminator: `string` or `boolean` |
| `IsSecret` | `bool` | If true, value is masked as `"***"` on read |

Namespace and key values must not contain `::`. Both should be snake_case by convention.

### Service Interface

```csharp
public interface ISystemConfigProvider
{
    Task<SystemConfig> GetAsync(string @namespace, string key);
    Task<SystemConfig> GetSecretAsync(string @namespace, string key);
    Task<IEnumerable<SystemConfig>> GetAllAsync();
    Task<SystemConfig> UpdateAsync(string @namespace, string key, string value);
}
```

- `GetAsync` — returns the entry; secrets are returned with `Value = "***"`. Throws if not found.
- `GetSecretAsync` — returns the entry with the real value. Throws if not found. For internal consumers that need actual credentials.
- `GetAllAsync` — returns all entries; secrets masked. Used by the admin API.
- `UpdateAsync` — updates `Value` only. `Type`, `IsSecret`, `Namespace`, and `Key` are immutable after migration.

Masking is a service-layer concern. The controller does not apply additional masking.

### API Endpoints

| Method | Path | Description |
|--------|------|-------------|
| `GET` | `/api/system-config` | Returns all entries as a flat array, secrets masked |
| `GET` | `/api/system-config/{namespace}/{key}` | Returns single entry, secret masked |
| `PUT` | `/api/system-config/{namespace}/{key}` | Updates value; body: `{ "value": "..." }` |

Response shape (per entry):
```json
{
  "id": "weather::api_key",
  "namespace": "weather",
  "key": "api_key",
  "value": "***",
  "type": "string",
  "isSecret": true
}
```

Secret fields always return `"***"` on GET — the real value is never exposed via the API.

### Key Decisions

| Decision | Chosen Approach | Rationale |
|----------|----------------|-----------|
| Value storage | All values stored as `string`; `Type` discriminator column | Simple schema, coercion at call site, UI can render correct input widget |
| Namespacing | Separate `Namespace` column (snake_case) | Enables clean grouping queries; avoids `LIKE` hacks on dot-notation keys |
| Composite key | `Id = "{namespace}::{key}"` | Fits `IIdentifyable`/`ICrud<T>` without schema changes; `::` avoids hyphen ambiguity |
| Default values | None — service throws if key not found | Forces explicit seeding via migrations; avoids silent misconfiguration |
| Secrets | `IsSecret` flag; `GetAsync` masks, `GetSecretAsync` returns real value | Internal consumers opt in to secret reads explicitly; API never leaks values |
| Caching | None | DB is local; read frequency is low; avoids stale-value bugs |
| Auth | None | Consistent with current API; can be added uniformly later |
| Row lifecycle | Migrations only (no API create/delete) | Config entries are part of the schema, not user-generated data |

### Dependencies

- EntityFramework project (`AppDbContext`) — `DbSet<SystemConfig>` added here
- `Common` project — `SystemConfig` model and `ISystemConfigProvider` interface
- `ICrud<T>` / `ICrudFactory` — EF implementation wraps `ICrud<SystemConfig>` obtained from the factory
- EF migrations — all new config entries introduced via migrations

## Open Questions

_None — all design decisions resolved during planning._

## Out of Scope

- System Config Admin UI (tracked separately as `system-config-ui` stub)
- Secret masking UI (password-style input fields) — UI concern for `system-config-ui`
- Config value validation beyond type coercion
- Namespaced bulk GET (`GET /api/system-config/{namespace}`)
- Auth / role-based access to config endpoints
- Caching layer

## Success Metrics

- Any backend use case can inject `ISystemConfigProvider` and read a config value without touching `appsettings.json`
- Secret values are never returned in plaintext via the REST API
- New config entries can be introduced by adding an EF migration — no code changes required in the service or controller layer
- All existing tests continue to pass with the new `SystemConfig` DbSet added to `AppDbContext`

## Timeline / Milestones

_TBD_
