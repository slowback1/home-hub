using System;
using Common.Interfaces;

namespace Common.Models;

public class PremadeListItem : IIdentifyable
{
    public string Id { get; set; } = string.Empty;
    public string PremadeListId { get; set; } = string.Empty;
    public string SavedItemId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
