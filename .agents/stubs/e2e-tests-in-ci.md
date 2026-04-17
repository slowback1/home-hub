# E2E Tests in CI

**Status:** stub
**Created:** 2026-04-17

## Summary

Run the Playwright BDD end-to-end test suite as part of the CI pipeline, so that full-stack regressions are caught before images are pushed to DockerHub.

## Problem / Opportunity

The CI/CD pipeline (see `prds/ci-cd-deployment-pipeline.md`) gates merges on unit and integration tests only. E2E tests require both the backend and frontend to be running simultaneously, which adds significant complexity to CI. Deferring this keeps the initial pipeline simple while leaving a clear path to higher confidence later.

## Success Looks Like

- E2E tests run automatically on pull requests and/or pushes to `main`
- A failing E2E test blocks the Docker push
- The full stack (backend + frontend) is spun up ephemerally within the CI job

## Notes & Open Questions

- Should E2E run on every PR, or only on push to `main` (slower feedback vs. lower cost)?
- How are the backend and frontend services started in CI — docker compose, or separate run steps?
- Should a test database seed be applied before E2E runs?
- Parallelism strategy — single browser, multiple shards?
- Should Playwright HTML reports be uploaded as CI artifacts?
