# Bookmarks

**Status:** stub
**Created:** 2026-05-25

## Summary

A configurable bookmarks page in HomeHub that displays clickable cards for saved links, fetches favicons automatically for visual identification, and includes full CRUD management for adding, editing, and deleting bookmarks.

## Problem / Opportunity

Frequently visited links (home lab services, tools, sites) are scattered across browser bookmarks across devices. A HomeHub bookmarks page centralizes them in one always-accessible, self-hosted place.

## Success Looks Like

- A `/bookmarks` route displays all saved bookmarks as cards
- Each card shows the bookmark's title, URL, and favicon (auto-fetched — no manual upload required)
- Clicking a card opens the link (new tab)
- User can add, edit (title + URL), and delete bookmarks via a management UI (modal or separate `/bookmarks/manage` route — TBD during PRD)
- Bookmarks persist in the backend database (not localStorage)
- Page loads quickly even with many bookmarks; favicon fetch failures degrade gracefully (fallback icon)

## Notes & Open Questions

- **Favicon fetching**: Can be done entirely client-side using a public favicon service (e.g. `https://www.google.com/s2/favicons?domain=example.com`). No backend image fetching or caching needed in v1, which keeps the implementation simple. Downside: depends on Google's service being available and may not work for intranet-only URLs.
- **Intranet links**: If the user has local network services (e.g. `http://192.168.1.x`) as bookmarks, the public favicon service won't work for those. A fallback icon (generic globe or site initial) is fine for v1.
- **Grouping / ordering**: Should bookmarks be groupable by category, or just a flat list? Flat list is simpler; drag-to-reorder or manual sort order could be a v2.
- **Management UI location**: Inline add/edit modal on the bookmarks page vs. a separate `/bookmarks/manage` route — TBD. Modal keeps it simple.
- **Open in same tab vs. new tab**: New tab is probably the right default for a dashboard context.
- **No splits needed** — backend CRUD + frontend page are tightly coupled and small enough to live in one PRD.
