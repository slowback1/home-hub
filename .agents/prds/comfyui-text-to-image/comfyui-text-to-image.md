# PRD: ComfyUI Text-to-Image

## Status

`Draft`

## Overview

Integrate a locally running ComfyUI instance into HomeHub so the user can select a saved workflow, enter a prompt and negative prompt, trigger image generation, and view the result — all without leaving the app. Includes a lightweight workflow management page for adding and removing workflows.

## Problem Statement

ComfyUI workflows are built and tested in the ComfyUI UI, but there is no way to trigger them from HomeHub or view results without switching applications. Bringing inference into the app keeps the experience in one place and makes the feature a first-class part of the home dashboard.

## Goals

- Provide a `ComfyUiClient` that any future backend service can use to submit workflows and retrieve images
- Store ComfyUI workflows in the database with a user-friendly name, managed via a simple admin page
- Deliver an inference page where the user selects a workflow, enters prompts, and views the generated image
- Keep scope to the generate-and-view loop; no gallery or history in v1

## Non-Goals

- Persistent image gallery / history — deferred
- WebSocket-based real-time progress updates from ComfyUI — deferred
- Multiple simultaneous generation requests — out of scope for a single-user home app
- Editing workflow JSON in the app — delete and re-add instead
- Negative prompt node convention enforcement — the placeholder contract is sufficient

## User Stories / Use Cases

- **As a HomeHub user**, I want to select a ComfyUI workflow, enter a prompt, and see the generated image in the app, so I don't need to switch to the ComfyUI interface.
- **As a HomeHub operator**, I want to add and remove workflows through a config page, so I can iterate on workflows in ComfyUI and register them here without touching code.

## E2E Scenarios

The generate and error scenarios require a running ComfyUI-compatible server. For E2E tests, a lightweight stub server (`e2e/stub-servers/comfyui/`) will be implemented and wired into the Playwright `webServer` config alongside the existing backend and frontend entries.

**Stub server behaviour:**
- `POST /prompt` → returns `{ "prompt_id": "test-prompt-123" }`
- `GET /history/test-prompt-123` → returns a completed output referencing a small test PNG
- `GET /view?filename=test.png&...` → returns a minimal valid PNG
- `POST /stub/fail` → puts the stub into failure mode; next `/prompt` call returns 500 (used by the error scenario; reset after each test)

`appsettings.E2E.json` sets `comfyui::base_url` to the stub server's port (e.g. `http://localhost:8199`).

```gherkin
@comfyui
Feature: ComfyUI Text-to-Image

  @comfyui-add-workflow
  Scenario: Add a workflow on the config page
    Given I am on the ComfyUI config page
    When I fill in a workflow name and paste a workflow JSON
    And I submit the Add Workflow form
    Then the new workflow should appear in the workflow list

  @comfyui-delete-workflow
  Scenario: Delete a workflow on the config page
    Given a ComfyUI workflow exists
    And I am on the ComfyUI config page
    When I delete the workflow
    Then the workflow should no longer appear in the list

  @comfyui-generate-happy-path
  Scenario: Generate an image from a workflow
    Given a ComfyUI workflow exists
    And I am on the ComfyUI page
    When I select the workflow from the dropdown
    And I enter a prompt
    And I click Generate
    Then I should see a loading state while the image is generating
    And I should see the generated image when complete

  @comfyui-generate-error
  Scenario: Generate shows an error when ComfyUI is unreachable
    Given a ComfyUI workflow exists
    And the ComfyUI stub server is set to fail
    And I am on the ComfyUI page
    When I select the workflow and enter a prompt
    And I click Generate
    Then I should see an inline error message
    And the Generate button should be re-enabled
```

## Proposed Solution

1. Add `IComfyUiClient` / `ComfyUiClient` in the Logic layer — polls ComfyUI's REST API to submit a workflow and retrieve the resulting image bytes.
2. Add a `ComfyUiWorkflow` EF Core entity and migration. Expose CRUD endpoints (list, create, delete).
3. Add a `POST /api/comfyui/generate` endpoint that loads the workflow from the DB, injects prompt placeholders, submits to ComfyUI, polls until done, and returns the image as base64.
4. Add frontend routes: `/comfyui` (inference page) and `/comfyui/config` (workflow management).
5. Seed a `comfyui::base_url` entry in system config.

## Technical Approach

### ComfyUiClient

- **Location**: `backend/Logic/ComfyUi/ComfyUiClient.cs`
- **Interface**: `IComfyUiClient` in `backend/Common/Interfaces/`
- **Methods**:
  - `Task<string> SubmitPromptAsync(string workflowJson)` — `POST /prompt` with the workflow JSON body; returns `prompt_id` from the response
  - `Task<byte[]> PollForImageAsync(string promptId)` — polls `GET /history/{promptId}` every 1 second until the output appears, then fetches the image bytes via `GET /view?filename=...&subfolder=...&type=output`
- **Pattern**: follows `OllamaClient` — typed `HttpClient`, reads `comfyui::base_url` from `ISystemConfigProvider` at call time
- **Timeout**: 120 seconds (set at `AddHttpClient<ComfyUiClient>()` registration)
- **Error handling**: throws a descriptive exception if ComfyUI is unreachable or returns a non-success status; the controller catches and returns a meaningful error message

### Workflow Entity

```csharp
// backend/EntityFramework/Entities/ComfyUiWorkflow.cs
public class ComfyUiWorkflow
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string WorkflowJson { get; set; } = "";
}
```

EF Core migration generated via `dotnet ef migrations add AddComfyUiWorkflow`.

### Prompt Injection

Before posting to ComfyUI, the backend performs string substitution on the stored workflow JSON:

```csharp
workflowJson
    .Replace("{{prompt}}", request.Prompt)
    .Replace("{{negative_prompt}}", request.NegativePrompt ?? "");
```

The user is responsible for placing `{{prompt}}` and `{{negative_prompt}}` in the correct node's text field when saving the workflow JSON.

### Backend API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/api/comfyui/workflows` | List all workflows (`Id`, `Name` only — no JSON) |
| `POST` | `/api/comfyui/workflows` | Create workflow (`Name`, `WorkflowJson`) |
| `DELETE` | `/api/comfyui/workflows/{id}` | Delete workflow |
| `POST` | `/api/comfyui/generate` | Submit inference and return image |

**Generate request body**:
```json
{ "workflowId": 1, "prompt": "...", "negativePrompt": "..." }
```

**Generate response body**:
```json
{ "imageBase64": "data:image/png;base64,..." }
```

### Generate Flow

1. Load `ComfyUiWorkflow` by `workflowId` from DB
2. Replace `{{prompt}}` and `{{negative_prompt}}` placeholders in `WorkflowJson`
3. Call `ComfyUiClient.SubmitPromptAsync(workflowJson)` → `promptId`
4. Call `ComfyUiClient.PollForImageAsync(promptId)` → `byte[]`
5. Return `{ imageBase64: "data:image/png;base64," + Convert.ToBase64String(bytes) }`

### Frontend Routes

**`/comfyui`** — Inference page:
- Dropdown to select a workflow (populated from `GET /api/comfyui/workflows`)
- Textarea for positive prompt
- Textarea for negative prompt
- "Generate" button — disabled while request is in flight
- Loading state shown while awaiting the blocking response
- Generated image displayed below once returned
- Inline error banner if the request fails; button re-enables

**`/comfyui/config`** — Workflow management page:
- Table of workflows (`Name`, delete button per row)
- "Add Workflow" form: `Name` text input, `WorkflowJson` textarea, submit button
- Follows the same layout/component patterns as other config pages in the app

### Configuration

New seed entry added alongside existing system config seeds in `Program.cs`:

| Namespace | Key | Default | Notes |
|-----------|-----|---------|-------|
| `comfyui` | `base_url` | `""` | Must be set to the local ComfyUI URL (e.g. `http://localhost:8188`) |

### DI Registration

```csharp
builder.Services.AddHttpClient<ComfyUiClient>()
    .ConfigureHttpClient(c => c.Timeout = TimeSpan.FromSeconds(120));
builder.Services.AddScoped<IComfyUiClient, ComfyUiClient>();
```

### Key Decisions

| Decision | Chosen Approach | Rationale |
|----------|-----------------|-----------|
| Inference result polling | Backend polls REST (`/history/{promptId}`) | Simpler than WebSocket; polling every 1s is fine for ~60s generations |
| Frontend ↔ backend communication | Blocking HTTP request | No frontend polling state needed; 120s timeout accommodates observed generation times |
| Workflow storage | DB table (`ComfyUiWorkflow`) | Avoids filesystem dependency; allows user-friendly names separate from filenames |
| Prompt injection | `{{prompt}}` / `{{negative_prompt}}` placeholder substitution | Zero extra fields; works across any workflow structure; easily extensible |
| Image persistence | Transient (base64 in response) | ComfyUI retains its own output; gallery is a follow-on feature |
| Base URL config | `comfyui::base_url` in `ISystemConfigProvider` | Consistent with Ollama pattern; editable via existing admin UI |
| Error handling | Inline error banner, button re-enables | Consistent with audiobook page pattern |

### Dependencies

- ComfyUI running locally, reachable from the backend host
- Workflows exported from ComfyUI in **API format** ("Save (API Format)" in the ComfyUI UI) with `{{prompt}}` and `{{negative_prompt}}` placeholders placed in the appropriate nodes before saving to the DB
- Existing `ISystemConfigProvider` / `SystemConfig` infrastructure
- Existing EF Core migration toolchain

## Open Questions

_None — all major decisions resolved during design session._

## Out of Scope

- Persistent image gallery / history
- WebSocket / real-time progress from ComfyUI
- Editing workflow JSON in the UI
- Multiple simultaneous inference requests
- Seed/default workflows

## Success Metrics

- User can add a workflow, enter a prompt, click Generate, and see the resulting image in the app
- Workflow management (add/delete) works without touching code or the filesystem
- If ComfyUI is unreachable, a clear error message appears and the Generate button re-enables
- `ComfyUiClient` follows the same interface and DI patterns as `OllamaClient`

## Timeline / Milestones

_TBD_
