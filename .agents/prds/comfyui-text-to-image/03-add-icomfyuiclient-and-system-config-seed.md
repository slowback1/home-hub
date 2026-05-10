# Add IComfyUiClient, ComfyUiClient, and System Config Seed

## Status

`done`

## Description

Implement the typed HTTP client that wraps ComfyUI's REST API, register it in DI with a 120-second timeout, and seed the `comfyui::base_url` config entry. This is the infrastructure layer that the generate endpoint will depend on.

## Acceptance Criteria

- [ ] `backend/Common/Interfaces/IComfyUiClient.cs` defines the interface with `SubmitPromptAsync` and `PollForImageAsync`
- [ ] `backend/Logic/ComfyUi/ComfyUiClient.cs` implements the interface using a typed `HttpClient`
  - `SubmitPromptAsync(string workflowJson)` — `POST /prompt`, returns `prompt_id` string
  - `PollForImageAsync(string promptId)` — polls `GET /history/{promptId}` every 1 second until output appears, then fetches image bytes via `GET /view?filename=...&subfolder=...&type=output`
- [ ] Base URL read from `ISystemConfigProvider` at call time using namespace `"comfyui"`, key `"base_url"`
- [ ] DI registered in `Program.cs` with 120s `HttpClient` timeout: `builder.Services.AddHttpClient<ComfyUiClient>().ConfigureHttpClient(c => c.Timeout = TimeSpan.FromSeconds(120))`
- [ ] `builder.Services.AddScoped<IComfyUiClient, ComfyUiClient>()` registered in `Program.cs`
- [ ] `comfyui` / `base_url` seeded in the system config seed block in `Program.cs` (default value `""`)
- [ ] Unit tests cover happy-path submission, successful poll response, and a failed/unreachable scenario using the `FakeHandler` pattern from existing tests

## Notes

Follow the `OllamaClient` pattern throughout — it lives in `backend/Logic/Ollama/OllamaClient.cs` and is the closest existing analogue. The `GpuServiceClient` is also a useful reference for error handling patterns.

ComfyUI's `/history/{promptId}` response structure: when the job is complete, the prompt ID key appears in the top-level object with an `outputs` map. Poll until that key is present. The image filename and subfolder are nested inside `outputs`.
