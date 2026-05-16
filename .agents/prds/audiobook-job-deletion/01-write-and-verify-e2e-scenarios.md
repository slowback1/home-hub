# Write & Verify E2E Scenarios

## Status

`done`

## Description

Add three deletion Gherkin scenarios to the existing `e2e/features/audiobook.feature` file, stub the corresponding step definitions and any required page object methods, then run the new scenarios and confirm they fail RED with a meaningful "not implemented" error — not a tooling or compilation error. This confirmed-RED state is the gate for this task.

## Acceptance Criteria

- [ ] Three new scenarios added to `e2e/features/audiobook.feature`: `@audiobook-delete-completed-job`, `@audiobook-delete-failed-job`, `@audiobook-delete-cancelled-job`
- [ ] Step definitions for all new steps stubbed in `e2e/steps/audiobook.steps.ts` (throw or `pending()` — not silently passing)
- [ ] Any new page object methods required (e.g. `clickDelete`, `jobIsAbsent`) stubbed in `e2e/pages/AudiobookConvertPage.ts`
- [ ] `task e2e:test` runs without tooling or compilation errors
- [ ] All three new scenarios FAIL with a meaningful assertion or "not implemented" error (confirmed RED)

## Notes

Scenarios (from PRD):

```gherkin
@audiobook-delete-completed-job
Scenario: Delete a completed job
  Given I am on the audiobook convert page
  And a completed job exists
  When I click delete on the completed job
  Then the job is removed from the queue list

@audiobook-delete-failed-job
Scenario: Delete a failed job
  Given I am on the audiobook convert page
  And a failed job exists
  When I click delete on the failed job
  Then the job is removed from the queue list

@audiobook-delete-cancelled-job
Scenario: Delete a cancelled job
  Given I am on the audiobook convert page
  And a cancelled job exists
  When I click delete on the cancelled job
  Then the job is removed from the queue list
```

Several steps may already be implemented in the existing `audiobook.steps.ts` (e.g. "I am on the audiobook convert page", "a completed job exists"). Only stub the genuinely new ones.
