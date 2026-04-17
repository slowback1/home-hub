# Random Task Picker

**Status:** stub
**Created:** 2026-04-17

## Summary

A feature where the user defines a list of possible daily activities, a background job picks one at random each hour, and a page displays the current pick.

## Problem / Opportunity

Decision fatigue around what to do with free time is real. A randomizer that surfaces one activity per hour removes the friction of choosing and adds a bit of fun structure to unstructured time.

## Success Looks Like

- The user can define and manage a list of activities (e.g. "program", "play video games", "do chores")
- A background process runs once per hour and randomly selects an activity from the list
- A page in HomeHub displays the currently selected activity
- The page updates to reflect the latest hourly pick without requiring a manual refresh

## Notes & Open Questions

- Depends on: `database-infrastructure` stub
- Activity list management — simple config file, or a CRUD UI in the app?
- Should activities have weights (e.g. make "do chores" less likely)?
- Background job mechanism — cron, a job queue, or a simple scheduled server process?
- Should pick history be stored and viewable?
- What happens if the activity list is empty?
