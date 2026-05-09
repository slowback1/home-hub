# Config Seeding and DI Wiring

## Status

`done`

## Description

Seed the three new `SystemConfig` entries in `Program.cs` and register all new services: `OllamaClient` (with 60 s timeout), `RandomActivitySelector`, `AiActivitySelector`, and the `IActivitySelector` factory that reads `activity::selector` to choose an implementation. This is the final integration task — after it, switching from random to AI selection requires only a config value change.

## Acceptance Criteria

- [ ] `Program.cs` seeds `ollama::url` (default `""`, type `"string"`)
- [ ] `Program.cs` seeds `activity::selector` (default `"random"`, type `"string"`)
- [ ] `Program.cs` seeds `activity::ai_model` (default `"llama3.2"`, type `"string"`)
- [ ] `OllamaClient` registered as a typed `HttpClient` with `Timeout` set to 60 seconds
- [ ] `RandomActivitySelector` and `AiActivitySelector` registered as scoped services
- [ ] `IActivitySelector` registered via factory: reads `activity::selector` config value; returns `AiActivitySelector` when value is `"ai"`, `RandomActivitySelector` otherwise
- [ ] Setting `activity::selector` to `"ai"` (with a valid `ollama::url` and `activity::ai_model`) causes the job to use the AI selector
- [ ] Setting `activity::selector` to `"random"` (or any other value) uses the random selector
- [ ] Existing integration tests still pass

## Notes

- Follow the `IAudiobookService` factory pattern already in `Program.cs` for the `IActivitySelector` factory
- Seed entries follow the same `new SystemConfig { Id = "ollama::url", Namespace = "ollama", Key = "url", Value = "", Type = "string" }` pattern used for audiobook and weather config
- No secret entries needed — Ollama has no API key
