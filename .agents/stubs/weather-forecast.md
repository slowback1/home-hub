# Weather Forecast

**Status:** stub
**Created:** 2026-05-04

## Summary

Extend the weather widget with a multi-day forecast view, showing high/low temperatures and conditions for the next several days.

## Problem / Opportunity

The current weather widget shows only current conditions. A short forecast (3–5 days) adds meaningful value for planning without significantly increasing complexity.

## Success Looks Like

- The weather page (or a dedicated section) shows a multi-day forecast alongside current conditions
- Forecast data comes from the same provider abstraction (`IWeatherProvider`) as current conditions
- The feature is additive — current-conditions display is unchanged

## Notes & Open Questions

- Scope: how many days? (3, 5, 7?)
- Layout: inline below current conditions, or a separate tab/section?
- Granularity: daily high/low only, or hourly breakdown?
- OpenWeatherMap free tier supports 5-day / 3-hour forecast — is that sufficient?
