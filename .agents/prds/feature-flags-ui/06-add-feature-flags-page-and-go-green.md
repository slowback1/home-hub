# Add Feature Flags Page and Go GREEN

## Status

`done` <!-- pending | in-progress | done -->

## Description

Build the `/admin/feature-flags` page and implement the E2E step definitions and page object so all four scenarios pass. This is the final task; it ties together all prior work and closes the feature.

## Acceptance Criteria

- [ ] `frontend/src/routes/admin/feature-flags/+page.svelte` exists and renders a "Feature Flags" heading
- [ ] The page fetches all flags on mount via `FeatureFlagApi.getAll()` and shows a `Spinner` while loading
- [ ] Each flag is displayed as a row with its auto-formatted name (e.g. `DEMO_FEATURE_FLAG` → "Demo Feature Flag") and a `ToggleSwitch`
- [ ] Toggling a switch calls `FeatureFlagApi.toggle()` immediately; on failure the switch reverts to its previous state and a toast error is shown
- [ ] `e2e/steps/feature-flags-ui.steps.ts` step definitions are fully implemented (no `not implemented` stubs remaining)
- [ ] `e2e/pages/FeatureFlagsPage.ts` page object exposes all methods needed by the step definitions
- [ ] `task e2e:test` passes with all four `@feature-flags-*` scenarios GREEN

## Notes

- Flag name formatting: replace underscores with spaces and apply title case — a small pure utility function is sufficient; no library needed.
- `ToggleSwitch` is at `frontend/src/lib/ui/inputs/ToggleSwitch/ToggleSwitch.svelte` — check its props (likely `checked` + `onChange` or `onchange`).
- Toast infrastructure (`ToastWrapper`, message bus) follows the same pattern used in `system-config` — check `frontend/src/routes/admin/system-config/+page.svelte` for reference.
- The page object's `goto()` should navigate to `/admin/feature-flags`. Use existing page objects (e.g. `SystemConfigPage`) as structural reference.
- Run the full E2E suite (`task e2e:test`) at the end, not just the new scenarios, to catch any regression in the system-config tab navigation.
