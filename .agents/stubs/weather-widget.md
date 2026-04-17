# Weather Widget

**Status:** stub
**Created:** 2026-04-17

## Summary

A simple weather widget that displays current weather conditions for a location defined in app config, powered by a third-party weather API.

## Problem / Opportunity

A quick glance at current weather is a useful ambient piece of information to have on a personal hub dashboard, without needing to open a separate app or browser tab.

## Success Looks Like

- The widget displays current weather (temperature, conditions, maybe wind/humidity) for the configured location
- Location is set via config or environment variable — no UI required to change it
- The widget handles API errors gracefully (e.g. shows stale data or a fallback state)
- API calls are reasonably cached so the weather API isn't hammered on every page load

## Notes & Open Questions

- Weather API provider preference? (OpenWeatherMap, WeatherAPI, Open-Meteo, etc.)
- API key storage — env var
- Units preference — imperial or metric, or user-configurable?
- Should the widget be a standalone page or an embeddable component for a future dashboard?
- Forecast (hourly/daily) in scope for v1, or current conditions only?
