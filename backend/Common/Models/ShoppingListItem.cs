using System;
using Common.Interfaces;

namespace Common.Models;

public class ShoppingListItem : IIdentifyable
{
    public string Id { get; set; } = string.Empty;
    public string ListId { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool IsChecked { get; set; }
    public DateTime CreatedAt { get; set; }
}
