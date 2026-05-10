# Add Workflow Management Page (/comfyui/config)

## Status

`pending`

## Description

Add the `/comfyui/config` SvelteKit route for managing workflows: a table listing all saved workflows with a delete button per row, and an "Add Workflow" form with a name field and a JSON textarea.

## Acceptance Criteria

- [ ] `frontend/src/routes/comfyui/config/+page.svelte` exists and renders at `/comfyui/config`
- [ ] Page loads and displays the full list of workflows (name column, delete button per row)
- [ ] "Add Workflow" form has a `Name` text input and a `WorkflowJson` textarea; submitting calls `POST /api/comfyui/workflows` and refreshes the list
- [ ] Delete button calls `DELETE /api/comfyui/workflows/{id}` and removes the row from the list
- [ ] API client methods (`getWorkflows`, `createWorkflow`, `deleteWorkflow`) added to a `ComfyUiApi` class in `frontend/src/lib/api/`
- [ ] Page follows the layout and component conventions of other config pages in the app

## Notes

The `WorkflowJson` textarea will hold large JSON blobs. No validation of the JSON content is needed beyond it being non-empty — the user is responsible for pasting valid ComfyUI API-format JSON with placeholders in place.
