# PRD: Bookmarks

## Status

`Draft`

## Overview

A bookmarks page in HomeHub that centralizes frequently visited links — home-lab services, tools, and external sites — as a grid of clickable cards with auto-fetched favicons. Users can add, edit, delete, and star bookmarks via modals on the page, with client-side search to filter as the list grows.

## Problem Statement

Frequently visited links (home-lab services, tools, sites) are scattered across browser bookmarks on different devices. There is no single, always-accessible, self-hosted place to find them. A HomeHub bookmarks page solves this by keeping all saved links in one location, persisted in the backend database.

## Goals

- Display all saved bookmarks as a grid of cards at `/bookmarks`
- Each card shows the bookmark's title, hostname, and favicon (auto-fetched — no manual upload)
- Clicking a card opens the link in a new tab
- Users can add, edit (title, URL, description), and delete bookmarks via modals
- Users can star/unstar bookmarks and filter to starred-only
- Client-side search filters cards by name, URL, or description
- Cards are sorted alphabetically by name
- Bookmarks persist in the backend database (PostgreSQL)
- Favicon fetch failures degrade gracefully (letter-tile fallback using the first character of the bookmark name)
- Intranet URLs (e.g. `192.168.x.x`) that cannot be fetched by the Google favicon service fall back to the same letter-tile

## Non-Goals

- Bookmark grouping or categories (v2)
- Drag-to-reorder or manual sort order (v2)
- Grid / list layout toggle (v2)
- Keyboard shortcuts for add / search
- Backend favicon caching or proxy (v1 uses the Google favicon service client-side)

## User Stories / Use Cases

- **As a** HomeHub user, **I want to** see all my saved links as cards, **so that** I can quickly navigate to frequently visited services.
- **As a** HomeHub user, **I want to** add a new bookmark by entering a URL and optional name/description, **so that** I can save links without managing browser bookmarks.
- **As a** HomeHub user, **I want to** edit a bookmark's name, URL, or description, **so that** I can keep my list accurate.
- **As a** HomeHub user, **I want to** delete a bookmark with a confirmation dialog, **so that** I don't lose links by accident.
- **As a** HomeHub user, **I want to** star important bookmarks, **so that** I can filter down to the ones I use most.
- **As a** HomeHub user, **I want to** search by name, URL, or description, **so that** I can find a specific link quickly as the list grows.

## E2E Scenarios

```gherkin
@bookmarks
Feature: Bookmarks

  @view-bookmarks-page
  Scenario: View bookmarks page shows saved bookmarks as cards
    Given I have saved bookmarks
    When I navigate to the bookmarks page
    Then I should see each bookmark displayed as a card with its title and favicon

  @add-bookmark-happy-path
  Scenario: Add a bookmark with title, URL, and description
    Given I am on the bookmarks page
    When I open the add bookmark modal and submit a URL, name, and description
    Then the new bookmark card should appear on the page

  @add-bookmark-url-normalization
  Scenario: URL without a protocol is normalized to https
    Given I am on the bookmarks page
    When I add a bookmark with the URL "github.com" and no protocol
    Then the bookmark should be saved with the URL "https://github.com"

  @edit-bookmark
  Scenario: Edit a bookmark name and description
    Given I have a saved bookmark
    When I open the edit modal and change the name and description
    Then the card should reflect the updated values

  @delete-bookmark-confirmed
  Scenario: Delete a bookmark after confirming the dialog
    Given I have a saved bookmark
    When I click delete and confirm the confirmation dialog
    Then the bookmark card should no longer appear on the page

  @delete-bookmark-cancelled
  Scenario: Cancel delete confirmation leaves the bookmark intact
    Given I have a saved bookmark
    When I click delete and cancel the confirmation dialog
    Then the bookmark card should still appear on the page

  @star-bookmark
  Scenario: Star a bookmark toggles its starred state
    Given I have a saved bookmark that is not starred
    When I click the star button on the card
    Then the bookmark should be marked as starred

  @search-bookmarks
  Scenario: Search filters visible bookmark cards
    Given I have multiple saved bookmarks
    When I type a search term that matches only one bookmark
    Then only the matching bookmark card should be visible

  @bookmarks-empty-state
  Scenario: Empty state is shown when no bookmarks exist
    Given I have no saved bookmarks
    When I navigate to the bookmarks page
    Then I should see an empty state message
```

## Proposed Solution

A new `/bookmarks` route backed by a dedicated backend CRUD resource. The frontend renders bookmarks as a grid of cards (using the existing HomeHub design system) with an add/edit modal, delete confirmation dialog, star toggle, and a client-side search bar. Favicons are fetched client-side via the Google favicon service with a letter-tile fallback component for failures.

## Design

**Handoff:** https://api.anthropic.com/v1/design/h/Ah75Oj1nDmsuyAufxtukhA?open_file=index.html

Key visual decisions from the design session:

- **Favicon tile**: Uniform muted swatch (`--color-surface-deep` background, secondary-text letter) — not a per-site hue. The tile is hidden entirely once a real favicon image loads; it only shows while loading, on error, or when no fetch is attempted.
- **Intranet URL detection**: Pattern-based (`.local` suffix, RFC 1918 ranges `192.168.*`, `10.*`, `172.16–31.*`, `127.*`) — skips the Google favicon fetch entirely and goes straight to the letter tile.
- **Delete UX**: Confirmation dialog first, then a 5-second undo toast after the deletion is committed. Both are present.
- **Toolbar visibility**: The search bar and starred-only pill are hidden when the list is completely empty; only the Add Bookmark button and the zero-state CTA are shown.
- **Two distinct empty states**: (1) zero bookmarks — large CTA with Bookmark icon and Add button; (2) search/filter mismatch — compact "no matches" message with a "Clear filters" link.
- **Results summary line**: Rendered between the toolbar and the grid: _"X of Y bookmarks, starred only, matching 'query'"_ — omits clauses that don't apply.
- **Card layout**: favicon + title/hostname block + star button (top row), optional description (middle), edit/delete action buttons (bottom). Hostname includes a small external-link icon.

## Technical Approach

### Data Model

`Bookmark` entity (persisted via EF Core / PostgreSQL):

| Field | Type | Notes |
|---|---|---|
| `Id` | `string` (GUID) | Primary key |
| `Name` | `string` | Display title; defaults to the hostname if blank on save |
| `Url` | `string` | Required; normalized to include `https://` if no protocol provided |
| `Description` | `string?` | Optional freeform note |
| `Starred` | `bool` | Default `false` |
| `CreatedAt` | `DateTime` (UTC) | Set on creation |

### API Endpoints

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/bookmarks` | List all bookmarks, sorted alphabetically by name |
| `POST` | `/api/bookmarks` | Create a bookmark |
| `PUT` | `/api/bookmarks/{id}` | Update name, URL, description |
| `DELETE` | `/api/bookmarks/{id}` | Delete a bookmark |
| `PATCH` | `/api/bookmarks/{id}/star` | Toggle starred (returns updated bookmark) |

### Key Decisions

| Decision | Chosen Approach | Rationale |
|---|---|---|
| Favicon fetching | Client-side via Google favicon service (`https://www.google.com/s2/favicons?domain=...&sz=64`) | No backend changes needed; acceptable for public URLs |
| Intranet / fetch-failure fallback | Letter-tile (first char of name, uniform `--color-surface-deep` swatch); tile hidden once real favicon loads | Flat navy aesthetic; more distinctive than a generic globe; purely client-side |
| Intranet URL detection | Pattern-based: `.local` suffix + RFC 1918 ranges (`192.168.*`, `10.*`, `172.16–31.*`, `127.*`) | Skips favicon fetch entirely rather than making a doomed request |
| Sort order | Alphabetical by name (server returns unsorted; client sorts) | Simple; no drag-to-reorder in v1 |
| Star toggle | Dedicated `PATCH /api/bookmarks/{id}/star` endpoint | Follows existing pattern (`POST /api/tasks/{id}/complete`); avoids full payload for a boolean flip |
| Delete UX | Confirmation dialog + 5-second undo toast after commit | Dialog prevents accidents; toast provides a recovery window without blocking the UI |
| URL normalization | Frontend prepends `https://` if no protocol present before saving | Keeps backend simple; catches the common "forgot the protocol" case |
| Sidebar nav | `Bookmark` icon (lucide-svelte), behind `BOOKMARKS_ENABLED` feature flag | Consistent with all other feature pages |
| Management UI | Add/edit modal inline on `/bookmarks` | No separate route needed; modal pattern is already established |

### Feature Flag

`BOOKMARKS_ENABLED` — disabled by default, consistent with all other feature flags. Users enable it via the admin panel.

### E2E Test Setup

Following the pattern established by `audiobook.steps.ts`:

- `Before({ tags: '@bookmarks' })` — enable `BOOKMARKS_ENABLED` via `PATCH /api/feature-flags/BOOKMARKS_ENABLED` and clear all bookmarks via `DELETE /api/test/bookmarks`
- `After({ tags: '@bookmarks' })` — delete test data and disable `BOOKMARKS_ENABLED`

This requires a `DELETE /api/test/bookmarks` test-helper endpoint (consistent with `/api/test/tasks` and `/api/test/activities`).

### Dependencies

- Google Favicon Service (`https://www.google.com/s2/favicons`) — public, no API key; unavailable for intranet-only URLs
- Lucide Svelte — `Bookmark` icon for sidebar nav
- EF Core / PostgreSQL — existing infrastructure

## Open Questions

_None._

## Out of Scope

- Bookmark grouping / categories
- Drag-to-reorder or manual sort order
- Grid / list layout toggle
- Bulk import (e.g. from a browser bookmark export)
- Backend favicon caching or proxy

## Success Metrics

- All saved bookmarks load and display correctly on page visit
- Favicon loads for standard public URLs; fallback renders for intranet/unreachable URLs
- All E2E scenarios pass

## Timeline / Milestones

_TBD_
