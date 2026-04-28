# System Config Admin UI

**Status:** stub
**Created:** 2026-04-28

## Summary

An admin page for viewing and editing database-backed system configuration values at runtime, without requiring a redeploy.

## Problem / Opportunity

Once config lives in the database (see `system-config-infrastructure`), there needs to be a way to read and update it without direct DB access. An admin UI page makes this safe and discoverable.

## Success Looks Like

- A settings/admin page lists all current config keys and their values
- Each value can be edited in place and saved
- Changes take effect at runtime without restarting the application
- Read-only display for any config values that are sourced from files (infra-critical settings)

## Notes & Open Questions

- Depends on `system-config-infrastructure` stub
- Should config values be grouped/categorized visually?
- Are any values sensitive enough to warrant masking (e.g. API keys)?
- Who has access — any logged-in user, or a specific admin role?
