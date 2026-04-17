# Chore / Task Tracker

**Status:** stub
**Created:** 2026-04-17

## Summary

A personal task tracker that handles both one-off tasks (e.g. "plan vacation") and recurring tasks (e.g. "sweep floors"), rewritten from a previous project with improved functionality.

## Problem / Opportunity

The previous version of this tool lacked polish and functionality. Rewriting it into HomeHub is an opportunity to build a more capable tracker that handles recurrence properly, lives alongside other features, and benefits from the shared infrastructure (DB, UI design system).

## Success Looks Like

- One-off tasks can be created, completed, and archived
- Recurring tasks can be defined with a recurrence schedule (daily, weekly, custom interval)
- Recurring tasks automatically regenerate after completion
- A clear UI distinguishes one-off from recurring tasks and shows what's due
- Data is persisted in the real database

## Notes & Open Questions

- Depends on: `database-infrastructure` stub
- Recurrence rule format — simple interval (every N days) or full cron/iCal-style rules?
- Should completed recurring tasks show history, or just reset silently?
- Any notion of priority, due dates, or tags in scope for v1?
- Categories or grouping (e.g. household, personal, work)?
- Notifications or reminders out of scope for now?
