# Add shared spin utility

## Status

`done`

## Description

Add a small, pure, well-tested function that parses a wheel's newline-delimited `items` string into a clean list and picks one uniformly at random. Shared by the `/wheels` page's spin section and the dashboard widget so the selection logic lives in one tested place.

## Acceptance Criteria

- [ ] A utility (e.g. `frontend/src/lib/utils/spinWheel.ts`) exposes: parsing `items` into trimmed, non-empty lines; and picking a uniformly-random element from a list.
- [ ] The parser trims each line and drops blank lines; duplicates are preserved (they legitimately bias the odds).
- [ ] Picking from an empty list returns `null` (or equivalent) rather than throwing.
- [ ] Unit tests assert: the picked value is always a member of the input list; empty input yields no pick; blank/whitespace lines are excluded from the item list.
- [ ] Frontend unit tests pass.

## Notes

- Keep it framework-free (a plain `.ts` module) so both `+page.svelte` and `WheelsWidget.svelte` can import it — analogous to how `RandomActivitySelector` isolates selection on the backend.
- Randomness can use `Math.random()`; tests should assert membership/invariants across many iterations rather than a fixed value.
- Follow the existing `frontend/src/lib/utils/` conventions for file layout and test naming.
