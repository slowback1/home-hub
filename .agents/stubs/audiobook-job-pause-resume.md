# Audiobook Job Pause / Resume

**Status:** stub
**Created:** 2026-05-06

## Summary

Allow a running audiobook conversion to be paused (freeing the GPU) and later resumed, both from the UI and potentially via a backend API for programmatic preemption.

## Problem / Opportunity

The conversion process monopolises the GPU for the duration of a job, which can be hours for a full book. When a shorter-priority inference task needs the GPU, there is no way to yield without cancelling the conversion entirely and losing progress.

## Success Looks Like

- A Pause button appears on in-progress jobs; clicking it suspends the conversion subprocess (e.g. `SIGSTOP` to the process group) and updates job status to `paused`
- A Resume button appears on paused jobs; clicking it sends `SIGCONT` and returns status to `in_progress`
- The conversion picks up mid-chapter (already works via the existing resume-chapter detection logic)
- Optionally: a backend-only `POST /api/jobs/:id/pause` endpoint that can be called by other services or scripts without going through the UI

## Notes & Open Questions

- `SIGSTOP`/`SIGCONT` affect the whole process group — verify the subprocess spawning uses a process group so child ffmpeg/python processes are also suspended
- The worker currently polls every 5 seconds; pausing should be near-instantaneous so the signal approach is preferable to a polling flag
- Should the GPU service expose a pause endpoint to the main backend, or only to the frontend directly?
- If the server restarts while a job is `paused`, should it auto-resume or stay paused on boot?
