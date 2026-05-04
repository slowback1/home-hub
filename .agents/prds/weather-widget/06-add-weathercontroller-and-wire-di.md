# Add WeatherController and Wire DI Registration

## Status

`done`

## Description

Expose `GET /api/weather/current` via a new `WeatherController` that delegates to `WeatherService`. Wire all DI registrations: select the active `IWeatherProvider` implementation based on `weather::provider` from SystemConfig, register `WeatherService`, register `IMemoryCache`, and register the `HttpClient` for `OpenWeatherMapProvider`.

## Acceptance Criteria

- [ ] `WeatherController` exists at `GET /api/weather/current` and returns the `WeatherConditions` payload as JSON on success
- [ ] Returns `503 Service Unavailable` with a plain-text error message when `WeatherService` throws
- [ ] DI registration reads `weather::provider` from `ISystemConfigProvider` at startup and registers either `MockWeatherProvider` or `OpenWeatherMapProvider` as `IWeatherProvider`
- [ ] `services.AddMemoryCache()` is called if not already registered
- [ ] `HttpClient` for `OpenWeatherMapProvider` is registered via `IHttpClientFactory` or typed client
- [ ] `WeatherService` is registered in DI (scoped or singleton as appropriate)
- [ ] `GET /api/weather/current` returns a valid JSON response when the backend is running with `weather::provider = "mock"`
- [ ] `dotnet build` passes with no errors or warnings

## Notes

Read `weather::provider` at startup (not per-request) to keep DI registration straightforward. If the config value is missing or unrecognised, default to `MockWeatherProvider` and log a warning.

The response shape should match `WeatherConditions` directly — no additional wrapping DTO needed.

Verify the endpoint manually with `curl http://localhost:5273/api/weather/current` after wiring.
