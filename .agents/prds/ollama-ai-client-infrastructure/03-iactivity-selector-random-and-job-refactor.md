# IActivitySelector Interface, RandomActivitySelector, and ActivityPickerJob Refactor

## Status

`done`

## Description

Define the `IActivitySelector` strategy interface in Common, extract the existing weighted-random algorithm from `ActivityPickerJob` into a `RandomActivitySelector`, and refactor `ActivityPickerJob` to inject `IActivitySelector`. The job now computes the recent-pick window size (`N × 2` where N is the number of unique activities), fetches recent picks via `GetRecentAsync`, and delegates selection to the injected selector. No behaviour changes for users — the random path is preserved exactly.

## Acceptance Criteria

- [ ] `IActivitySelector` interface exists in `backend/Common/Interfaces/` with method `Task<Activity> SelectAsync(IList<Activity> activities, IList<ActivityPick> recentPicks)`
- [ ] `RandomActivitySelector` in `backend/Logic/ActivityPicker/` implements `IActivitySelector` using the same weighted-random algorithm previously in `ActivityPickerJob`
- [ ] `ActivityPickerJob` no longer contains the `WeightedRandom` method — it is fully removed
- [ ] `ActivityPickerJob` injects `IActivitySelector` and `IActivityPickRepository`
- [ ] `ActivityPickerJob.ExecuteAsync` fetches all activities (count = N), calls `GetRecentAsync(N * 2)`, then calls `selector.SelectAsync(activities, recentPicks)`
- [ ] End-to-end behavior of the job is unchanged when `RandomActivitySelector` is the injected implementation
- [ ] Unit tests for `RandomActivitySelector` cover: weighted distribution is respected, single activity always selected, all activities eligible
- [ ] Unit tests for the refactored `ActivityPickerJob` verify it calls the selector with the correct activity list and recent picks

## Notes

- Current `WeightedRandom` is a private static method in `backend/Logic/ActivityPicker/ActivityPickerJob.cs`
- `RandomActivitySelector` should be `Task`-based to satisfy the interface, even though the random selection itself is synchronous — wrap in `Task.FromResult`
- `recentPicks` is passed through to the selector even in the random implementation, which ignores it — this keeps the interface uniform
