using System;
using Common.Interfaces;

namespace Common.Models;

public class Bookmark : IIdentifyable
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool Starred { get; set; }
    public DateTime CreatedAt { get; set; }
}
