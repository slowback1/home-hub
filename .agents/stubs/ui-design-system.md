# UI Design System

**Status:** stub
**Created:** 2026-04-17

## Summary

Establish a consistent, well-organized UI component library — either by adopting an existing design system or structuring an internal one — so all current and future features share a cohesive visual language.

## Problem / Opportunity

The current UI components are visually rough and inconsistently organized, making the app feel unpolished and making future UI work slower. A design system provides reusable primitives (typography, color, spacing, components) that make building new features faster and more consistent.

## Success Looks Like

- A clear set of base components exists (buttons, inputs, cards, layout primitives, typography) with consistent styling
- Components are organized in a discoverable way (e.g. a component directory or Storybook-style catalog)
- Existing rough UI is migrated to use the new system
- Adding a new page or feature defaults to using design system components rather than one-off styles

## Notes & Open Questions

- Adopt an existing system (shadcn/ui, Radix, Mantine, etc.) or build a custom one from scratch?
- What is the current frontend stack? (React, Vue, etc. — affects component library options)
- Dark mode / theming in scope now or later?
- Should a visual catalog (Storybook or similar) be part of this, or deferred?
- Are there existing components worth keeping, or is this a clean slate?
