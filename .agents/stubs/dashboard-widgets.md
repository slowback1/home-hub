# Dashboard Widgets

**Status:** stub
**Created:** 2026-05-03

## Summary

Slimmed-down widget components for each completed feature, designed to slot into the customizable dashboard grid.

## Problem / Opportunity

The dashboard skeleton needs actual content. Each completed feature should expose a compact read-only (or light-interaction) view that gives the user a useful at-a-glance summary without replicating the full feature UI.

## Success Looks Like

- At least one widget exists for each feature that is "done" at the time this is built
- Each widget is visually consistent: same card chrome, padding, and heading style
- Widgets are read-only or minimally interactive (e.g. a quick-tap spit counter increment is fine; full config is not)
- Clicking a widget navigates to the full feature page
- Adding a new widget for a future feature requires minimal boilerplate

## Notes & Open Questions

- **Depends on:** customizable-dashboard-skeleton stub
- Candidate widgets at time of writing: Activity tracker summary, Spit Counter current tally, Weather current conditions, Chore tasks due today, RetroAchievements random game picker
- Widget registration pattern: auto-discovery by convention, or an explicit registry?
- Should widgets fetch their own data, or receive it as props from a parent that batches requests?
- Stale/error state: what does a widget show if its data endpoint is down?
- Some features (e.g. epub-to-audiobook, feature-flags admin) may not make sense as dashboard widgets — decide per feature at implementation time
