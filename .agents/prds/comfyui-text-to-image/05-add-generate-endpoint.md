# Add Generate Endpoint

## Status

`pending`

## Description

Add `POST /api/comfyui/generate` to `ComfyUiController`. The endpoint loads the requested workflow from the DB, injects `{{prompt}}` and `{{negative_prompt}}` placeholders, submits to ComfyUI via `IComfyUiClient`, and returns the image as a base64 data URI in the response body.

## Acceptance Criteria

- [ ] `POST /api/comfyui/generate` accepts `{ workflowId, prompt, negativePrompt }` (negativePrompt optional)
- [ ] Loads the workflow by `workflowId`; returns 404 if not found
- [ ] Replaces `{{prompt}}` and `{{negative_prompt}}` in `WorkflowJson` before submission
- [ ] Calls `IComfyUiClient.SubmitPromptAsync` then `PollForImageAsync`
- [ ] Returns `{ imageBase64: "data:image/png;base64,..." }` on success
- [ ] Returns a descriptive error message (not a raw exception) if ComfyUI is unreachable or generation fails
- [ ] Tests cover: happy path (mocked client), workflow not found, and client throws (error response)

## Notes

`negativePrompt` defaults to `""` if omitted — `{{negative_prompt}}` in the workflow JSON is replaced with an empty string rather than the literal placeholder text.

The blocking nature of this endpoint (up to ~120s) is intentional per the PRD. No job queue or async polling pattern is needed.
