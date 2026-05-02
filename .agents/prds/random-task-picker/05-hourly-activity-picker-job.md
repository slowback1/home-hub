# Hourly Activity Picker Job

## Status

`done` <!-- pending | in-progress | done -->

## Description

Implement the Hangfire recurring job that runs at the top of every hour, randomly selects one activity using weighted probability, and writes a snapshot record to the `ActivityPick` table. This is the core business logic of the feature. If the activity list is empty, the job exits silently with no record written.

## Acceptance Criteria

- [ ] `ActivityPickerJob` class exists in the backend with a single public method (e.g. `ExecuteAsync`)
- [ ] Weighted random selection is implemented: an activity with weight 3 is 3× more likely to be selected than one with weight 1
- [ ] On selection, an `ActivityPick` record is written via `IActivityPickRepository` with the activity's name (snapshot) and `PickedAt` set to UTC now
- [ ] If the activity list is empty, the job returns without writing any record and without throwing
- [ ] The job is registered as a Hangfire recurring job in `Program.cs` with cron expression `0 * * * *`
- [ ] No pick is triggered on application startup
- [ ] The job class is covered by unit tests: weighted selection distribution, empty-list no-op, and record-writing behavior

## Notes

- Weighted selection algorithm: build a flat list where each activity appears `Weight` times, then pick a random index — or use a running-sum approach. Either is fine.
- The job should depend on `IActivityRepository` (or equivalent from task 03) and `IActivityPickRepository` (task 04) via constructor injection
- Hangfire will handle retries on exception; the job itself does not need retry logic
- Registration line: `RecurringJob.AddOrUpdate<ActivityPickerJob>("activity-picker", j => j.ExecuteAsync(), "0 * * * *")`
