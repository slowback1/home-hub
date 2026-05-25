using System;
using Common.Interfaces;
using Common.Models;

namespace Logic.Recurrence;

public class IntervalDaysRecurrenceCalculator : IRecurrenceCalculator
{
    public DateTime GetNextDoDate(ChoreTask task, DateTime completedAt)
    {
        if (task.IntervalDays is null or < 1)
            throw new InvalidOperationException("IntervalDays must be set and >= 1 for recurring tasks.");

        return completedAt.Date.AddDays(task.IntervalDays.Value);
    }
}
