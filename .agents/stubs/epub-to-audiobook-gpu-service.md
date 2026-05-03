# Epub-to-Audiobook GPU Service

**Status:** stub
**Created:** 2026-05-03

## Summary

A standalone GPU microservice that wraps the [ai-epub-to-audiobook](https://github.com/slowback1/ai-epub-to-audiobook) Python script and exposes an HTTP API for TTS conversion jobs.

## Problem / Opportunity

The main home server cannot handle PyTorch / TTS model inference — it lacks the GPU resources required. Converting an epub to an audiobook needs to be offloaded to a separate GPU machine. A dedicated service decouples the heavy compute from the dashboard application.

## Success Looks Like

- A containerized Python service (FastAPI or similar) runs on a GPU server
- Accepts an epub file upload and conversion parameters via HTTP
- Returns a job ID; the dashboard can poll for status and retrieve the finished MP3/M4B on completion
- The service can be started/stopped independently of the main stack

## Notes & Open Questions

- Source script: https://github.com/slowback1/ai-epub-to-audiobook (PyTorch + a TTS model + ffmpeg)
- Which GPU server will host this? Needs CUDA drivers, ffmpeg, and sufficient VRAM for the TTS model
- Sync vs. async: long conversions likely need a job queue (Celery? simple task table?) rather than a blocking HTTP response
- Auth: should the service be locked to the home network, or does it need an API key?
- Output storage: does the service stream results back, or write to shared storage the dashboard can read?
- Dependency on the main project: the dashboard feature stub (epub-to-audiobook-ui) should be built after this service exists
