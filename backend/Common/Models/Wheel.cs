using System;
using Common.Interfaces;

namespace Common.Models;

public class Wheel : IIdentifyable
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    /// <summary>Newline-delimited list of items (one per line).</summary>
    public string Items { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
