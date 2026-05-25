using System;
using Common.Models;

namespace Common.Interfaces;

public interface IRecurrenceCalculator
{
    DateTime GetNextDoDate(ChoreTask task, DateTime completedAt);
}
