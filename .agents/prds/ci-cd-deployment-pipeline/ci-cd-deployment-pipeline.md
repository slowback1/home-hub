# PRD: CI/CD & Deployment Pipeline

**Status:** ready  
**Created:** 2026-04-17  
**Promoted from stub:** `.agents/stubs/ci-cd-deployment-pipeline.md`

---

## Problem

Deploying changes to HomeHub is a fully manual process. As features accumulate, this becomes a bottleneck and a source of errors. There is no automated gate preventing broken code from reaching the server.

## Goal

Automate build, test, and Docker image publishing so that a push to `main` is sufficient to ship a change. The home server handles pulling and restarting containers on its own schedule; the pipeline's only responsibility is producing a verified, up-to-date image on DockerHub.

---

## Success Criteria

- A pull request to `main` automatically runs all required tests; failures block merge
- A merge to `main` automatically builds and pushes Docker images tagged `latest` to DockerHub
- The pipeline requires zero manual steps to ship a change
- A failed build or test blocks the Docker push
- DockerHub credentials are never hardcoded; the username is configurable

---

## Scope

### In scope

- Two GitHub Actions workflow files placed in `.github/workflows/` (active immediately)
- A production Docker Compose file (`docker-compose.prod.yml`) in the repo root
- Deletion of the now-superseded draft workflows in `frontend/deployment/github_actions/`

### Out of scope

- E2E tests in CI (tracked separately as a stub)
- Self-hosted runners
- Staging / preview environments
- Automatic rollback
- Any deployment mechanism beyond pushing to DockerHub (the server's nightly auto-pull handles the rest)

---

## Architecture

### Deployment model

```
GitHub (push to main)
  └── GitHub Actions (hosted runner)
        ├── backend job  →  build + test  →  push homehub-backend:latest to DockerHub
        └── frontend job →  build + test  →  push homehub-frontend:latest to DockerHub

DockerHub
  └── home server (nightly auto-pull of :latest)
        └── docker compose up -d  (using docker-compose.prod.yml)
```

### Workflow triggers

| Event | Backend job | Frontend job |
|---|---|---|
| Pull request targeting `main` | test only (no push) | test only (no push) |
| Push to `main` | test + build + push | test + build + push |

### Jobs run in parallel

The `backend` and `frontend` jobs are independent and run concurrently.

---

## Workflow Details

### `.github/workflows/backend.yml`

**Steps:**

1. Checkout
2. Setup .NET 8
3. Restore dependencies (`dotnet restore`)
4. Build (`dotnet build --no-restore`)
5. Run unit + integration tests (`dotnet test`) — integration tests use the `InMemory` data layer
6. *(push to `main` only)* Log in to DockerHub using `DOCKERHUB_USERNAME` + `DOCKERHUB_TOKEN` secrets
7. *(push to `main` only)* Build Docker image from `backend/WebAPI/Dockerfile`, tag as `${{ vars.DOCKERHUB_USERNAME }}/homehub-backend:latest`
8. *(push to `main` only)* Push image to DockerHub

### `.github/workflows/frontend.yml`

**Steps:**

1. Checkout
2. Setup Node.js (version from `.nvmrc` or current LTS)
3. Install dependencies (`npm ci`)
4. Lint check (`npm run lint`)
5. Run unit tests (`npm run test:ci`)
6. *(push to `main` only)* Log in to DockerHub
7. *(push to `main` only)* Build Docker image from `frontend/docker/Dockerfile`, tag as `${{ vars.DOCKERHUB_USERNAME }}/homehub-frontend:latest`
8. *(push to `main` only)* Push image to DockerHub

---

## Secrets & Variables

| Name | Type | Purpose |
|---|---|---|
| `DOCKERHUB_USERNAME` | GitHub Actions variable (`vars.*`) | DockerHub account username — used to namespace image names |
| `DOCKERHUB_TOKEN` | GitHub Actions secret (`secrets.*`) | DockerHub access token (not password) for authentication |

> Use a DockerHub access token (Settings → Security → New Access Token) with `Read & Write` scope, not your account password.

---

## Production Compose File

A `docker-compose.prod.yml` will be added to the repo root. It references the DockerHub images (not local builds) so the home server can pull and run them directly.

```yaml
# docker-compose.prod.yml (illustrative — exact config TBD during implementation)
services:
  backend:
    image: <DOCKERHUB_USERNAME>/homehub-backend:latest
    restart: unless-stopped
    ports:
      - "8080:8080"

  frontend:
    image: <DOCKERHUB_USERNAME>/homehub-frontend:latest
    restart: unless-stopped
    ports:
      - "80:80"
```

The `DOCKERHUB_USERNAME` value in this file will be replaced with the actual username (it is a static string in a compose file, not a secret).

---

## Files Changed

| Action | Path |
|---|---|
| Create | `.github/workflows/backend.yml` |
| Create | `.github/workflows/frontend.yml` |
| Create | `docker-compose.prod.yml` |
| Delete | `frontend/deployment/github_actions/run-unit-tests.yml` |
| Delete | `frontend/deployment/github_actions/deploy-to-s3.yml` |
| Delete | `frontend/deployment/github_actions/README.md` |

---

## Open Questions

None — all decisions resolved during refinement.
