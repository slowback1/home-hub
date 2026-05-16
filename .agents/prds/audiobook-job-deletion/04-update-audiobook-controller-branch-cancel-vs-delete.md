# Update AudiobookController to Branch Cancel vs Delete

## Status

`done`

## Description

Update the `DELETE /api/audiobook/jobs/{id}` endpoint to be context-sensitive: cancel the job if it is active (Queued or InProgress), delete the job record if it is terminal (Completed, Failed, Cancelled). Add integration tests covering all branches.

## Acceptance Criteria

- [ ] `DELETE /jobs/{id}` on a Queued job → 204, job is cancelled (existing behaviour preserved)
- [ ] `DELETE /jobs/{id}` on an InProgress job → 204, job is cancelled (existing behaviour preserved)
- [ ] `DELETE /jobs/{id}` on a Completed job → 204, subsequent `GET /jobs/{id}` returns 404
- [ ] `DELETE /jobs/{id}` on a Failed job → 204, subsequent `GET /jobs/{id}` returns 404
- [ ] `DELETE /jobs/{id}` on a Cancelled job → 204, subsequent `GET /jobs/{id}` returns 404
- [ ] `DELETE /jobs/{id}` on an unknown id → 404
- [ ] Integration tests in `AudiobookControllerTests` cover all branches above
- [ ] All existing tests continue to pass

## Notes

- The controller should check the job's current status after fetching it, then call `CancelJobAsync` or `DeleteJobAsync` accordingly.
- `KeyNotFoundException` from the service should map to 404; `InvalidOperationException` should map to 409 (consistent with existing cancel error handling).
