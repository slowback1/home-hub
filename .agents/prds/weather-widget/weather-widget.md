# PRD: Weather Widget

## Status

`Draft`

## Overview

A weather widget page at `/weather` that displays current conditions (temperature, conditions, humidity, wind speed) for a configured location. Weather data is fetched lazily via a backend proxy — protecting the API key — and cached in-memory for 5 minutes to avoid hammering the upstream API. The active weather provider is selected via a SystemConfig entry (`weather::provider`), defaulting to a mock implementation that returns hard-coded data. A real OpenWeatherMap implementation is available once an API key is configured.

## Problem Statement

A quick glance at current weather is useful ambient information on a personal home hub, without switching to a separate app or tab. The `/weather` route currently shows "Coming soon." This PRD delivers the full implementation.

## Goals

- Display current weather conditions (temperature, condition label, humidity, wind speed) at `/weather`
- Support configurable units (imperial / metric) via `weather::units` SystemConfig
- Hide the weather API implementation behind `IWeatherProvider` so providers are swappable
- Ship a `MockWeatherProvider` (hard-coded data) as the default provider, safe to use without an API key
- Ship an `OpenWeatherMapProvider` as the production provider, selected via `weather::provider`
- Cache backend weather responses for 5 minutes (in-memory) to reduce upstream API calls
- Show a clear "Weather unavailable" fallback state on API errors
- Gate the page behind the existing `WEATHER_ENABLED` feature flag

## Non-Goals

- Multi-day or hourly forecast (deferred; see weather-forecast stub)
- Embeddable component for a future dashboard (deferred to dashboard PRD)
- Location selection from the UI (location is config-only via `weather::zip_code`)
- Other weather API providers beyond Mock and OpenWeatherMap

## User Stories / Use Cases

- **As a** home hub user, **I want to** open the weather page and immediately see current conditions, **so that** I know what to expect outside without leaving the hub.
- **As a** home hub user, **I want** the page to show a clear error message if weather data is unavailable, **so that** I know the widget isn't working rather than seeing stale or empty data.
- **As a** home hub admin, **I want to** switch between the mock and real provider in System Config, **so that** I can test the feature before obtaining an API key.
- **As a** home hub admin, **I want** the units (°F/°C, mph/km/h) to respect my System Config preference, **so that** the display matches my local convention.

## E2E Scenarios

```gherkin
@weather
Feature: Weather Widget

  @weather-displays-current-conditions
  Scenario: Weather page displays current conditions using mock provider
    Given the weather provider is "mock"
    When I navigate to the weather page
    Then I should see a temperature value
    And I should see a condition description
    And I should see a humidity value
    And I should see a wind speed value

  @weather-unavailable-state
  Scenario: Weather page shows unavailable message when the backend returns an error
    Given the weather provider is "mock"
    When I navigate to the weather page and the weather API returns an error
    Then I should see a "Weather unavailable" message

  @weather-feature-flag-hidden
  Scenario: Weather page is not accessible when the feature flag is disabled
    Given the "WEATHER_ENABLED" feature flag is disabled
    When I navigate to the weather page
    Then I should be redirected or see a not-found state
```

## Proposed Solution

### Backend

**`IWeatherProvider` interface** (in `Common.Interfaces`):

```csharp
public interface IWeatherProvider
{
    Task<WeatherConditions> GetCurrentAsync(string zipCode, string units);
}

public record WeatherConditions(
    double Temperature,
    string ConditionLabel,
    int HumidityPercent,
    double WindSpeed,
    string Units // "imperial" | "metric"
);
```

**`MockWeatherProvider`** — returns hard-coded `WeatherConditions`. Used in E2E and as the default out-of-the-box provider.

**`OpenWeatherMapProvider`** — calls `api.openweathermap.org/data/2.5/weather` with the configured zip code, units, and API key. Registered when `weather::provider = "openweathermap"`.

**`WeatherService`** — wraps `IWeatherProvider`, applies 5-minute `IMemoryCache` TTL. Reads `weather::zip_code` and `weather::units` from `ISystemConfigProvider` on each call (cache key includes zip + units).

**`WeatherController`** — `GET /api/weather/current` calls `WeatherService.GetCurrentAsync()` and returns the result as JSON. Returns `503 Service Unavailable` on provider error.

**DI registration** — `weather::provider` is read from `ISystemConfigProvider` at startup to select `MockWeatherProvider` or `OpenWeatherMapProvider`.

**EF migration** — seeds new SystemConfig rows:

| Namespace | Key | Value | Type | IsSecret |
|-----------|-----|-------|------|----------|
| weather | provider | mock | select | false |
| weather | units | imperial | select | false |

Populates `SystemConfigOptions`:

| SystemConfigId | Value | Label |
|----------------|-------|-------|
| weather::provider | mock | Mock |
| weather::provider | openweathermap | Open Weather Map |
| weather::units | imperial | Imperial (°F, mph) |
| weather::units | metric | Metric (°C, km/h) |

Also updates `Type` on existing weather rows (`weather::zip_code` → `text`, `weather::api_key` → `secret`).

### Frontend

**`/weather` page** — loads `GET /api/weather/current` on mount. Renders:
- Temperature (with unit label)
- Condition label
- Humidity (%)
- Wind speed (with unit label)
- Loading state (spinner) while fetching
- "Weather unavailable" message on error (503 or network failure)

**Feature flag guard** — page checks `WEATHER_ENABLED` flag via `FeatureFlagService`; redirects or renders a not-found state if disabled.

**`WeatherApi`** — new API client extending `BaseApi`, single `getCurrent()` method calling `GET /api/weather/current`.

## Technical Approach

### Key Decisions

| Decision | Chosen Approach | Rationale |
|----------|----------------|-----------|
| Provider selection | `weather::provider` SystemConfig key, read at startup | Consistent with `CrudFactory.Implementation` pattern already in the codebase |
| Default provider | `MockWeatherProvider` | Works without an API key; safe out-of-the-box default |
| Caching | `IMemoryCache`, 5-minute TTL, keyed on zip+units | Simple, no infrastructure needed; TTL is sensible for personal use |
| Fetch strategy | Lazy / on-demand (no Hangfire job) | Low traffic; ~200ms cold-cache latency is imperceptible |
| Error handling | `503` from backend → "Weather unavailable" UI state | Honest, simple; no stale-data persistence needed |
| Units | `weather::units` SystemConfig (imperial / metric), default `imperial` | Configurable without code change; select field via Enhanced SystemConfig PRD |
| Location | `weather::zip_code` SystemConfig (existing row) | Already seeded; no new config needed |
| API key storage | `weather::api_key` SystemConfig (existing row, `IsSecret = true`) | Already seeded and secret-masked in admin UI |
| E2E provider | `MockWeatherProvider` (selected via `weather::provider = "mock"` in E2E seed) | No real API key needed; hard-coded data is deterministic |
| Forecast interface | Not stubbed on `IWeatherProvider` | Forecast is a separate PRD; interface kept minimal |

### Dependencies

- **Enhanced System Config PRD** (prerequisite): `select`-type fields, `SystemConfigOptions` table — required for `weather::provider` and `weather::units` dropdowns on admin page
- `ISystemConfigProvider` (completed): reads `weather::zip_code`, `weather::api_key`, `weather::provider`, `weather::units`
- `IMemoryCache` (standard .NET, already available)
- `FeatureFlagService` (frontend, completed): `WEATHER_ENABLED` flag check
- Existing `BaseApi`, `Spinner`, `Heading` UI components

## Open Questions

_None._

## Out of Scope

- Forecast (hourly / multi-day) — see `weather-forecast` stub
- Embeddable dashboard component — deferred to dashboard PRD
- Other weather API providers
- Location search / geocoding UI

## Success Metrics

- All three E2E scenarios pass
- Navigating to `/weather` with `provider = mock` shows hard-coded current conditions within 1 second
- Navigating to `/weather` with a backend error shows "Weather unavailable" (not a blank page)
- The existing System Config E2E scenarios continue to pass

## Timeline / Milestones

_TBD_
