# Implement OpenWeatherMapProvider

## Status

`pending`

## Description

Implement the production `IWeatherProvider` that calls the OpenWeatherMap current weather API. Reads the API key from `ISystemConfigProvider` at call time, maps the JSON response to `WeatherConditions`, and propagates errors so `WeatherService` can handle them.

## Acceptance Criteria

- [ ] `OpenWeatherMapProvider` exists in the `Logic` project and implements `IWeatherProvider`
- [ ] Calls `https://api.openweathermap.org/data/2.5/weather` with `zip`, `units`, and `appid` query parameters
- [ ] Maps the response fields to `WeatherConditions`: temperature from `main.temp`, condition label from `weather[0].description`, humidity from `main.humidity`, wind speed from `wind.speed`
- [ ] Reads `weather::api_key` from `ISystemConfigProvider` at call time (not cached at startup)
- [ ] Throws a descriptive exception on non-2xx HTTP responses or network errors (caller handles)
- [ ] `HttpClient` is injected (not instantiated directly) to enable unit testing
- [ ] Unit tests cover: successful response mapping, non-2xx response throws, missing/empty API key behaviour
- [ ] `dotnet build` passes with no errors or warnings

## Notes

Register `HttpClient` for `OpenWeatherMapProvider` via `IHttpClientFactory` or a typed client in DI — wired in task 06.

The `units` parameter passed to the API should be `"imperial"` or `"metric"` directly (OpenWeatherMap accepts these values as-is).

Do not cache results here — caching is `WeatherService`'s responsibility (task 05).
