using System;
using Common.Interfaces;

namespace Common.Models;

public class WalkSession : IIdentifyable
{
    public string Id { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public int DurationSeconds { get; set; }
    public int StepCount { get; set; }
    public DateTime SyncedAt { get; set; }
}
