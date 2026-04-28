# PRD: UI Design System

## Status

`Draft`

## Overview

Establish a cohesive, well-organized UI design system for HomeHub — a personal hub platform that will house a growing collection of independent side projects under a unified interface. This work formalizes the existing component library, polishes its visual quality, adds missing primitives, and replaces the current Header-based shell with a persistent collapsible Sidebar, all underpinned by a consistent token system and documented in Storybook.

## Problem Statement

The current UI components are visually rough and inconsistently organized. There is no coherent token system for spacing, radius, or color beyond a minimal set of CSS variables. As new sections are added to HomeHub, each risks developing its own one-off styles, making the app feel fragmented. Without a design system, building new features is slower and the result is less polished.

## Goals

- Establish a complete design token system (color, typography, spacing, radius) rooted in a single dark theme
- Polish all existing components to meet the new visual standard
- Add missing layout and UI primitives needed to build any new section
- Replace the top Header with a persistent, collapsible Sidebar as the primary navigation shell
- Create stub routes for all planned hub sections so Sidebar navigation is functional
- Make Storybook the discoverable source of truth, with a Design Tokens story at the top
- Migrate all existing pages to use the new system

## Non-Goals

- Extracting the component library as a standalone npm package
- Light theme or theme switching (architecture remains token-based for future flexibility)
- Animation or motion system
- Responsive / mobile layout
- Accessibility audit beyond what already exists

## User Stories / Use Cases

- **As a** developer adding a new hub section, **I want to** reach for existing design system components, **so that** my new page looks consistent with the rest of the app without custom styling.
- **As a** user, **I want to** navigate between hub sections via the Sidebar, **so that** I can switch contexts quickly from anywhere in the app.
- **As a** user, **I want to** collapse the Sidebar to icon-only mode, **so that** I can reclaim horizontal space when working in content-heavy sections.
- **As a** developer, **I want to** browse the full token set and component catalog in Storybook, **so that** I understand what's available before building a new feature.

## E2E Scenarios

```gherkin
@design-system
Feature: App Shell Navigation

  @sidebar-navigation
  Scenario: Navigate between hub sections via the Sidebar
    Given I am on the home page
    When I click the "Chore / Task Tracker" nav item in the Sidebar
    Then I should be on the task tracker page
    And the "Chore / Task Tracker" nav item should be marked as active

  @sidebar-collapse
  Scenario: Collapse the Sidebar to icon-only mode
    Given I am on the home page
    And the Sidebar is expanded
    When I click the collapse toggle
    Then the Sidebar should collapse to icon-only mode
    And nav item labels should not be visible

  @sidebar-collapse-persists
  Scenario: Collapsed Sidebar state persists across page loads
    Given I am on the home page
    And I have collapsed the Sidebar
    When I reload the page
    Then the Sidebar should still be in icon-only mode

  @dark-theme-applied
  Scenario: App renders in dark theme by default
    Given I navigate to the home page
    Then the app should have the dark theme applied
    And no light theme class should be present on the document
```

## Proposed Solution

Replace the existing rough component library and Header-based shell with a polished, token-driven design system built on the existing Svelte 5 + SvelteKit + pure CSS stack. The system is documented in Storybook, with a Design Tokens top-level story as the entry point. All existing pages are migrated to use the new system.

### Visual Direction

- **Aesthetic**: Warm and friendly — rounded corners, soft color palette, approachable feel
- **Primary color**: `#003554` (deep navy)
- **Theme**: Single dark theme; surfaces are tinted with the primary hue (deep navy backgrounds, e.g. `#001a2e`). Token architecture uses CSS custom properties on a single theme class, making future theme additions trivial.
- **Typography**: Nunito (Google Fonts), loaded via `<link>` in the app head
- **Border radius**: `8px` for interactive elements (buttons, inputs); `12px` for containers (cards, modals)
- **Spacing scale**: 4px base — `4, 8, 12, 16, 24, 32, 48, 64px` as named tokens

## Technical Approach

The implementation stays entirely within the existing stack: Svelte 5, SvelteKit, pure CSS with CSS custom properties, Storybook 8, and `lucide-svelte` for icons.

### Key Decisions

| Decision | Chosen Approach | Rationale |
|----------|----------------|-----------|
| Theme architecture | Single dark theme class; CSS custom properties on `:root` or one class | Keeps token structure clean so a second theme can be added without rework |
| Theme switching | Remove `ThemeService`, `ThemeToggle`, and all light/dark class-switching logic | App is dark-only; the complexity has no current value |
| Primary navigation | Persistent collapsible Sidebar replaces the Header entirely | Sidebar scales better as more sections are added; Header + Sidebar would be redundant |
| Sidebar logo | App name / logo links to `/` | Expected convention; coexists naturally with a Home nav item |
| Sidebar collapse toggle | Positioned at the bottom of the Sidebar | Keeps the top area clean; bottom-anchored controls are a common pattern and reduce accidental clicks |
| Sidebar collapse | Collapses to icon-only rail (~60px); state persisted in localStorage | Reclaims space for content-heavy sections; state survives page loads |
| Icon library | `lucide-svelte` | Native Svelte integration; rounded stroke style matches warm/friendly aesthetic; large icon set |
| Section icons | Home: `House`, Tasks: `CheckSquare`, Activity: `Shuffle`, RetroAchievements: `Gamepad2`, Weather: `Cloud` | Lucide icons chosen to reflect each section's purpose |
| Typography | Nunito via Google Fonts | Rounded letterforms reinforce the warm aesthetic; good web weight range |
| Storybook structure | Design Tokens story first, then components grouped by type | Makes the system legible as a system, not just a component list |
| Migration | All existing pages migrated as part of this work | Existing pages are minimal; leaving them on the old system would immediately create inconsistency |
| Test integrity | All unit tests must pass at the completion of each unit of work | Visual polish changes should not break behaviour; failures are fixed as they appear |

### New Components

The following components do not currently exist and will be built as part of this work:

| Component | Purpose |
|-----------|---------|
| `Sidebar` | Persistent collapsible app shell navigation |
| `Card` | General-purpose content container |
| `Spinner` | Loading state indicator |
| `Badge` | Small count or status label |
| `Tabs` | In-page section switching |
| `Divider` | Visual separator between sections |

### Stub Routes

Stub pages (heading + placeholder copy) will be created for all planned hub sections so Sidebar navigation is functional from day one:

- `/tasks` — Chore / Task Tracker
- `/activity` — Random Task Picker
- `/retro` — RetroAchievements
- `/weather` — Weather Widget

### Storybook Structure

```
Design Tokens        ← color palette, type scale, spacing scale, radius
Buttons              ← Button (all variants and sizes)
Inputs               ← TextBox, Checkbox, CheckboxGroup, Select, ComboBox, ToggleSwitch
Containers           ← Card, Accordion, Alert, Chip, Toast, Tooltip
Feedback             ← Spinner, Badge
Navigation           ← Sidebar, Tabs
Data                 ← Table, TablePagination, TableFilter
Typography           ← Heading, Divider
```

### Dependencies

- `lucide-svelte` — icon library (new dependency)
- Nunito — Google Font (loaded via `<link>`, no npm dependency)
- Existing: Svelte 5, SvelteKit, Storybook 8, Vitest, `@testing-library/svelte`

## Open Questions

_None — all questions resolved during planning._

## Out of Scope

- npm package extraction
- Light theme
- Animation / motion system
- Responsive / mobile layout
- Accessibility audit

## Success Metrics

- All existing pages render using the new design token system with no one-off inline styles
- Storybook catalog is complete: every component has at least one story; Design Tokens story shows the full palette, type scale, and spacing scale
- Sidebar navigation is functional across all stub routes and persists collapse state
- `ThemeService`, `ThemeToggle`, and light/dark switching logic are fully removed
- No regressions in existing unit tests after migration

## Timeline / Milestones

_TBD_
