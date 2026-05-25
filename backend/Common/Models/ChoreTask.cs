using System;
using Common.Interfaces;

namespace Common.Models;

public class ChoreTask : IIdentifyable
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsRecurring { get; set; }
    public int? IntervalDays { get; set; }
    public DateTime? DoDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
