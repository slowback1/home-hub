# BOOKMARKS_ENABLED Feature Flag + Sidebar Nav Entry

## Status

`pending`

## Description

Register the `BOOKMARKS_ENABLED` feature flag (disabled by default) and add the Bookmarks entry to the sidebar navigation, gated by the flag. This makes the feature accessible in the UI once the flag is turned on.

## Acceptance Criteria

- [ ] `BOOKMARKS_ENABLED` added to `frontend/src/lib/services/FeatureFlag/FeatureFlags.ts`
- [ ] Bookmarks nav item added to the `navItems` array in `Sidebar.svelte`: `href: '/bookmarks'`, label `'Bookmarks'`, icon `Bookmark` from lucide-svelte, flag `FeatureFlags.BOOKMARKS_ENABLED`, `testId: 'nav-item-bookmarks'`
- [ ] Nav item only appears in the sidebar when `BOOKMARKS_ENABLED` is enabled (consistent with existing flag-gated items)
- [ ] Feature flag is seeded as disabled by default in the backend seed/default data

## Notes

- See `FeatureFlags.ts` for the existing flag names — add `BOOKMARKS_ENABLED: 'BOOKMARKS_ENABLED'`
- See `Sidebar.svelte` for the navItems array structure; the `Bookmark` icon is available in lucide-svelte
- The `/bookmarks` route itself is created in task 07 — this task only adds the nav entry and flag registration
- Check how other flags are defaulted to disabled (e.g. `AUDIOBOOK_ENABLED`) to ensure consistency
