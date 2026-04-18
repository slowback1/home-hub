# Add Production Docker Compose File

## Status

`done` <!-- pending | in-progress | done -->

## Description

Create `docker-compose.prod.yml` in the repo root. Unlike the development compose file, this references the published DockerHub images directly so the home server can pull and run them without a local build. This file is what the server's nightly auto-pull uses after `docker compose up -d`.

## Acceptance Criteria

- [ ] `docker-compose.prod.yml` exists at the repo root
- [ ] `backend` service references `<DOCKERHUB_USERNAME>/homehub-backend:latest` (actual username substituted, not a variable)
- [ ] `frontend` service references `<DOCKERHUB_USERNAME>/homehub-frontend:latest` (actual username substituted, not a variable)
- [ ] Both services have `restart: unless-stopped`
- [ ] Port mappings match the existing dev setup (`backend` on `8080:8080`, `frontend` on `80:80`)
- [ ] File passes `docker compose -f docker-compose.prod.yml config` validation with no errors

## Notes

- `DOCKERHUB_USERNAME` in this file is a **static string** (e.g. `johndoe/homehub-backend:latest`), not an environment variable or compose interpolation — compose files served to the home server have no access to GitHub Actions variables.
- Confirm the correct port mappings by checking the existing `docker-compose.yml` or `docker-compose.dev.yml` before writing.
- See PRD §Production Compose File for the illustrative config.
