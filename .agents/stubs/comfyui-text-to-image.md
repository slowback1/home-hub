# ComfyUI Text-to-Image

**Status:** stub
**Created:** 2026-05-09

## Summary

Integrate with a locally running ComfyUI instance so the user can select a workflow, enter a prompt, run inference, and view the generated image in the app.

## Problem / Opportunity

ComfyUI workflows are built and tested outside the app, but there's no way to trigger them or see results without switching to the ComfyUI UI. Bringing inference into HomeHub keeps the experience in one place.

## Success Looks Like

- The user navigates to a page, selects a ComfyUI workflow from a list, and enters a prompt
- Pressing a button submits the job to ComfyUI and shows a loading state while inference runs
- The generated image is displayed in the app once complete
- The backend can enumerate available workflows and proxy inference requests to the ComfyUI API

## Notes & Open Questions

- ComfyUI exposes a REST/WebSocket API — need to confirm which endpoint handles workflow submission and result polling
- Workflows are JSON files on disk; backend should read them from a configured directory
- How prompts are injected into a workflow node (e.g. which node ID holds the positive prompt text) may vary per workflow — needs a convention or per-workflow config
- Should generated images be saved to disk / a gallery, or just returned transiently?
- ComfyUI base URL should be configurable (likely via app settings or appsettings.json)
