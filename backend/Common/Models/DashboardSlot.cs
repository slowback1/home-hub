using Common.Interfaces;

namespace Common.Models;

public class DashboardSlot : IIdentifyable
{
    public string Id { get; set; } = string.Empty;
    public string LayoutFormat { get; set; } = string.Empty;
    public int SlotIndex { get; set; }
    public string? WidgetType { get; set; }
}
