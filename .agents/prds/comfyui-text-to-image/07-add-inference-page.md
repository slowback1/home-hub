# Add Inference Page (/comfyui)

## Status

`pending`

## Description

Add the `/comfyui` SvelteKit route: the main text-to-image page where the user selects a workflow, enters a positive and negative prompt, triggers generation, and sees the resulting image. Add a nav link for the route.

## Acceptance Criteria

- [ ] `frontend/src/routes/comfyui/+page.svelte` exists and renders at `/comfyui`
- [ ] Page loads the workflow list from `GET /api/comfyui/workflows` and populates a dropdown
- [ ] Positive prompt textarea and negative prompt textarea are present
- [ ] "Generate" button is disabled while the request is in flight; a loading indicator is shown
- [ ] On success, the generated image is displayed below the form using the returned `imageBase64` data URI
- [ ] On failure, an inline error banner is shown and the Generate button re-enables
- [ ] A nav link to `/comfyui` is added to the app navigation
- [ ] `generateImage` method added to `ComfyUiApi` in `frontend/src/lib/api/`

## Notes

The generate request blocks for up to ~120 seconds. The loading state must remain visible for the full duration — do not add a client-side timeout shorter than 120s.

The generated image is transient: it is not saved and disappears on page navigation or refresh. No persistence UI is needed.
