using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace Logic.NbaSchedule;

public class MockNbaScheduleProvider : INbaScheduleProvider
{
    private static readonly (string Home, string Away)[] Matchups =
    [
        ("Chicago Bulls", "Boston Celtics"),
        ("Los Angeles Lakers", "Golden State Warriors"),
        ("Milwaukee Bucks", "New York Knicks"),
    ];

    public Task<IEnumerable<NbaScheduledGame>> GetScheduleAsync(DateTime from, DateTime to)
    {
        var span = to - from;
        var games = new List<NbaScheduledGame>();
        for (var day = 1; day <= 3; day++)
        {
            var matchup = Matchups[day - 1];
            games.Add(new NbaScheduledGame
            {
                SourceGameId = $"mock-{day}",
                StartAt = from.AddTicks(span.Ticks * day / 4),
                HomeTeam = matchup.Home,
                AwayTeam = matchup.Away,
            });
        }
        return Task.FromResult<IEnumerable<NbaScheduledGame>>(games);
    }
}
