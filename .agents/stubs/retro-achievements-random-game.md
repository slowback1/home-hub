# RetroAchievements Random Game

**Status:** stub
**Created:** 2026-04-17

## Summary

An integration with the RetroAchievements API that returns a random game from a specified console, rewritten from a previous project into HomeHub.

## Problem / Opportunity

The previous standalone version of this tool worked but lived in isolation. Bringing it into HomeHub gives it a proper home with shared infrastructure and UI, and the rewrite is an opportunity to improve its functionality and presentation.

## Success Looks Like

- A page allows the user to select a console and receive a randomly picked game from RetroAchievements
- Game details (title, cover art, achievement count, etc.) are displayed
- The integration handles API errors and rate limits gracefully
- Console selection is persistent or easy to re-use

## Notes & Open Questions

- RetroAchievements API requires an API key — where is this stored? (env var / config)
- Should the random pick be purely random, or weighted/filtered (e.g. exclude already completed games)?
- Is there a concept of a "collection" — only pick from games the user owns or has played?
- Should results be cached to avoid hammering the API on every pick?
- Any integration with the Ollama AI layer for game suggestions deferred to a future stub
