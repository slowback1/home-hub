# Chore Tracker → RPG Integration

**Status:** stub
**Created:** 2026-05-25

## Summary

Wire chore task completions into the HomeHub RPG as progression events — completing a task grants the character XP, items, or other in-game rewards.

## Problem / Opportunity

The chore tracker already records task completions. Rewarding those completions in the RPG creates a motivational loop: doing real-world chores has an in-game payoff, making both systems more engaging.

## Success Looks Like

- When a chore task is marked complete, an RPG progression event is triggered (XP grant, item, buff, etc.)
- The reward is reflected immediately (or near-immediately) in the character's state
- The game UI shows a visible acknowledgment when a chore-driven reward is received (e.g. an entry in the combat log: "You completed 'Clean kitchen' and gained 50 XP!")
- Completing multiple tasks on the same day is rewarded, but there's some design decision about diminishing returns or a daily cap (open question)
- Undo-ing a task completion (the existing undo toast feature) should reverse or forfeit the associated reward

## Notes & Open Questions

- **Reward model**: What does a task completion give? XP is simplest. Could also drop items, restore HP, grant a daily buff. TBD during PRD refinement.
- **Reward scaling**: Should recurring tasks and one-off tasks give different rewards? Should task difficulty/recurrence interval affect the reward size?
- **Undo interaction**: The chore tracker supports undoing a completion within ~5 seconds. If an RPG reward was granted, should it be clawed back on undo? Probably yes, but requires the reward grant to be reversible.
- **Implementation approach**: Options include (a) the chore controller directly calls an RPG service on completion, (b) an event/message is emitted and the RPG polls for unconsumed events, or (c) the RPG checks completion history on each game tick. Loosest coupling is probably (b).
- **Depends on**: HomeHub RPG Core (character model and XP system must exist first). Existing chore tracker (`/api/tasks/{id}/complete`) stays unchanged — integration is additive.
