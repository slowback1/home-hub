# Establish Design Token System

## Status

`done`

## Description

Rewrite `globals.css` with a complete, single dark-theme token system covering color, spacing, border radius, and typography. Remove all light-theme variables. Install `lucide-svelte` and wire Nunito via a Google Fonts `<link>` in the app head. This task is the foundation everything else builds on — no component polish or new components should begin until the token system is in place.

## Acceptance Criteria

- [ ] `lucide-svelte` is installed as a project dependency
- [ ] Nunito is loaded via a Google Fonts `<link>` in the SvelteKit app head (e.g. `src/app.html` or the root layout `<svelte:head>`)
- [ ] `globals.css` defines a complete color token set derived from primary `#003554`, with navy-tinted dark surface colors (e.g. `#001a2e` range)
- [ ] `globals.css` defines a spacing scale using 4px base: `--space-1` (4px) through `--space-16` (64px) or equivalent named tokens
- [ ] `globals.css` defines radius tokens: `--radius-sm` (8px for interactive elements), `--radius-md` (12px for containers)
- [ ] `globals.css` defines typography tokens referencing Nunito as the primary font family
- [ ] All tokens live under a single theme class (e.g. `.app-theme` or `:root`) — no separate `.light-theme` / `.dark-theme` classes
- [ ] All existing tests pass

## Notes

Current token file: `frontend/src/style/globals.css`. Current tokens include `--color-primary`, `--color-secondary`, `--gutters-y`, `--gutters-x`, and a handful of component-level variables — these should all be replaced or subsumed by the new system.

The old `--gutters-y` / `--gutters-x` tokens are referenced in the root layout and possibly other components. Map them to equivalents in the new spacing scale and update references.
