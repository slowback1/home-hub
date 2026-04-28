# Add Postgres Container to Dev Docker Compose

## Status

`pending` <!-- pending | in-progress | done -->

## Description

Add a `postgres` service to `backend/development/docker-compose.yml` so the full dev stack (Postgres + Jaeger + WebAPI) starts with a single `docker compose up`. Data is persisted across restarts via a named Docker volume. The `webapi` service gains a `depends_on` entry for `postgres`.

## Acceptance Criteria

- [ ] `postgres` service added to `backend/development/docker-compose.yml` using the official `postgres` image
- [ ] Postgres credentials set to: database `homehub`, username `homehub`, password `homehub`
- [ ] Port `5432` mapped to `localhost:5432`
- [ ] A named Docker volume (e.g. `homehub-postgres-data`) is declared and mounted at `/var/lib/postgresql/data`
- [ ] `webapi` service has `depends_on: postgres` so Postgres starts before the app container
- [ ] `docker compose up` in `backend/development/` starts all three services without errors

## Notes

- File is at `backend/development/docker-compose.yml`.
- The `webapi` service in this compose file builds from the local Dockerfile — it will need the connection string env var set (or rely on `appsettings.json` added in task 07). If the connection string is not yet in `appsettings.json`, add an `environment:` entry to the `webapi` service:
  ```yaml
  environment:
    ConnectionStrings__DefaultConnection: "Host=postgres;Port=5432;Database=homehub;Username=homehub;Password=homehub"
  ```
  using the service name `postgres` as the host (Docker internal DNS). This can be removed once `appsettings.json` is updated in task 07.
- The `localhost:5432` mapping is for `dotnet run` (outside Docker); the in-compose hostname is `postgres`.
