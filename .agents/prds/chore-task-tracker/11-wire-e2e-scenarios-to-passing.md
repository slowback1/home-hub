# Wire E2E Scenarios to Passing

## Status

`pending`

## Description

Implement the full `TasksPage` page object and all step definitions in `tasks.steps.ts` so that every scenario from `tasks.feature` passes GREEN. This is the final integration gate for the feature.

## Acceptance Criteria

- [ ] `e2e/pages/TasksPage.ts` has methods covering all step actions: navigating to the page, reading Due/Upcoming section contents, clicking Done, clicking Undo, opening Add/Edit modal, submitting the form, clicking Delete
- [ ] All 9 scenarios in `e2e/features/tasks.feature` pass when running `task e2e:test`
- [ ] No existing E2E scenarios are broken (full suite passes)

## Notes

- Scenarios that involve creating seed data (e.g. "Given a task X with DoDate 5 days ago") will likely need to call the API directly from the step definition or use a fixture — check how other step files handle test data setup (e.g. `audiobook.steps.ts`)
- The `tasksPage` fixture must be registered in `e2e/fixtures.ts` (started in task 01)
- Run with `task e2e:test` from repo root
