# Add/Edit Modal

## Status

`done`

## Description

Implement the `BookmarkFormModal` component used for both adding new bookmarks and editing existing ones. Wire it to the Add Bookmark button in the page header and the Edit button on each card.

## Acceptance Criteria

- [ ] Modal opens when the "Add Bookmark" header button is clicked (empty form)
- [ ] Modal opens when a card's Edit button is clicked (pre-populated with existing values)
- [ ] Form fields: URL (required), Name (optional), Description (optional textarea)
- [ ] URL field shows a validation error if left blank or not parseable as a URL on submit
- [ ] URL auto-normalizes: if no protocol is present, `https://` is prepended before saving
- [ ] Name defaults to the hostname (stripped of `www.`) if left blank
- [ ] On submit (add): calls `BookmarksApi.createBookmark`, closes modal, new card appears in the grid in correct alphabetical position
- [ ] On submit (edit): calls `BookmarksApi.updateBookmark`, closes modal, card reflects updated values
- [ ] Cancel closes the modal without saving
- [ ] Modal title is "Add Bookmark" or "Edit Bookmark" depending on context

## Notes

- Use the existing `Modal` primitive component from the HomeHub design system
- URL focus is set automatically when the modal opens (autofocus on the URL input)
- The name placeholder text should show the derived hostname as a hint while the URL field has a value (e.g. `Defaults to "github.com"`)
- See the design handoff for the form layout and field behaviour
