# PRD: Ollama AI Client Infrastructure

## Status

`Draft`

## Overview

A typed HTTP client for the local Ollama instance, plus a strategy-based refactor of `ActivityPickerJob` that replaces its hardcoded weighted-random selection with a pluggable `IActivitySelector` interface. The first concrete AI implementation prompts Ollama to choose an activity from the configured list, using recent pick history as context, with automatic silent fallback to random selection on failure.

## Problem Statement

Without a shared AI client layer, every future feature that wants to use Ollama must implement its own HTTP wiring, error handling, and model configuration. The activity picker also has selection logic hardcoded inside the job — there is no way to swap strategies without rewriting it. Both problems compound as AI-powered features are added: repeated integration work and no clean seam for testing.

## Goals

- Provide a reusable `OllamaClient` that any backend service can inject and use for chat completions
- Extract activity selection into an `IActivitySelector` strategy interface, making the picker logic swappable
- Deliver a working `AiActivitySelector` that uses Ollama to pick an activity with recent-pick context
- Manage all configuration (Ollama URL, model, selector type) through the existing `SystemConfig` / `ISystemConfigProvider` pattern
- No new UI required — the existing activity picker page surfaces the result without changes

## Non-Goals

- Streaming (chunked/SSE) responses — deferred
- Embeddings endpoint — deferred until a feature needs it
- Frontend-callable proxy route for chat — Ollama stays server-side only
- Warning/notification system for AI failures — separate ideation backlog item

## User Stories / Use Cases

- **As a HomeHub user**, I want the activity picker to use AI to choose from my activity list with awareness of recent picks, so that selections feel more intentional and varied than pure random.
- **As a developer adding an AI-powered feature**, I want an injectable `OllamaClient` already wired up with config and error handling, so I don't need to re-implement HTTP infrastructure for each new feature.
- **As a HomeHub operator**, I want to switch activity selection between random and AI via a config value, so I can enable or disable AI without a code change or restart.

## Proposed Solution

1. Add `IOllamaClient` / `OllamaClient` in the Logic layer — a thin, typed `HttpClient` wrapper that calls Ollama's `/api/chat` endpoint (non-streaming).
2. Define `IActivitySelector` with a single `SelectAsync` method. Extract the existing weighted-random logic into `RandomActivitySelector`. Implement `AiActivitySelector` that calls Ollama, validates the response against the activity list, and falls back to random on any failure.
3. Refactor `ActivityPickerJob` to inject `IActivitySelector` and fetch recent picks before delegating.
4. Add three new `SystemConfig` seed entries and a factory registration in `Program.cs`.

## Technical Approach

### OllamaClient

- **Location**: `backend/Logic/Ollama/OllamaClient.cs`
- **Interface**: `IOllamaClient` in `backend/Common/Interfaces/`
- **Method**: `Task<string> ChatAsync(string model, string prompt)`
- **Pattern**: follows `GpuServiceClient` — typed `HttpClient`, reads `ollama::url` from `ISystemConfigProvider` at call time
- **Auth**: none (Ollama is unauthed on the LAN)
- **Timeout**: 60 seconds, set at `AddHttpClient<OllamaClient>()` registration in `Program.cs`
- **Request format**: `POST /api/chat` with `{ model, messages: [{ role: "user", content: prompt }], stream: false }`
- **Response**: extract `message.content` string from Ollama's JSON response

### IActivitySelector

```csharp
// backend/Common/Interfaces/IActivitySelector.cs
public interface IActivitySelector
{
    Task<Activity> SelectAsync(IList<Activity> activities, IList<ActivityPick> recentPicks);
}
```

### RandomActivitySelector

- Moves the existing weighted-random algorithm verbatim from `ActivityPickerJob.WeightedRandom()`
- Location: `backend/Logic/ActivityPicker/RandomActivitySelector.cs`

### AiActivitySelector

- Location: `backend/Logic/ActivityPicker/AiActivitySelector.cs`
- Injects `IOllamaClient`, `ISystemConfigProvider`, and `RandomActivitySelector`
- Reads `activity::ai_model` from config at call time
- Builds a prompt containing all activity names and the last `N×2` recent pick names
- Requires the model to return **exactly** one activity name from the list
- Validates the response is an exact match against the activity list (case-insensitive trim)
- On any failure (network error, timeout, unrecognized response), logs a warning and falls back to `RandomActivitySelector.SelectAsync`

### ActivityPickerJob changes

1. Remove `WeightedRandom()` — now in `RandomActivitySelector`
2. Inject `IActivitySelector`
3. After fetching all activities (count = N), call `repository.GetRecentAsync(N * 2)` to retrieve context picks
4. Call `selector.SelectAsync(activities, recentPicks)`

### IActivityPickRepository addition

Add to the interface and both implementations (`EfActivityPickRepository`, `InMemoryActivityPickRepository`):

```csharp
Task<IEnumerable<ActivityPick>> GetRecentAsync(int count);
```

Returns the most recent `count` picks ordered by `PickedAt` descending.

### Configuration

New seed entries added to `Program.cs`:

| Config Key | Default | Notes |
|---|---|---|
| `ollama::url` | `""` | Must be set per environment; empty = feature non-functional |
| `activity::selector` | `"random"` | AI is opt-in; `"ai"` to enable |
| `activity::ai_model` | `"llama3.2"` | Per-feature model name |

### DI Registration (follows audiobook pattern)

```csharp
builder.Services.AddHttpClient<OllamaClient>()
    .ConfigureHttpClient(c => c.Timeout = TimeSpan.FromSeconds(60));

builder.Services.AddScoped<RandomActivitySelector>();
builder.Services.AddScoped<AiActivitySelector>();
builder.Services.AddScoped<IActivitySelector>(sp =>
{
    var cfg = sp.GetRequiredService<ISystemConfigProvider>();
    var key = cfg.GetAsync("activity", "selector").GetAwaiter().GetResult().Value;
    return key == "ai"
        ? sp.GetRequiredService<AiActivitySelector>()
        : sp.GetRequiredService<RandomActivitySelector>();
});
```

### Key Decisions

| Decision | Chosen Approach | Rationale |
|---|---|---|
| Ollama endpoints in scope | Chat completions only | Embeddings require a vector store; not needed yet |
| Streaming | Deferred | Most planned use cases don't need fast time-to-first-token |
| Client accessibility | Server-side only | Ollama stays on private LAN; no proxy route needed |
| Auth | None | Ollama is unauthed; HomeHub handles its own auth |
| Selector switching | `activity::selector` config value | Consistent with `audiobook::provider` pattern; swappable without restart |
| Model config | Per-feature (`activity::ai_model`) | Different features may need different models |
| AI failure handling | Silent fallback to random | Enhancement, not critical path; no error surfaced to user |
| Recent pick window | `N × 2` (N = unique activity count) | Ensures every activity has appeared at least twice in context |
| Timeout | 60 s hardcoded | Long enough for local inference; not user-configurable |

### Dependencies

- Ollama running on the home LAN, reachable from the backend host
- Existing `ISystemConfigProvider` / `SystemConfig` infrastructure
- Existing `IActivityPickRepository` (extended with `GetRecentAsync`)
- Existing Hangfire job scheduler for `ActivityPickerJob`

## Open Questions

_None — all major decisions resolved during design session._

## Out of Scope

- Streaming / SSE responses
- Ollama embeddings endpoint
- Frontend chat proxy endpoint (`POST /api/ai/chat`)
- Warning log table or notification system for AI failures (to be ideated separately)
- Global/shared model config — model selection is per-feature

## Success Metrics

- `OllamaClient` successfully calls a running Ollama instance and returns a completion string
- Activity picker switches between random and AI selector by changing `activity::selector` config value — no code change or restart required
- `AiActivitySelector` falls back to random when Ollama is unreachable with no error surfaced in the UI or API response
- All new classes covered by unit tests following the existing `FakeHandler` / mock patterns

## Timeline / Milestones

_TBD_
