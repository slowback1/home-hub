namespace Common.Models;

public class SystemConfigOption
{
    public string SystemConfigId { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;

    public SystemConfig SystemConfig { get; set; } = null!;
}
