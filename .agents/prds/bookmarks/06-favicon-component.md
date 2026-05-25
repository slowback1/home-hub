# Favicon Component

## Status

`done`

## Description

Build the reusable `Favicon` Svelte component that fetches a site's favicon via the Google favicon service and falls back to a uniform letter-tile when the fetch fails or the URL is an intranet address. This component is used inside BookmarkCard (task 07).

## Acceptance Criteria

- [ ] `Favicon.svelte` component accepts `url: string` and `name: string` props (and optional `size` in px, default 40)
- [ ] For public URLs, renders a lazy-loaded `<img>` pointed at `https://www.google.com/s2/favicons?domain={hostname}&sz=64`
- [ ] On image load error, falls back to the letter tile
- [ ] Letter tile uses `--color-surface-deep` background and secondary-text color; shows the first alphanumeric character of `name` (uppercased)
- [ ] Letter tile is hidden once a real favicon image loads (not just a coloured background behind the image)
- [ ] Intranet URLs are detected by pattern and skip the favicon fetch entirely, showing the letter tile directly:
  - `.local` suffix
  - RFC 1918: `192.168.*`, `10.*`, `172.16–31.*`, `127.*`
- [ ] Component renders at the correct size and does not break layout when the image is loading

## Notes

- Place in `frontend/src/lib/ui/` following the existing component organisation
- The letter tile should be `--color-surface-deep` background with `--color-text-secondary` letter — uniform across all bookmarks (not per-site hue)
- Use Svelte's reactive `bind` or `on:error`/`on:load` handlers for the image state
- Reference the design handoff for exact tile behaviour: `https://api.anthropic.com/v1/design/h/Ah75Oj1nDmsuyAufxtukhA`
