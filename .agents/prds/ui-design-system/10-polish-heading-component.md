# Polish Heading Component

## Status

`done`

## Description

Update the `Heading` component to use the new typography token system: Nunito font family (inherited globally), updated size tokens for each heading level (h1–h6), and color tokens for heading text. This is a small but visible task since headings appear on every page.

## Acceptance Criteria

- [ ] Heading renders in Nunito (inherited from the global font-family token — no per-component font override needed)
- [ ] Font sizes for each heading level (h1–h6) reference typography tokens from `globals.css`
- [ ] Heading color references a design token (not a hardcoded value)
- [ ] All six heading levels (h1–h6) and font-weight control work correctly
- [ ] Existing Heading unit tests pass; update tests if token names changed
- [ ] Storybook story for Heading reflects the updated visual

## Notes

Heading component is at `frontend/src/lib/ui/typography/Heading.svelte`. Current sizing uses `small: 16px, medium: 24px, large: 32px` — map these to a sensible h1–h6 scale using the new token system.
