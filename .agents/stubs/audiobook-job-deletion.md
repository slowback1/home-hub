# Audiobook Job Deletion

**Status:** stub
**Created:** 2026-05-06

## Summary

Allow users to delete completed or failed audiobook conversion jobs, removing the record and any associated output files from disk.

## Problem / Opportunity

Failed and completed jobs accumulate in the queue list with no way to clean them up. This clutters the UI and wastes disk space from partial or finished output files.

## Success Looks Like

- A delete button appears on jobs in `completed` or `failed` status
- Clicking it removes the job record from the database and deletes the job's output directory on the GPU server
- The job disappears from the queue list immediately after deletion
- In-progress or queued jobs cannot be deleted (button absent or disabled)

## Notes & Open Questions

- Should deletion also remove the source epub that was uploaded, or only the output?
- Consider a confirmation prompt before deleting completed jobs (output files may be wanted)
- Backend: `DELETE /api/jobs/:id` endpoint on the GPU service; cascades to filesystem cleanup
