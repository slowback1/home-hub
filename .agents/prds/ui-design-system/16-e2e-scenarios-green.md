# E2E Scenarios Green

## Status

`pending`

## Description

Implement the step definitions and Page Objects for all `@design-system` E2E scenarios so they pass GREEN. This task closes the loop opened in task 01 — the feature is complete when all four scenarios pass end-to-end against the running app.

## Acceptance Criteria

- [ ] `e2e/steps/design-system.steps.ts` has fully implemented step definitions (no `not implemented` stubs remaining)
- [ ] All required Page Objects in `e2e/pages/` are fully implemented
- [ ] `task e2e:test --grep @design-system` passes with all four scenarios GREEN:
  - `@sidebar-navigation` — navigating to the Task Tracker page via the Sidebar
  - `@sidebar-collapse` — collapsing the Sidebar to icon-only mode
  - `@sidebar-collapse-persists` — collapsed state survives a page reload
  - `@dark-theme-applied` — app renders with dark theme class, no light theme class present
- [ ] No other existing E2E scenarios are broken

## Notes

By the time this task runs, the Sidebar, stub routes, and dark theme are all wired up (tasks 04, 05, and 02/03). The step implementations should be straightforward DOM interactions and assertions.

For `@sidebar-collapse-persists`, the step that reloads the page should use Playwright's `page.reload()` and then re-query the Sidebar state from the DOM.

For `@dark-theme-applied`, assert the absence of a `.light-theme` class on the document root and the presence of whatever theme class was established in task 02.
