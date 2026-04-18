# Add Backend GitHub Actions Workflow

## Status

`done` <!-- pending | in-progress | done -->

## Description

Create `.github/workflows/backend.yml`. On every pull request targeting `main` the job runs restore, build, and tests. On a push to `main` it additionally logs in to DockerHub, builds the backend Docker image, and pushes it tagged `:latest`. The backend and frontend jobs are independent and run concurrently.

## Acceptance Criteria

- [ ] `.github/workflows/backend.yml` exists and is valid YAML accepted by GitHub Actions
- [ ] Workflow triggers on `pull_request` (targeting `main`) and `push` (to `main`)
- [ ] Steps in order: checkout → setup .NET 8 → `dotnet restore` → `dotnet build --no-restore` → `dotnet test`
- [ ] DockerHub login, image build, and push steps are conditional on `github.event_name == 'push'` and do not run on pull requests
- [ ] Image is tagged `${{ vars.DOCKERHUB_USERNAME }}/homehub-backend:latest`
- [ ] Image is built from `backend/WebAPI/Dockerfile`
- [ ] DockerHub credentials use `secrets.DOCKERHUB_TOKEN` and `vars.DOCKERHUB_USERNAME` — no hardcoded values
- [ ] A failed `dotnet test` prevents the Docker push from running

## Notes

- Integration tests use the `InMemory` data layer so no external database is needed in CI.
- `DOCKERHUB_USERNAME` is a repository **variable** (`vars.*`), not a secret — it appears in image tag names and is not sensitive.
- `DOCKERHUB_TOKEN` is a repository **secret** (`secrets.*`) — use a DockerHub access token with Read & Write scope, not the account password.
- See PRD §Workflow Details → backend.yml for the full step list.
