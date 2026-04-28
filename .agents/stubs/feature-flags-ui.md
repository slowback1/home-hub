# Feature Flags Admin UI

**Status:** stub
**Created:** 2026-04-28

## Summary

An admin UI for toggling feature flags at runtime, surfacing the existing feature flag infrastructure through a management page.

## Problem / Opportunity

Feature flags already exist in the codebase but likely require direct DB or code changes to toggle. An admin UI makes flag management accessible without touching code or data directly.

## Success Looks Like

- An admin page lists all known feature flags and their current enabled/disabled state
- Each flag can be toggled on or off with a single action
- Changes take effect immediately at runtime

## Notes & Open Questions

- Depends on existing feature flag infrastructure (flags already exist in the codebase)
- Could live alongside the system config admin page (`system-config-ui`) or be a separate section
- Should flags support more than a boolean? (e.g. percentage rollout, per-user targeting)
- Who has access — any logged-in user, or a specific admin role?
