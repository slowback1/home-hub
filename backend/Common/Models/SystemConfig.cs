using Common.Interfaces;

namespace Common.Models;

public class SystemConfig : IIdentifyable
{
    public string Id { get; set; } = string.Empty;
    public string Namespace { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsSecret { get; set; }
}
