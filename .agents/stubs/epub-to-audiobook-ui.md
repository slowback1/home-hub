# Epub-to-Audiobook UI

**Status:** stub
**Created:** 2026-05-03

## Summary

A dashboard page where the user can upload an epub, trigger conversion via the GPU TTS service, and download or stream the resulting audiobook.

## Problem / Opportunity

Converting an epub to an audiobook manually requires running a script by hand on a separate machine. Integrating it into the home dashboard gives a single-UI workflow: upload, wait, listen.

## Success Looks Like

- User can upload an epub through the dashboard UI
- The backend submits the conversion job to the GPU TTS service and tracks its status
- The UI polls and shows conversion progress (queued → processing → done)
- On completion, the user can download or stream the finished audiobook file
- Conversion history is visible (at minimum: filename, status, timestamp)

## Notes & Open Questions

- **Depends on:** epub-to-audiobook-gpu-service stub — the GPU service must exist first
- Where are completed audiobook files stored? (shared volume, object storage, served directly from the GPU box?)
- Should the user be able to configure TTS voice/speed/model from the UI, or hard-coded defaults for now?
- How large are typical epub → audiobook outputs? Need to consider file size limits in the upload path
- Playback in-browser (audio element) vs. download-only?
- Should conversions survive a dashboard restart? (implies persisting job state in the DB)
