# API Key Auth Middleware and Health Endpoint

## Status

`done`

## Description

Add a FastAPI dependency that validates the `Authorization: Bearer <key>` header on all protected routes, and implement `GET /health` as an unauthenticated health check endpoint.

## Acceptance Criteria

- [ ] All routes except `GET /health` reject requests missing or with an incorrect `Authorization: Bearer` header with `HTTP 401`
- [ ] `GET /health` returns `HTTP 200` with no auth header required
- [ ] API key is read from the `API_KEY` environment variable; service fails to start if `API_KEY` is unset or empty
- [ ] Unit tests cover: valid key accepted, missing header rejected, wrong key rejected, health endpoint bypasses auth

## Notes

Implement as a FastAPI `Depends` dependency so it can be applied per-router rather than globally, making it easy to exempt `/health`. The `API_KEY` env var is set in `.env` on the GPU box and must never be hard-coded.
