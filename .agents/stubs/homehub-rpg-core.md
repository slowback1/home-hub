# HomeHub RPG Core

**Status:** stub
**Created:** 2026-05-25

## Summary

A web-based idle RPG embedded in HomeHub with Dragon Quest-style combat, character stats, and a progression loop — self-contained game engine with no real-world integrations in this slice.

## Problem / Opportunity

HomeHub has several productivity features (chore tracker, walk sessions) that generate "good behavior" events. Tying those events to a game makes the habits more rewarding and gives the dashboard a fun, personal dimension. The core game needs to exist before any real-world progression can be wired in.

## Success Looks Like

- A dedicated `/rpg` route in the HomeHub frontend renders the game UI
- Player has a character with at least: HP, max HP, level, XP, XP-to-next-level, and one or two combat stats (e.g. attack, defense)
- Idle combat loop: enemies spawn on a timer, the character auto-attacks each tick, enemies drop XP on defeat, character levels up when XP threshold is reached
- Player can manually trigger an action (e.g. a special attack or use an item) — gives some agency without requiring constant attention
- Character state persists across page loads (backend-persisted, not localStorage)
- Basic game UI: character panel, enemy panel, combat log / event feed
- No real-world data wired in at this stage — XP sources are purely in-game (defeating enemies)

## Notes & Open Questions

- **"Incremental" vs "idle"**: Current direction is idle RPG — things happen automatically on a timer while you're away. Player interaction is optional/supplementary, not required to progress. This may evolve.
- **Combat model**: Dragon Quest-ish suggests turn-based or tick-based with simple hit/miss/damage math. A server-side tick (e.g. every N seconds, processed on request or via a background job) may be cleaner than client-side timers that drift.
- **Persistence**: Character state needs a backend model and CRUD. Likely a single `Character` record per user (single-player, one character).
- **Weather integration**: Floated as a potential mechanic (weather affects combat, cosmetic flavor, etc.) — not in scope for this stub, open question for later.
- **Depends on**: Nothing — this is the foundational slice. Chore tracker and step counter integrations layer on top of it.
