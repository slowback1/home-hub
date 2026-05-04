# Add IWeatherProvider Interface and MockWeatherProvider

## Status

`pending`

## Description

Define the `IWeatherProvider` contract and the `WeatherConditions` response record that all provider implementations will return. Implement `MockWeatherProvider` with hard-coded current conditions data — this is the default provider used in E2E and out-of-the-box without an API key.

## Acceptance Criteria

- [ ] `IWeatherProvider` interface exists in `Common.Interfaces` with a single method: `Task<WeatherConditions> GetCurrentAsync(string zipCode, string units)`
- [ ] `WeatherConditions` record exists with properties: `Temperature` (double), `ConditionLabel` (string), `HumidityPercent` (int), `WindSpeed` (double), `Units` (string)
- [ ] `MockWeatherProvider` exists, implements `IWeatherProvider`, and returns deterministic hard-coded data (stable values suitable for E2E assertions)
- [ ] Unit tests cover `MockWeatherProvider.GetCurrentAsync` for both `"imperial"` and `"metric"` units inputs
- [ ] `dotnet build` passes with no errors or warnings

## Notes

`IWeatherProvider` intentionally has no `GetForecastAsync` method — forecast is deferred to a future PRD (see `weather-forecast` stub).

Place `IWeatherProvider` and `WeatherConditions` in `Common.Interfaces` alongside `ISystemConfigProvider` and `IFeatureFlagProvider`. Place `MockWeatherProvider` in the `Logic` project alongside other provider implementations.

The hard-coded values returned by `MockWeatherProvider` should be specific enough for E2E assertions (e.g. a fixed temperature like `72.0`, a fixed label like `"Sunny"`) rather than random or environment-dependent.
