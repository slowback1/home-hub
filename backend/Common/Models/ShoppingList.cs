using System;
using Common.Interfaces;

namespace Common.Models;

public enum ShoppingListStatus
{
    Active,
    Archived
}

public class ShoppingList : IIdentifyable
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public ShoppingListStatus Status { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
