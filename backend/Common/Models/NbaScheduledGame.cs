using System;

namespace Common.Models;

public class NbaScheduledGame
{
    public string? SourceGameId { get; set; }
    public DateTime StartAt { get; set; }
    public string HomeTeam { get; set; } = string.Empty;
    public string AwayTeam { get; set; } = string.Empty;
}
