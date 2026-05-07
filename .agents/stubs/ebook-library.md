# Ebook Library

**Status:** stub
**Created:** 2026-05-07

## Summary

A backend service that scans a configurable directory tree for epub files and exposes them via API, paired with a frontend library view where the user can browse and select a book.

## Problem / Opportunity

Epub files stored on the server have no way to be browsed or accessed from the HomeHub UI. A library layer would make the collection discoverable and serve as the entry point for both reading and audiobook conversion.

## Success Looks Like

- A configurable root directory (set via system config or env var) is scanned recursively for `.epub` files
- The backend exposes an API listing discovered books with metadata (title, author, file path, cover image if extractable)
- The frontend has a `/library` route showing the collection as a browsable grid or list
- Selecting a book navigates to the reader view (or surfaces other actions like "Convert to Audiobook")

## Notes & Open Questions

- How is the library kept in sync — on-demand scan per request, periodic background scan, or filesystem watcher?
- Should metadata be persisted to the database or always derived live from the epub files?
- Cover image extraction may require ebooklib (already a dependency in the GPU service) — does the main backend need it too, or does a separate service handle this?
- Nested directory structure: does the UI reflect the folder hierarchy, or flatten everything into one list with search/filter?
- This is a natural integration point for the audiobook conversion flow — a "Convert" action on a book could pre-fill the Convert tab
