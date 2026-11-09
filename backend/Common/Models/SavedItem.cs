using System;
using Common.Interfaces;

namespace Common.Models;

public class SavedItem : IIdentifyable
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
