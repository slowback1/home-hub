# Build ComfyUI E2E Stub Server

## Status

`pending`

## Description

Build a minimal Node.js stub server that mimics the ComfyUI REST API for E2E tests. Wire it into the Playwright `webServer` config so it starts automatically before the test suite, and point the E2E backend config at it via `appsettings.E2E.json`.

## Acceptance Criteria

- [ ] `e2e/stub-servers/comfyui/server.js` (or `.ts`) starts an HTTP server on a fixed port (e.g. `8199`)
- [ ] `POST /prompt` returns `{ "prompt_id": "test-prompt-123" }`
- [ ] `GET /history/test-prompt-123` returns a completed output object referencing a test image filename
- [ ] `GET /view` (with any query params) returns a minimal valid PNG image (can be a 1×1 pixel PNG)
- [ ] `POST /stub/fail` puts the server into failure mode; the next `POST /prompt` returns HTTP 500; mode resets after triggering once
- [ ] Server added as a `webServer` entry in `e2e/playwright.config.ts` so it starts before tests run
- [ ] `appsettings.E2E.json` sets `comfyui::base_url` to the stub server's URL (e.g. `http://localhost:8199`)
- [ ] `task e2e:test` starts the stub server cleanly alongside the existing backend and frontend servers

## Notes

The stub server only needs to satisfy the four `@comfyui` E2E scenarios — it does not need to fully replicate the ComfyUI API.

The failure mode (`POST /stub/fail`) is used by the `@comfyui-generate-error` scenario to simulate an unreachable ComfyUI without changing config between tests.

A minimal 1×1 PNG in base64: `iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==`
