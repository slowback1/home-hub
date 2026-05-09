# OllamaClient Infrastructure

## Status

`done`

## Description

Define the `IOllamaClient` interface in the Common layer and implement `OllamaClient` in Logic as a typed `HttpClient` wrapper. The client calls Ollama's `/api/chat` endpoint in non-streaming mode and returns the model's reply as a plain string. The base URL is read from `ISystemConfigProvider` at call time (key: `ollama::url`). No auth header is sent — Ollama runs unauthed on the LAN.

## Acceptance Criteria

- [ ] `IOllamaClient` interface exists in `backend/Common/Interfaces/` with method `Task<string> ChatAsync(string model, string prompt)`
- [ ] `OllamaClient` in `backend/Logic/Ollama/` implements `IOllamaClient` as a typed `HttpClient`
- [ ] Sends `POST /api/chat` with body `{ "model": "...", "messages": [{ "role": "user", "content": "..." }], "stream": false }`
- [ ] Extracts and returns `message.content` from the Ollama JSON response
- [ ] Reads base URL from `ISystemConfigProvider` using namespace `"ollama"`, key `"url"` at call time
- [ ] No `Authorization` header is added
- [ ] Unit tests cover: successful response returns content string, HTTP error throws, malformed JSON throws
- [ ] Tests follow the existing `FakeHandler` pattern used in `GpuServiceClientTests`

## Notes

- Follow `GpuServiceClient` (`backend/Logic/Audiobook/GpuServiceClient.cs`) as the implementation template
- Ollama's non-streaming chat response shape: `{ "message": { "role": "assistant", "content": "..." }, ... }`
- Use `JsonNamingPolicy.SnakeCaseLower` for deserialization (consistent with existing clients)
- Registration with 60 s timeout happens in task 05 — this task only defines the class and interface
