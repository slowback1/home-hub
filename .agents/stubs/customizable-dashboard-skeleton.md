# Customizable Dashboard Skeleton

**Status:** stub
**Created:** 2026-05-03

## Summary

A configurable overview page with a widget grid layout where the user can choose which feature components appear and where.

## Problem / Opportunity

As the number of features grows, navigating to each page individually becomes inefficient. A single overview page with at-a-glance summaries of the most relevant features would surface key info without hunting through the nav. The user should control which widgets appear and their positions.

## Success Looks Like

- A `/` or `/dashboard` route renders a grid of widget slots
- The user can configure which widgets occupy which slots (add, remove, reorder)
- Layout preferences persist across sessions (stored in DB or user config)
- The grid degrades gracefully when a widget's underlying feature is unavailable or not yet built
- The system makes it easy to register new widget types as future features are completed

## Notes & Open Questions

- **Prerequisite for:** dashboard-widgets stub — widgets need the grid to render in
- Grid system: fixed-size slots vs. freeform drag-and-drop (react-grid-layout or simpler CSS grid + config)?
- Where is layout config stored? User config table in the DB, or a config file?
- Is there a single shared layout or per-user (currently a single-user app, but worth deciding)?
- Should widgets be collapsible or resizable, or just placeable/removable?
- Mobile layout: stacked single-column, or separate mobile config?
