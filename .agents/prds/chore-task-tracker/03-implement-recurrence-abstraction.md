# Implement Recurrence Abstraction

## Status

`pending`

## Description

Define the `IRecurrenceCalculator` interface and implement `IntervalDaysRecurrenceCalculator`, which calculates the next DoDate for a recurring task by adding its `IntervalDays` to the completion date. Register the implementation in DI. Unit tests verify the calculation.

## Acceptance Criteria

- [ ] `IRecurrenceCalculator` interface exists in `Common/Interfaces/` (or `Logic/`) with a single method `DateOnly GetNextDoDate(ChoreTask task, DateTime completedAt)`
- [ ] `IntervalDaysRecurrenceCalculator` implements the interface: returns `DateOnly.FromDateTime(completedAt).AddDays(task.IntervalDays!.Value)`
- [ ] `IntervalDaysRecurrenceCalculator` is registered in `Program.cs` DI as `IRecurrenceCalculator`
- [ ] Unit tests cover: standard interval (e.g. 7 days), 1-day interval, and behaviour when `IntervalDays` is null (should not be called, but guard or throw clearly)
- [ ] `dotnet test` passes

## Notes

- This is pure logic with no DB dependency — keep it in `Logic/` or `Common/`
- The interface placement should allow `Logic/` use cases to depend on it without referencing EF or WebAPI projects
- The name `ChoreTask` must be used (not `Task`) throughout to avoid `System.Threading.Tasks.Task` conflicts
