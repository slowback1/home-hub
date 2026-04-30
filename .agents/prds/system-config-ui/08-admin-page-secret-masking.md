# Admin Page: Secret Value Masking

## Status

`done`

## Description

Mask config values where `isSecret` is true. Secret entries display `••••••••` by default with a show/hide toggle button. Toggling reveals the real value. The masking also applies when the row enters edit mode — the text input is pre-filled with the actual value but the display starts masked. This is the final implementation task; all 5 E2E scenarios must pass GREEN after it lands.

## Acceptance Criteria

- [ ] Rows with `isSecret: true` display `••••••••` instead of the real value by default
- [ ] A show/hide toggle button is visible on secret rows
- [ ] Clicking the toggle reveals the real value; clicking again re-masks it
- [ ] When a secret row enters edit mode (task 07), the text input is pre-filled with the actual value
- [ ] Non-secret rows are unaffected
- [ ] Unit/component tests cover: masked display, reveal toggle, re-masking, and edit-mode pre-fill for secret values
- [ ] `task e2e:test` passes with all 5 `@admin` scenarios GREEN

## Notes

The toggle can use the `Eye` and `EyeOff` icons from `lucide-svelte`.

The actual value is always present in the API response — masking is purely a display concern handled client-side.

Running `task e2e:test --grep @admin` to target only the admin scenarios is sufficient for the green gate check.
