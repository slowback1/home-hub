using Common.Interfaces;

namespace Common.Models;

public class Activity : IIdentifyable
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Weight { get; set; } = 1;
}
