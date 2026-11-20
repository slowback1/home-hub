using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace InMemory;

public class InMemoryNbaScheduleRepository : INbaScheduleRepository
{
    private static readonly List<NbaGame> Games = [];

    public Task UpsertAsync(IEnumerable<NbaScheduledGame> games)
    {
        foreach (var game in games)
        {
            var existing = Games.FirstOrDefault(g => MatchesIdentity(g, game));
            if (existing != null)
            {
                existing.StartAt = game.StartAt;
                existing.HomeTeam = game.HomeTeam;
                existing.AwayTeam = game.AwayTeam;
                continue;
            }

            Games.Add(new NbaGame
            {
                Id = Guid.NewGuid().ToString(),
                SourceGameId = game.SourceGameId,
                StartAt = game.StartAt,
                HomeTeam = game.HomeTeam,
                AwayTeam = game.AwayTeam,
            });
        }
        return Task.CompletedTask;
    }

    public Task PruneBeforeAsync(DateTime cutoff)
    {
        Games.RemoveAll(g => g.StartAt < cutoff);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<NbaGame>> GetRangeAsync(DateTime from, DateTime to)
    {
        var results = Games
            .Where(g => g.StartAt >= from && g.StartAt <= to)
            .AsEnumerable();
        return Task.FromResult(results);
    }

    public Task ClearAllAsync()
    {
        Games.Clear();
        return Task.CompletedTask;
    }

    public static void ClearStaticState() => Games.Clear();

    private static bool MatchesIdentity(NbaGame existing, NbaScheduledGame incoming) =>
        existing.SourceGameId != null || incoming.SourceGameId != null
            ? existing.SourceGameId == incoming.SourceGameId
            : existing.StartAt == incoming.StartAt
              && existing.HomeTeam == incoming.HomeTeam
              && existing.AwayTeam == incoming.AwayTeam;
}
