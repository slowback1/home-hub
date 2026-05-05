# /audiobook Layout, Tab Navigation, and Sidebar Nav Item

## Status

`pending`

## Description

Create the `/audiobook` route section with a shared layout that renders Convert/Voice Samples tab navigation, and add the audiobook entry to the sidebar behind the `AUDIOBOOK_ENABLED` feature flag. No page content yet — just the navigation shell.

## Acceptance Criteria

- [ ] `frontend/src/routes/audiobook/+layout.svelte` exists with two tabs: "Convert" (href `/audiobook`) and "Voice Samples" (href `/audiobook/voice-samples`), styled to match the admin layout pattern
- [ ] Active tab is highlighted based on `$page.url.pathname`, with the Convert tab active for exact match on `/audiobook` and the Voice Samples tab active for `/audiobook/voice-samples`
- [ ] `frontend/src/routes/audiobook/+page.svelte` exists as a placeholder (can be empty or minimal) so the `/audiobook` route resolves without a 404
- [ ] `frontend/src/routes/audiobook/voice-samples/+page.svelte` exists as a placeholder for the same reason
- [ ] `Sidebar.svelte` has a new nav item: icon `BookAudio`, label "Audiobook", href `/audiobook`, active path prefix `/audiobook`, test ID `nav-item-audiobook`, gated on `AUDIOBOOK_ENABLED` feature flag
- [ ] With `AUDIOBOOK_ENABLED` disabled (default), the nav item does not appear in the sidebar
- [ ] With `AUDIOBOOK_ENABLED` enabled, the nav item appears and navigating to `/audiobook` renders the tab shell

## Notes

Copy the `+layout.svelte` structure from `frontend/src/routes/admin/+layout.svelte` — same tab rendering loop, same active-state logic, same CSS class names. The only differences are the tab definitions and the wrapping element's class name.

`BookAudio` is available in `lucide-svelte` — verify the import name matches the installed version before using it.

The feature flag check in `Sidebar.svelte` follows the same pattern as the other flagged nav items (e.g. `WEATHER_ENABLED`).
