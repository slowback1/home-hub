---
name: design-brief
description: Generate a short, focused prompt to paste into Claude Design (claude.ai/design) for a feature that has been refined via interview. The project's design system is already stored in Claude Design, so the brief only needs to describe what to design — not how it should look.
---

# Design Brief

Using the feature context from the current conversation (interview notes, user stories, proposed solution), produce a **Claude Design prompt** the user can paste directly into claude.ai/design.

## Rules

- **Short**: 3–6 sentences maximum. Claude Design already knows the HomeHub design system (colors, typography, components, voice) — do not describe any of that.
- **Functional, not visual**: describe the screen's purpose, the key UI elements it must contain, and any important interaction or state the design needs to handle (e.g. empty state, loading, error). Do not dictate layout or style choices — those are for the designer.
- **Specific enough to be actionable**: name the screen, name the data it shows, name the primary user actions.
- **No implementation detail**: do not mention Svelte, API endpoints, or database fields.

## Output format

Present the brief inside a fenced block so the user can copy it cleanly:

```
[The brief text, ready to paste]
```

Then say: "Paste that into Claude Design, iterate until you're happy with it, then share the URL back here and I'll finish the PRD."

Do not ask any follow-up questions. One output, then wait.
