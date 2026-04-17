using Common.Interfaces;

namespace Common.Models;

public class ExampleData : IIdentifyable
{
    public string Name { get; set; } = string.Empty;
    public int Value { get; set; }
    public string Id { get; set; } = string.Empty;
}