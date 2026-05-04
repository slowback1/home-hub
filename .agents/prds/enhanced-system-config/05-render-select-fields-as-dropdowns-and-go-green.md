# Render Select-Type Config Fields as Dropdowns and Go GREEN

## Status

`done`

## Description

Update the System Config admin page to inspect each entry's `Type` field and render `select`-type entries as a `<select>` dropdown populated from the entry's `Options` list. Selecting a new value saves immediately (no separate Save button). Wire up the new Page Object methods and step definitions, then drive all four enhanced-system-config E2E scenarios to GREEN.

## Acceptance Criteria

- [ ] Config entries with `Type = "select"` render as a `<select>` element on the admin page
- [ ] The `<select>` is populated with the entry's `Options` (using `Value` as the option value and `Label` as the display text)
- [ ] Changing the selected option calls `PUT /api/system-config/{namespace}/{key}` with the new value immediately (no separate Save button)
- [ ] A success toast is shown after a successful save; an error toast is shown on failure
- [ ] Config entries with `Type = "text"` or unset `Type` continue to use the existing inline text-edit behaviour
- [ ] `SystemConfigPage.ts` Page Object has methods to support the new step definitions (e.g. `selectDropdownOption`, `getDropdownOptions`, `hasSelectField`)
- [ ] `task e2e:test` targeting `enhanced-system-config.feature` — all four scenarios pass GREEN
- [ ] All existing system-config-ui E2E scenarios continue to pass GREEN

## Notes

Select fields should not enter an "edit mode" — they are always in their rendered select state. The existing click-to-edit pattern only applies to `text` and `secret` entries.

The `Before { tags: '@admin' }` hook in `system-config-ui.steps.ts` resets `weather::zip_code` and `weather::api_key` to known values. Extend this hook (or add a complementary one) to also reset `weather::provider` to `"mock"` before each `@admin` scenario so tests don't bleed state.
