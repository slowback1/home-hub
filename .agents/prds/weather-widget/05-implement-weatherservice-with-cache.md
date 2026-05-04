# Implement WeatherService with 5-Minute In-Memory Cache

## Status

`done`

## Description

Implement `WeatherService`, which reads the configured zip code and units from `ISystemConfigProvider`, delegates to `IWeatherProvider`, and caches results in `IMemoryCache` for 5 minutes. This is the single point of entry for all weather data in the application.

## Acceptance Criteria

- [ ] `WeatherService` exists in the `Logic` project and exposes `Task<WeatherConditions> GetCurrentAsync()`
- [ ] Reads `weather::zip_code` and `weather::units` from `ISystemConfigProvider` on each call
- [ ] Cache key includes both zip code and units so a units change produces a fresh fetch
- [ ] Cache TTL is 5 minutes (`IMemoryCache` absolute expiration)
- [ ] On cache miss, delegates to `IWeatherProvider.GetCurrentAsync(zipCode, units)` and stores the result
- [ ] Exceptions from `IWeatherProvider` propagate to the caller (not swallowed)
- [ ] Unit tests cover: cache hit returns cached value without calling provider, cache miss calls provider and caches result, provider exception propagates
- [ ] `dotnet build` passes with no errors or warnings

## Notes

`IMemoryCache` is part of `Microsoft.Extensions.Caching.Memory` — no extra NuGet package required. Registration in DI (`services.AddMemoryCache()`) is handled in task 06.

`WeatherService` should not know which provider implementation is active — it depends only on `IWeatherProvider`. Provider selection is a DI concern (task 06).
