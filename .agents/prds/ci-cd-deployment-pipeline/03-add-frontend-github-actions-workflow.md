# Add Frontend GitHub Actions Workflow

## Status

`pending` <!-- pending | in-progress | done -->

## Description

Create `.github/workflows/frontend.yml`. On every pull request targeting `main` the job runs install, lint, and unit tests. On a push to `main` it additionally logs in to DockerHub, builds the frontend Docker image, and pushes it tagged `:latest`. Runs concurrently with the backend job.

## Acceptance Criteria

- [ ] `.github/workflows/frontend.yml` exists and is valid YAML accepted by GitHub Actions
- [ ] Workflow triggers on `pull_request` (targeting `main`) and `push` (to `main`)
- [ ] Steps in order: checkout → setup Node.js → `npm ci` → `npm run lint` → `npm run test:ci`
- [ ] Node.js version is sourced from `.nvmrc` if present, otherwise current LTS
- [ ] DockerHub login, image build, and push steps are conditional on `github.event_name == 'push'` and do not run on pull requests
- [ ] Image is tagged `${{ vars.DOCKERHUB_USERNAME }}/homehub-frontend:latest`
- [ ] Image is built from `frontend/docker/Dockerfile`
- [ ] DockerHub credentials use `secrets.DOCKERHUB_TOKEN` and `vars.DOCKERHUB_USERNAME` — no hardcoded values
- [ ] A failed lint or test step prevents the Docker push from running

## Notes

- `npm run test:ci` is the non-interactive (CI-safe) test command; verify this script exists in `frontend/package.json` before finalising.
- See PRD §Workflow Details → frontend.yml for the full step list.
