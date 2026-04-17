# Ollama / AI Client Infrastructure

**Status:** stub
**Created:** 2026-04-17

## Summary

A ready-to-use client layer for communicating with the Ollama instance on the home network, so future features can invoke local AI models without reinventing the integration each time.

## Problem / Opportunity

Without a shared AI client layer, every feature that wants to use Ollama would need to implement its own HTTP calls, error handling, and model selection logic. A single well-structured client makes AI-powered features cheap to add.

## Success Looks Like

- A typed client module exists that wraps the Ollama API (chat, completion, embeddings as needed)
- Model selection is configurable (env var or config file) without code changes
- The client handles errors and timeouts gracefully
- At least one example feature or endpoint demonstrates the integration working end-to-end

## Notes & Open Questions

- Which Ollama endpoints are in scope for v1? (chat completions, embeddings, both?)
- Streaming responses — in scope now or deferred?
- Should the client be server-side only, or also callable from the frontend via an API route?
- Authentication/network isolation — is the Ollama host reachable directly or proxied through the app?
