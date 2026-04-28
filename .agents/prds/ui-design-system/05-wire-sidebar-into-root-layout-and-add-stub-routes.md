# Wire Sidebar Into Root Layout and Add Stub Routes

## Status

`pending`

## Description

Replace the Header with the Sidebar in the root layout and update the layout CSS to a sidebar+content split. Create stub pages for all planned hub sections (/tasks, /activity, /retro, /weather) so Sidebar navigation is functional and the @sidebar-navigation E2E scenario has a real route to land on.

## Acceptance Criteria

- [ ] Root layout (`+layout.svelte`) imports and renders `Sidebar` in place of the removed `Header`
- [ ] Layout CSS uses a horizontal split: Sidebar on the left, main content area filling the remaining space
- [ ] Main content area still renders `<slot />` and `ToastWrapper`
- [ ] `/tasks` stub page exists with a heading ("Chore / Task Tracker") and placeholder copy
- [ ] `/activity` stub page exists with a heading ("Random Task Picker") and placeholder copy
- [ ] `/retro` stub page exists with a heading ("RetroAchievements") and placeholder copy
- [ ] `/weather` stub page exists with a heading ("Weather Widget") and placeholder copy
- [ ] Navigating to each stub route via the Sidebar highlights the correct nav item
- [ ] All existing tests pass

## Notes

The root layout is at `frontend/src/routes/+layout.svelte`. The current layout wraps everything in a theme-class div and uses `.main-content` with padding derived from `--gutters-y` / `--gutters-x`. Update this to a CSS grid or flexbox layout with the Sidebar fixed on the left.

Stub pages only need a `+page.svelte` file — no server routes or data loading required.
