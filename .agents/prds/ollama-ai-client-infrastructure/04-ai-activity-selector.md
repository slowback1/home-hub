# AiActivitySelector

## Status

`done`

## Description

Implement `AiActivitySelector`, which uses `IOllamaClient` to ask a local model to pick an activity from the configured list. The selector reads the model name from `ISystemConfigProvider` (`activity::ai_model`), builds a prompt containing all activity names and the most recent picks, and validates that the model's response exactly matches one of the activity names. On any failure — network error, timeout, unrecognized response — it falls back silently to `RandomActivitySelector` and logs a warning.

## Acceptance Criteria

- [ ] `AiActivitySelector` in `backend/Logic/ActivityPicker/` implements `IActivitySelector`
- [ ] Reads model name from `ISystemConfigProvider` using namespace `"activity"`, key `"ai_model"` at call time
- [ ] Prompt includes all activity names (one per line) and the names from `recentPicks` as context
- [ ] Response is trimmed and compared case-insensitively against activity names; matched activity is returned
- [ ] If the response does not match any activity name, logs a warning and falls back to `RandomActivitySelector`
- [ ] If `IOllamaClient.ChatAsync` throws (any exception), logs a warning and falls back to `RandomActivitySelector`
- [ ] Fallback produces a valid activity — no exception is ever surfaced to the caller
- [ ] Unit tests cover: happy path returns correct activity, response with extra whitespace is matched, unrecognized response falls back to random, `ChatAsync` exception falls back to random

## Notes

- Inject: `IOllamaClient`, `ISystemConfigProvider`, `RandomActivitySelector`, `ILogger<AiActivitySelector>`
- The prompt should instruct the model clearly: give it the activity list, the recent history, and an explicit instruction to reply with exactly one name from the list and nothing else — this minimizes parsing failures
- `RandomActivitySelector` is injected directly (not via `IActivitySelector`) to avoid circular DI ambiguity
