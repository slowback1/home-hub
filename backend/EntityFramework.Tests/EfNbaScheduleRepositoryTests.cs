using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Tests;

[TestFixture]
public class EfNbaScheduleRepositoryTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static NbaScheduledGame Game(string? sourceId, DateTime startAt, string home, string away) =>
        new() { SourceGameId = sourceId, StartAt = startAt, HomeTeam = home, AwayTeam = away };

    [Test]
    public async Task UpsertAsync_InsertsNewGame()
    {
        using var ctx = CreateContext();
        var repo = new EfNbaScheduleRepository(ctx);
        var start = new DateTime(2026, 11, 21, 19, 30, 0, DateTimeKind.Utc);
        await repo.UpsertAsync([Game("src-1", start, "Bulls", "Celtics")]);

        var games = (await repo.GetRangeAsync(start.AddDays(-1), start.AddDays(1))).ToList();

        Assert.That(games, Has.Count.EqualTo(1));
        Assert.That(games[0].HomeTeam, Is.EqualTo("Bulls"));
        Assert.That(games[0].AwayTeam, Is.EqualTo("Celtics"));
        Assert.That(games[0].SourceGameId, Is.EqualTo("src-1"));
    }

    [Test]
    public async Task UpsertAsync_UpdatesInPlace_WhenSourceGameIdMatches()
    {
        using var ctx = CreateContext();
        var repo = new EfNbaScheduleRepository(ctx);
        var start = new DateTime(2026, 11, 21, 19, 30, 0, DateTimeKind.Utc);
        await repo.UpsertAsync([Game("src-1", start, "Bulls", "Celtics")]);
        await repo.UpsertAsync([Game("src-1", start.AddHours(1), "Bulls", "Celtics")]);

        var games = (await repo.GetRangeAsync(start.AddDays(-1), start.AddDays(1))).ToList();

        Assert.That(games, Has.Count.EqualTo(1));
        Assert.That(games[0].StartAt, Is.EqualTo(start.AddHours(1)));
    }

    [Test]
    public async Task UpsertAsync_Inserts_WhenNaturalKeyMatchesButIncomingHasNoSourceGameId()
    {
        using var ctx = CreateContext();
        var repo = new EfNbaScheduleRepository(ctx);
        var start = new DateTime(2026, 11, 21, 19, 30, 0, DateTimeKind.Utc);
        await repo.UpsertAsync([Game("src-1", start, "Bulls", "Celtics")]);
        await repo.UpsertAsync([Game(null, start, "Bulls", "Celtics")]);

        var games = (await repo.GetRangeAsync(start.AddDays(-1), start.AddDays(1))).ToList();

        Assert.That(games, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task UpsertAsync_KeepsDistinctGames_WhenNeitherHasSourceGameId()
    {
        using var ctx = CreateContext();
        var repo = new EfNbaScheduleRepository(ctx);
        var start = new DateTime(2026, 11, 21, 19, 30, 0, DateTimeKind.Utc);
        await repo.UpsertAsync([
            Game(null, start, "Bulls", "Celtics"),
            Game(null, start, "Lakers", "Warriors"),
        ]);

        var games = (await repo.GetRangeAsync(start.AddDays(-1), start.AddDays(1))).ToList();

        Assert.That(games, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task UpsertAsync_UpdatesInPlace_WhenNaturalKeyMatchesAndNeitherHasSourceGameId()
    {
        using var ctx = CreateContext();
        var repo = new EfNbaScheduleRepository(ctx);
        var start = new DateTime(2026, 11, 21, 19, 30, 0, DateTimeKind.Utc);
        await repo.UpsertAsync([Game(null, start, "Bulls", "Celtics")]);
        await repo.UpsertAsync([Game(null, start, "Bulls", "Celtics")]);

        var games = (await repo.GetRangeAsync(start.AddDays(-1), start.AddDays(1))).ToList();

        Assert.That(games, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task PruneBeforeAsync_RemovesOnlyGamesBeforeCutoff()
    {
        using var ctx = CreateContext();
        var repo = new EfNbaScheduleRepository(ctx);
        var old = new DateTime(2026, 11, 18, 19, 30, 0, DateTimeKind.Utc);
        var cutoff = new DateTime(2026, 11, 20, 0, 0, 0, DateTimeKind.Utc);
        var recent = new DateTime(2026, 11, 21, 19, 30, 0, DateTimeKind.Utc);
        await repo.UpsertAsync([
            Game("old-1", old, "Bulls", "Celtics"),
            Game("recent-1", recent, "Lakers", "Warriors"),
        ]);

        await repo.PruneBeforeAsync(cutoff);

        var games = (await repo.GetRangeAsync(cutoff.AddDays(-7), cutoff.AddDays(7))).ToList();
        Assert.That(games, Has.Count.EqualTo(1));
        Assert.That(games[0].SourceGameId, Is.EqualTo("recent-1"));
    }

    [Test]
    public async Task GetRangeAsync_IncludesGamesOnBothBounds()
    {
        using var ctx = CreateContext();
        var repo = new EfNbaScheduleRepository(ctx);
        var from = new DateTime(2026, 11, 20, 0, 0, 0, DateTimeKind.Utc);
        var to = new DateTime(2026, 11, 22, 0, 0, 0, DateTimeKind.Utc);
        await repo.UpsertAsync([
            Game("edge-from", from, "A", "B"),
            Game("mid", from.AddDays(1), "C", "D"),
            Game("edge-to", to, "E", "F"),
            Game("outside", to.AddSeconds(1), "G", "H"),
        ]);

        var games = (await repo.GetRangeAsync(from, to)).ToList();

        Assert.That(games, Has.Count.EqualTo(3));
    }
}
