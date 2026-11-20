using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace Logic.NbaSchedule;

// Official NBA public JSON API (no auth key required). The scores/v1/scorebox
// scoreboard endpoint returns scheduled games with team names and start times.
public class NbaScheduleProvider(HttpClient httpClient) : INbaScheduleProvider
{
    private const string BaseUrl = "https://stats.nba.com/scores/v1/scorebox";

    public async Task<IEnumerable<NbaScheduledGame>> GetScheduleAsync(DateTime from, DateTime to)
    {
        var dates = new List<string>();
        for (var date = from.Date; date <= to.Date; date = date.AddDays(1))
            dates.Add(date.ToString("yyyy-MM-dd"));

        var url = $"{BaseUrl}?leagueId=00&gameDates={string.Join(",", dates)}";
        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var payload = JsonSerializer.Deserialize<ScoreboxResponse>(json)
            ?? throw new InvalidOperationException("Empty response from NBA schedule API.");

        var eastern = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        var games = new List<NbaScheduledGame>();
        foreach (var game in payload.Scoreboard?.Games ?? [])
        {
            // seasonType: 2 = regular season, 3 = playoffs (admin decision: only these)
            if (game.SeasonType is not (2 or 3))
                continue;

            var easternStart = DateTime.ParseExact(
                game.StartDate, "yyyy-MM-ddTH:mm:ss", CultureInfo.InvariantCulture);
            games.Add(new NbaScheduledGame
            {
                SourceGameId = game.GameId,
                StartAt = DateTime.SpecifyKind(
                    TimeZoneInfo.ConvertTimeToUtc(easternStart, eastern), DateTimeKind.Utc),
                HomeTeam = game.HomeTeam?.TeamName ?? string.Empty,
                AwayTeam = game.AwayTeam?.TeamName ?? string.Empty,
            });
        }
        return games;
    }

    private sealed class ScoreboxResponse
    {
        [JsonPropertyName("scoreboard")] public ScoreboxScoreboard? Scoreboard { get; set; }
    }

    private sealed class ScoreboxScoreboard
    {
        [JsonPropertyName("games")] public ScoreboxGame[] Games { get; set; } = [];
    }

    private sealed class ScoreboxGame
    {
        [JsonPropertyName("gameId")] public string? GameId { get; set; }
        [JsonPropertyName("startDate")] public string StartDate { get; set; } = string.Empty;
        [JsonPropertyName("seasonType")] public int SeasonType { get; set; }
        [JsonPropertyName("homeTeam")] public ScoreboxTeam? HomeTeam { get; set; }
        [JsonPropertyName("awayTeam")] public ScoreboxTeam? AwayTeam { get; set; }
    }

    private sealed class ScoreboxTeam
    {
        [JsonPropertyName("teamName")] public string? TeamName { get; set; }
    }
}
