# Add Design Tokens Storybook Story

## Status

`done`

## Description

Create a top-level Storybook story that visually documents the complete design token system: the full color palette, type scale, spacing scale, and border radius values. This story is the entry point into the Storybook catalog and makes the system legible as a system rather than a collection of components.

## Acceptance Criteria

- [ ] A "Design Tokens" story exists and appears at the top of the Storybook sidebar (use a sort prefix or `storySort` config to ensure ordering)
- [ ] Color palette section shows all token colors with their CSS variable names and hex values
- [ ] Typography section shows each font size token rendered with Nunito, labeled with the token name
- [ ] Spacing section shows each spacing token as a visual bar with its token name and pixel value
- [ ] Border radius section shows `--radius-sm` and `--radius-md` with a visual swatch
- [ ] Story reads values directly from CSS custom properties where possible (so it stays in sync with `globals.css` automatically)
- [ ] Storybook builds without errors

## Notes

Place the story at `frontend/src/stories/DesignTokens.stories.ts` (or `.svelte` if a component wrapper is needed to render the swatches). Use Storybook's `storySort` in `.storybook/preview.ts` to pin "Design Tokens" to the top of the sidebar.
