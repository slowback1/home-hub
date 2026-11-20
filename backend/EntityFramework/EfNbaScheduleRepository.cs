using Common.Interfaces;
using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework;

public class EfNbaScheduleRepository(AppDbContext db) : INbaScheduleRepository
{
    public async Task UpsertAsync(IEnumerable<NbaScheduledGame> games)
    {
        foreach (var game in games)
        {
            var existing = await FindMatchAsync(game);
            if (existing != null)
            {
                existing.StartAt = game.StartAt;
                existing.HomeTeam = game.HomeTeam;
                existing.AwayTeam = game.AwayTeam;
            }
            else
            {
                db.NbaGames.Add(new NbaGame
                {
                    Id = Guid.NewGuid().ToString(),
                    SourceGameId = game.SourceGameId,
                    StartAt = game.StartAt,
                    HomeTeam = game.HomeTeam,
                    AwayTeam = game.AwayTeam,
                });
            }
        }
        await db.SaveChangesAsync();
    }

    public async Task PruneBeforeAsync(DateTime cutoff)
    {
        var stale = await db.NbaGames.Where(g => g.StartAt < cutoff).ToListAsync();
        db.NbaGames.RemoveRange(stale);
        await db.SaveChangesAsync();
    }

    public async Task<IEnumerable<NbaGame>> GetRangeAsync(DateTime from, DateTime to) =>
        await db.NbaGames
            .Where(g => g.StartAt >= from && g.StartAt <= to)
            .OrderBy(g => g.StartAt)
            .ToListAsync();

    public async Task ClearAllAsync()
    {
        db.NbaGames.RemoveRange(await db.NbaGames.ToListAsync());
        await db.SaveChangesAsync();
    }

    private async Task<NbaGame?> FindMatchAsync(NbaScheduledGame game) =>
        game.SourceGameId != null
            ? await db.NbaGames.FirstOrDefaultAsync(g => g.SourceGameId == game.SourceGameId)
            : await db.NbaGames.FirstOrDefaultAsync(g =>
                g.SourceGameId == null
                && g.StartAt == game.StartAt
                && g.HomeTeam == game.HomeTeam
                && g.AwayTeam == game.AwayTeam);
}
