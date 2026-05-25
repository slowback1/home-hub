# Bookmarks Route + Card Grid + Empty States

## Status

`done`

## Description

Create the `/bookmarks` SvelteKit route, the `BookmarkCard` component, and the two empty states. This task wires the API client to the page and renders the grid of cards, but defers the modal, delete dialog, search, and star filter to later tasks.

## Acceptance Criteria

- [ ] `frontend/src/routes/bookmarks/+page.svelte` exists and loads bookmarks from `BookmarksApi` on mount
- [ ] Bookmarks are displayed alphabetically by name as a grid of `BookmarkCard` components
- [ ] `BookmarkCard` shows: favicon (using the `Favicon` component from task 06), bookmark name, hostname (stripped of `www.`), and optional description
- [ ] Clicking a card opens the URL in a new tab (`target="_blank" rel="noopener noreferrer"`)
- [ ] Edit and delete buttons are present on each card (can be no-ops / placeholders until tasks 08–09)
- [ ] Star button is present on the card (no-op placeholder until task 10)
- [ ] Zero-bookmark empty state is shown when the list is empty: Bookmark icon, "No bookmarks yet" heading, descriptive sub-text, and an "Add Bookmark" button (button can be a no-op until task 08)
- [ ] Page is accessible via the sidebar nav when `BOOKMARKS_ENABLED` is on
- [ ] Results summary line shown above the grid: _"X of Y bookmarks"_ (simplified — no filter clauses until task 10)

## Notes

- The toolbar (search bar + starred-only pill) is added in task 10; this task only renders the grid and the Add button in the page header
- See the design handoff for card layout: favicon + title/hostname block + star button (top row), description (middle), edit/delete actions (bottom)
- The page header "Add Bookmark" button is wired in task 08
