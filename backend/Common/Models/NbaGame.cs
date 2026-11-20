using System;
using Common.Interfaces;

namespace Common.Models;

public class NbaGame : IIdentifyable
{
    public string Id { get; set; } = string.Empty;
    public string? SourceGameId { get; set; }
    public DateTime StartAt { get; set; }
    public string HomeTeam { get; set; } = string.Empty;
    public string AwayTeam { get; set; } = string.Empty;
}
