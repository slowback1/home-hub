# Database Infrastructure

**Status:** stub
**Created:** 2026-04-17

## Summary

Wire up a real database with connection management, schema migrations, and a query layer so future features have a reliable persistence foundation.

## Problem / Opportunity

Without a real database, features are limited to ephemeral or file-based storage. Establishing this infrastructure early means every future feature can rely on a consistent, well-managed data layer rather than bolting on persistence ad hoc.

## Success Looks Like

- Application connects to a real database (not SQLite/in-memory) on startup
- Schema migrations are version-controlled and run automatically on deploy
- A query layer (ORM or query builder) is available for feature work to build on
- Local development setup is documented and reproducible

## Notes & Open Questions

- Database engine preference? (PostgreSQL is a reasonable default)
- ORM vs. query builder vs. raw SQL?
- Where does the database run — same host as the app, separate container, NAS?
- Seeding strategy for local development?
