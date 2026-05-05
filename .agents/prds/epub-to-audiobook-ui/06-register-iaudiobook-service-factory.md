# Register IAudiobookService Factory in DI

## Status

`done`

## Description

Wire `IAudiobookService` into the .NET dependency injection container using a scoped factory that reads `audiobook::provider` from system config and returns either `MockAudiobookService` or `GpuServiceClient`. Mirrors the factory pattern used for `IWeatherProvider`.

## Acceptance Criteria

- [ ] `MockAudiobookService` and `GpuServiceClient` are both registered as scoped services in `Program.cs`
- [ ] `IAudiobookService` is registered as a scoped factory that reads `audiobook::provider` via `ISystemConfigProvider` and returns the matching implementation (`"mock"` → `MockAudiobookService`, `"gpu-service"` → `GpuServiceClient`), defaulting to `MockAudiobookService` if the config read fails
- [ ] `HttpClient` for `GpuServiceClient` is registered (e.g. via `AddHttpClient<GpuServiceClient>()` or a named client)
- [ ] Switching `audiobook::provider` in the System Config UI takes effect on the next request without a restart
- [ ] Project builds and starts with no DI errors

## Notes

Follow the factory pattern for `IWeatherProvider` in `Program.cs` (lines ~95–110) exactly — same structure, same fallback-to-mock on exception pattern.

`ISystemConfigProvider.GetAsync` is async; the factory lambda uses `.GetAwaiter().GetResult()` inside the synchronous DI factory delegate, matching the weather provider pattern.
