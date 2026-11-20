using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Tests;

[TestFixture]
public class NbaGameTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Test]
    public async Task NbaGames_CanStoreAndRetrieveGame()
    {
        using var ctx = CreateContext();
        var game = new NbaGame
        {
            SourceGameId = "src-123",
            StartAt = new DateTime(2026, 11, 21, 19, 30, 0, DateTimeKind.Utc),
            HomeTeam = "Chicago Bulls",
            AwayTeam = "Boston Celtics",
        };

        ctx.NbaGames.Add(game);
        await ctx.SaveChangesAsync();

        var saved = await ctx.NbaGames.FirstOrDefaultAsync(g => g.SourceGameId == "src-123");
        Assert.That(saved, Is.Not.Null);
        Assert.That(saved!.HomeTeam, Is.EqualTo("Chicago Bulls"));
        Assert.That(saved.AwayTeam, Is.EqualTo("Boston Celtics"));
        Assert.That(saved.StartAt, Is.EqualTo(new DateTime(2026, 11, 21, 19, 30, 0, DateTimeKind.Utc)));
    }

    [Test]
    public async Task NbaGames_SourceGameId_IsOptional()
    {
        using var ctx = CreateContext();
        var game = new NbaGame
        {
            StartAt = DateTime.UtcNow,
            HomeTeam = "LA Lakers",
            AwayTeam = "Golden State Warriors",
        };

        ctx.NbaGames.Add(game);
        await ctx.SaveChangesAsync();

        var saved = await ctx.NbaGames.FirstOrDefaultAsync(g => g.HomeTeam == "LA Lakers");
        Assert.That(saved, Is.Not.Null);
        Assert.That(saved!.SourceGameId, Is.Null);
    }
}
