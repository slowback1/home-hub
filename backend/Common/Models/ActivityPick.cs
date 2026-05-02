using System;

namespace Common.Models;

public class ActivityPick
{
    public string Id { get; set; } = string.Empty;
    public DateTime PickedAt { get; set; }
    public string ActivityName { get; set; } = string.Empty;
}
