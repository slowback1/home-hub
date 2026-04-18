# Configure Connection Strings and Environment Appsettings

## Status

`pending` <!-- pending | in-progress | done -->

## Description

Update `appsettings.json` to set `CrudFactory:Implementation = "EntityFramework"` and the Postgres connection string pointing at `localhost:5432` (for `dotnet run` outside Docker). Create `appsettings.Test.json` so integration tests running under the `"Test"` environment explicitly use `InMemory` rather than relying on the previous implicit fallthrough.

## Acceptance Criteria

- [ ] `appsettings.json` contains `CrudFactory:Implementation = "EntityFramework"`
- [ ] `appsettings.json` contains `ConnectionStrings:DefaultConnection` pointing at `Host=localhost;Port=5432;Database=homehub;Username=homehub;Password=homehub`
- [ ] `appsettings.Test.json` exists in `backend/WebAPI/` with `CrudFactory:Implementation = "InMemory"`
- [ ] `appsettings.Development.json` no longer sets `CrudFactory:Implementation` (it is now governed by the base `appsettings.json`); the `FileData:Directory` entry can remain
- [ ] All integration tests continue to pass (`dotnet test`) — confirming `appsettings.Test.json` correctly routes the test environment to `InMemory`
- [ ] `dotnet run` with the dev Postgres container running connects successfully (app starts without a DB connection error)

## Notes

- `appsettings.json` is at `backend/WebAPI/appsettings.json`.
- `appsettings.Development.json` is at `backend/WebAPI/appsettings.Development.json`.
- `appsettings.Test.json` must be placed at `backend/WebAPI/appsettings.Test.json` and will be automatically loaded by ASP.NET Core when `ASPNETCORE_ENVIRONMENT=Test` (set via `builder.UseEnvironment("Test")` in `ControllerTestBase`).
- The connection string in `appsettings.json` uses `localhost:5432` because it is intended for `dotnet run` on the host machine connecting to the mapped Docker port. Inside the dev Docker Compose network, the hostname is `postgres` — that override is handled by the `environment:` block in `docker-compose.yml` (task 06).
- Prod credentials are never committed to the repo — they arrive via a bind-mounted `appsettings.Production.json` managed in Portainer.
