# Add Title Case Utility and Update Section Headers / Field Labels

## Status

`done`

## Description

Add a small frontend utility that converts snake_case strings to Title Case, then apply it to the System Config admin page so namespace values appear as section headers (e.g. `weather` → "Weather") and key values appear as field labels (e.g. `zip_code` → "Zip Code"). No backend changes required.

## Acceptance Criteria

- [ ] A utility function (e.g. `toTitleCase(s: string): string`) exists and is unit-tested for the key cases: single word, snake_case with underscores, already-titlecased input
- [ ] Namespace values on the System Config admin page render as Title Case section headers
- [ ] Key values on the System Config admin page render as Title Case field labels
- [ ] The raw `namespace` and `key` strings are no longer displayed directly in the UI
- [ ] All existing system-config-ui E2E scenarios continue to pass (labels change but selectors in step definitions should target by role/test-id, not visible text — update step definitions/page object if needed)

## Notes

Expected conversions:
- `weather` → "Weather"
- `zip_code` → "Zip Code"
- `api_key` → "Api Key"
- `retro_achievements` → "Retro Achievements"

The utility only needs to handle lowercase snake_case input — no camelCase or mixed-case handling required yet.

Place the utility in a logical location alongside other frontend helpers (e.g. `frontend/src/lib/utils/`).
