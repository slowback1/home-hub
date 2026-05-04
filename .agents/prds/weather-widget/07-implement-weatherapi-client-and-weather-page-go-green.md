# Implement WeatherApi Client and /weather Page, Go GREEN

## Status

`pending`

## Description

Add the frontend `WeatherApi` client and implement the `/weather` page with current conditions display, loading state, and "Weather unavailable" error state. Gate the page behind the `WEATHER_ENABLED` feature flag. Drive all three E2E scenarios to GREEN.

## Acceptance Criteria

- [ ] `WeatherApi` exists in `frontend/src/lib/api/`, extends `BaseApi`, and exposes a `getCurrent()` method calling `GET /api/weather/current`
- [ ] `/weather` page displays on load: temperature (with unit label), condition label, humidity (%), wind speed (with unit label)
- [ ] A spinner is shown while the fetch is in-flight
- [ ] When `GET /api/weather/current` returns an error (any non-2xx), the page shows a "Weather unavailable" message instead of conditions
- [ ] Page checks `WEATHER_ENABLED` via `FeatureFlagService`; renders a not-found/disabled state if the flag is off
- [ ] `WeatherPage.ts` page object methods are fully implemented (replacing task 01 stubs)
- [ ] `task e2e:test` targeting `weather-widget.feature` — all three scenarios pass GREEN
- [ ] All existing E2E scenarios continue to pass GREEN

## Notes

The error-state scenario uses Playwright route interception to mock `GET /api/weather/current` returning 500 — implement the interception in the step definition or `WeatherPage.ts`, not in the backend.

The feature-flag scenario should restore `WEATHER_ENABLED` to `true` after the test via an `After` hook so it does not bleed into other scenarios.

Unit labels: imperial → `°F` / `mph`; metric → `°C` / `km/h`. The page derives the label from the `units` field on the `WeatherConditions` response — no separate config call needed from the frontend.

The `/weather` route and `<title>HomeHub — Weather</title>` already exist in the placeholder page — replace its contents rather than creating a new route.
