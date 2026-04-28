# System Config Infrastructure

**Status:** stub
**Created:** 2026-04-28

## Summary

A database-backed key/value store for runtime application configuration, replacing the current config-file approach for everything except infrastructure-critical settings (DB connection, CORS).

## Problem / Opportunity

Config file-based settings require a redeploy to change and can't be managed by an admin at runtime. Most application config (display preferences, integration settings, thresholds, etc.) doesn't need to be tied to the file system and would be better managed in the database.

## Success Looks Like

- A `system_config` DB table stores typed key/value pairs
- A service/API layer supports reading and writing config values
- Infrastructure-critical settings (DB URL, CORS origins) remain in the config file; everything else can live in the DB
- The config API is usable by other features (e.g. feature flags UI, weather widget location)

## Notes & Open Questions

- What types need to be supported? (string, number, boolean, JSON?)
- Should values be namespaced/grouped (e.g. `integrations.weather.api_key`)?
- Should there be a default-value mechanism so the app works before a value is explicitly set?
- Consider whether secrets (API keys) should be stored here or kept in env vars
