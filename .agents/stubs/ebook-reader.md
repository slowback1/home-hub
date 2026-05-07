# Ebook Reader

**Status:** stub
**Created:** 2026-05-07

## Summary

An in-app reader view that renders epub content chapter by chapter, allowing the user to read a selected book directly in the browser.

## Problem / Opportunity

Once a library exists, there is no way to actually read the books without a dedicated rendering layer. A reader view turns the HomeHub into a self-contained ebook experience rather than just a file index.

## Success Looks Like

- Navigating to a book from the library opens a `/library/:id/read` (or similar) reader route
- Epub content is rendered readably — HTML content extracted from the epub with basic typography styling
- The user can navigate forward and backward between chapters
- Reading position is optionally remembered (return to last position on re-open)

## Notes & Open Questions

- Epub rendering complexity is high: images, custom fonts, CSS, footnotes, tables — how much fidelity is required vs. a plain-text fallback?
- Client-side vs. server-side rendering: parse epub on backend and serve chapter HTML, or send the epub to the client and parse it in-browser (e.g. via epub.js)?
- epub.js is a mature client-side library worth evaluating — avoids backend parsing complexity
- Reading progress persistence: localStorage vs. backend storage
- Depends on: Ebook Library stub
