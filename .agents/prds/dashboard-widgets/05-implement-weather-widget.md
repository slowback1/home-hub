# Implement WeatherWidget

## Status

`done`

## Description

Replace the `WeatherWidget.svelte` placeholder with a real implementation that fetches current weather conditions, displays them according to the design, and polls every hour for fresh data.

## Acceptance Criteria

- [ ] On mount, the widget calls `WeatherApi.getCurrent()` and shows a spinner during the request.
- [ ] On success, the widget displays:
  - Temperature and condition label centered horizontally and vertically in the upper body area.
  - Humidity and wind speed in a two-column secondary stat row (`<dl>`) pinned to the bottom of the card body, separated from the primary area by a hairline.
  - Temperature is formatted as `{value}{unit symbol}` (°C or °F depending on `units`); wind speed includes the unit label (km/h or mph).
- [ ] On error, the widget shows the shared error state: a muted `—` dash and italic "Unavailable" text.
- [ ] The widget polls `WeatherApi.getCurrent()` every 1 hour; the interval is cleared when the component is destroyed.
- [ ] No retry button is present.

## Notes

- `WeatherApi` is at `src/lib/api/WeatherApi.ts`; it exposes `getCurrent(): Promise<WeatherConditions>` where `WeatherConditions = { temperature, conditionLabel, humidityPercent, windSpeed, units }`.
- Match the design from the handoff: `https://api.anthropic.com/v1/design/h/AtzNT0hwaDz9B5lJWPHL4w?open_file=Widget+Bodies.html` — temperature + condition centered, stats pinned bottom.
- The card header (icon, label, chevron) is rendered by the dashboard page, not the widget — implement only the body area.
- Loading and error state visual pattern is shared across all widgets: spinner + "Loading…" label; `—` + "Unavailable" label respectively.
- 1-hour interval in ms: `3_600_000`.
