# Remove Theme Switching Infrastructure

## Status

`done`

## Description

Delete all code related to light/dark theme switching: `ThemeService`, `ThemeToggle`, and the `Header` component (which will be replaced by the Sidebar in a later task). Remove the `MessageBus` subscription for `CurrentTheme` from the root layout and any other consumers. The root layout should no longer conditionally apply theme classes — the single dark theme is always active.

## Acceptance Criteria

- [ ] `ThemeService` and its associated files are deleted
- [ ] `ThemeToggle` component is deleted
- [ ] `Header` component and its associated files (including `HeaderLink`, `SkipToContentLink`) are deleted
- [ ] The root layout (`+layout.svelte`) no longer imports or references `ThemeService`, `ThemeToggle`, `Header`, or `ColorTheme`
- [ ] The root layout no longer subscribes to `Messages.CurrentTheme` or conditionally applies `.light-theme` / `.dark-theme` classes
- [ ] No other files reference `ThemeService` or `ColorTheme`
- [ ] All existing tests pass (delete any tests that exclusively tested the removed components)

## Notes

Files expected to be deleted (verify paths before removing):
- `frontend/src/lib/services/Theme/ThemeService.ts` (and related spec)
- `frontend/src/lib/components/navigation/ThemeToggle.svelte`
- `frontend/src/lib/components/navigation/Header.svelte`
- `frontend/src/lib/components/navigation/HeaderLink.svelte`
- `frontend/src/lib/components/navigation/SkipToContentLink.svelte`

The `MessageBus` and `LocalStorageProvider` themselves should be kept — they may be used by other features (e.g. Toasts). Only remove the theme-specific subscription from the root layout.

`FeatureFlagService` and `ConfigService` initialization in the root layout should remain untouched.
