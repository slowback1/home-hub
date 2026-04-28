# Spit Counter

**Status:** stub
**Created:** 2026-04-28

## Summary

A baseball-themed click counter page ported from the standalone spit-counter app, where pressing Space (or clicking the baseball) increments a spit count with visual overlays accumulating on the ball.

## Problem / Opportunity

The spit-counter was a standalone side project that fits naturally as a fun, self-contained page within HomeHub. Porting it consolidates it into the main app rather than maintaining a separate deployment.

## Success Looks Like

- A `/spit-counter` page exists in HomeHub with the baseball SVG UI
- Space bar increments the count; R resets it; clicking the baseball also increments
- Spit overlays accumulate visually on the baseball up to 10 spits
- Count persists across page reloads (localStorage or equivalent)
- UI adapts to the HomeHub design system (sidebar navigation, consistent layout)

## Notes & Open Questions

- Source: https://github.com/slowback1/spit-counter
- The original uses a MessageBus + localStorage for persistence — decide whether to keep that or use the HomeHub session/storage pattern
- Should the count reset after some threshold (>10 spits) or continue accumulating beyond the visual overlays?
- Consider whether keyboard shortcuts (Space, R) conflict with any global HomeHub shortcuts
