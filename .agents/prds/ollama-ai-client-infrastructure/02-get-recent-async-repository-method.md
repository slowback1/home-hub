# GetRecentAsync Repository Method

## Status

`done`

## Description

Add a `GetRecentAsync(int count)` method to `IActivityPickRepository` and implement it in both concrete repositories. This method is needed by the refactored `ActivityPickerJob` (task 03) to fetch the most recent N picks for use as AI context. The EF implementation must order by `PickedAt` descending and take `count` rows; the in-memory implementation must do the same against its in-memory store.

## Acceptance Criteria

- [ ] `IActivityPickRepository` in `backend/Common/Interfaces/` has `Task<IEnumerable<ActivityPick>> GetRecentAsync(int count)`
- [ ] `EfActivityPickRepository` implements the method: orders by `PickedAt` descending, takes `count` rows
- [ ] `InMemoryActivityPickRepository` implements the method with equivalent ordering
- [ ] Calling with `count = 0` returns an empty collection without error
- [ ] Calling when fewer than `count` picks exist returns all available picks
- [ ] Unit tests for both implementations cover: returns correct count, returns picks in descending order, handles fewer-than-count case, handles zero count

## Notes

- Interface: `backend/Common/Interfaces/IActivityPickRepository.cs`
- EF implementation: `backend/EntityFramework/EfActivityPickRepository.cs`
- In-memory implementation: `backend/InMemory/InMemoryActivityPickRepository.cs`
- Existing tests for both implementations live in `backend/EntityFramework.Tests/` and `backend/Common.Tests/` (or `Logic.Tests/`) — follow the existing test file pattern
