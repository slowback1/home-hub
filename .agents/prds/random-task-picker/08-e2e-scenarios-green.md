# E2E Scenarios GREEN

## Status

`done` <!-- pending | in-progress | done -->

## Description

Complete the E2E test suite by implementing all step definitions and page objects stubbed in task 01. Add the DB seeding mechanism needed by the display-page scenarios. All 5 `@activity` scenarios must pass GREEN by the end of this task.

## Acceptance Criteria

- [ ] All step definitions in `e2e/steps/activity.steps.ts` are fully implemented (no `not implemented` stubs remaining)
- [ ] `e2e/pages/ActivityPage.ts` and `e2e/pages/ActivityConfigPage.ts` expose concrete navigation and interaction methods used by the steps
- [ ] A test-helper mechanism exists for seeding and clearing `ActivityPick` rows (e.g. a `POST /api/test/activity-picks` endpoint gated behind a test-only config flag, or direct DB fixture in `fixtures.ts`)
- [ ] `@add-activity-happy-path` passes: adding "Play Chess" at weight 3 appears in the config list
- [ ] `@change-activity-weight` passes: changing weight from 3 to 5 is reflected immediately in the UI
- [ ] `@delete-activity` passes: deleting "Play Chess" removes it from the config list
- [ ] `@activity-empty-state` passes: with no picks seeded, the display page shows the placeholder
- [ ] `@activity-display-current-pick` passes: with a seeded pick for the current hour, "Play Chess" appears as the current pick
- [ ] `task e2e:test` (full suite) passes with no regressions in existing scenarios

## Notes

- DB seeding for the display-page scenarios: the simplest approach is a test-helper API controller (e.g. `TestHelperController`) that is only registered when a `Testing` config flag is set. It exposes endpoints to insert/clear `ActivityPick` rows directly. Alternatively, use Playwright fixtures to call the real `POST /api/activities` and trigger a pick via a test-only job endpoint.
- The `@activity-display-current-pick` scenario seeds a pick "for the current hour" — the step should insert a record with `PickedAt` set to the current UTC hour boundary (e.g. `DateTime.UtcNow` truncated to the hour)
- Review `e2e/AGENTS.md` for fixture wiring conventions before implementing
