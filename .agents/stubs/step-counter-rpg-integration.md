# Step Counter → RPG Integration

**Status:** stub
**Created:** 2026-05-25

## Summary

Wire synced walk sessions from the step counter sidecar into the HomeHub RPG as progression events — longer or more frequent walks grant larger in-game rewards.

## Problem / Opportunity

Walk sessions represent sustained physical effort. Rewarding them in the RPG creates a motivational loop between going outside and in-game progress, and distinguishes walking (longer, higher-effort) from chore completion (shorter, task-based) as a distinct progression path.

## Success Looks Like

- When a walk session syncs to the backend, an RPG progression event is triggered proportional to the session (e.g. steps or duration → XP)
- The reward appears in the character's state and game log after sync
- Walking significantly further than usual gives a noticeably better reward, creating an incentive for longer walks
- There is some design decision about how to prevent grinding (e.g. a daily step cap for RPG rewards, or diminishing returns per session)

## Notes & Open Questions

- **Reward formula**: How do steps/duration map to XP? Linear (100 steps = 10 XP)? Tiered (walk > 30 min = bonus multiplier)? TBD during PRD refinement.
- **Granularity**: Reward per session (one lump sum on sync) vs. per-step (impractical). Per-session is simpler.
- **Anti-abuse**: Since step counting can be gamed (e.g. shaking the phone), some sanity cap is probably wise. Daily XP ceiling from walks? Velocity check on step rate?
- **Delayed rewards**: Sessions may sync hours after the walk happened. The game should show rewards at sync time, but the log entry could note the original walk time.
- **Depends on**: HomeHub RPG Core (character model and XP system) and Step Counter Sidecar (walk session data must reach the backend) — this is the last piece in the chain.
