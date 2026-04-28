# Build Sidebar Component

## Status

`done`

## Description

Build the `Sidebar` Svelte component: a persistent, collapsible navigation rail that displays the app logo (linking to `/`) and a flat list of icon+label nav items. A collapse toggle at the bottom switches between expanded (~220px) and icon-only (~60px) modes. Collapsed state is persisted in localStorage so it survives page loads. Includes unit tests and a Storybook story.

## Acceptance Criteria

- [ ] `Sidebar` component exists at `frontend/src/lib/ui/navigation/Sidebar.svelte` (or equivalent path)
- [ ] Sidebar renders app logo/name at the top, linking to `/`
- [ ] Sidebar accepts nav item configuration (route, label, Lucide icon) as a prop or slot
- [ ] Nav items render with a Lucide icon and label text in expanded mode; icon only in collapsed mode
- [ ] The currently active route's nav item is visually highlighted
- [ ] A collapse toggle button is rendered at the bottom of the Sidebar
- [ ] Clicking the toggle switches between expanded and icon-only mode
- [ ] Collapsed state is read from and written to localStorage on mount and on toggle
- [ ] Sidebar uses design tokens from task 02 for all spacing, color, and radius values
- [ ] Unit tests cover: toggle behavior, active route highlighting, localStorage persistence
- [ ] A Storybook story exists showing the Sidebar in both expanded and collapsed states

## Notes

Nav items for HomeHub (icon names are from `lucide-svelte`):
- Home → `/` → `House`
- Chore / Task Tracker → `/tasks` → `CheckSquare`
- Random Task Picker → `/activity` → `Shuffle`
- RetroAchievements → `/retro` → `Gamepad2`
- Weather → `/weather` → `Cloud`

Use SvelteKit's `$page.url.pathname` to determine the active route. The Sidebar is not wired into the root layout in this task — that happens in task 05.
