# Client-Side Search + Starred Filter (E2E Green)

## Status

`done`

## Description

Add the search bar and starred-only filter toggle to the bookmarks toolbar. Both filter the displayed card grid client-side without any API calls. This is the final implementation task; all E2E scenarios must pass GREEN before it is marked done.

## Acceptance Criteria

- [ ] A search input appears in the toolbar when at least one bookmark exists (hidden on the zero-bookmark empty state)
- [ ] Typing in the search input filters cards client-side, matching against name, URL, and description (case-insensitive)
- [ ] A "Starred only" toggle pill appears alongside the search bar, showing the count of starred bookmarks
- [ ] When starred-only is active, only starred bookmarks are shown (combined with any active search query)
- [ ] A "Clear filters" affordance is shown when the filtered list is empty but bookmarks exist
- [ ] Results summary line updates to reflect active filters: _"X of Y bookmarks"_, _"starred only"_, _"matching 'query'"_ as applicable
- [ ] Star button on each card correctly toggles starred state by calling `BookmarksApi.toggleStar` and updating the card
- [ ] All 9 E2E scenarios in `e2e/features/bookmarks.feature` pass GREEN with `task e2e:test`

## Notes

- The E2E Before/After hooks (written as stubs in task 01) need to be fully wired here: enable `BOOKMARKS_ENABLED` flag and clear/restore test data
- The search and star filter are purely client-side — no changes to the GET endpoint needed
- The "Starred only" pill with count matches the design handoff: star icon + label + count badge
- The toolbar (search + starred pill) should only render when `totalCount > 0`; see the design handoff for the two-empty-state distinction
