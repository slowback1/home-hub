using System;
using Common.Interfaces;

namespace Common.Models;

public class TaskCompletion : IIdentifyable
{
    public string Id { get; set; } = string.Empty;
    public string TaskId { get; set; } = string.Empty;
    public DateTime CompletedAt { get; set; }
    public DateTime? PreviousDoDate { get; set; }
}
