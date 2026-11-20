using Common.Models;
using InMemory;

namespace Logic.Tests.NbaSchedule;

[TestFixture]
public class InMemoryNbaScheduleRepositoryTests
{
    private InMemoryNbaScheduleRepository _repo = null!;

    [SetUp]
    public void SetUp()
    {
        InMemoryNbaScheduleRepository.ClearStaticState();
        _repo = new InMemoryNbaScheduleRepository();
    }

    protected static NbaScheduledGame Game(string? sourceId, DateTime startAt, string home, string away) =>
        new() { SourceGameId = sourceId, StartAt = startAt, HomeTeam = home, AwayTeam = away };

    [Test]
    public async Task UpsertAsync_InsertsNewGame()
    {
        var start = new DateTime(2026, 11, 21, 19, 30, 0, DateTimeKind.Utc);
        await _repo.UpsertAsync([Game("src-1", start, "Bulls", "Celtics")]);

        var games = (await _repo.GetRangeAsync(start.AddDays(-1), start.AddDays(1))).ToList();

        Assert.That(games, Has.Count.EqualTo(1));
        Assert.That(games[0].HomeTeam, Is.EqualTo("Bulls"));
        Assert.That(games[0].AwayTeam, Is.EqualTo("Celtics"));
        Assert.That(games[0].SourceGameId, Is.EqualTo("src-1"));
    }

    [Test]
    public async Task UpsertAsync_UpdatesInPlace_WhenSourceGameIdMatches()
    {
        var start = new DateTime(2026, 11, 21, 19, 30, 0, DateTimeKind.Utc);
        await _repo.UpsertAsync([Game("src-1", start, "Bulls", "Celtics")]);
        await _repo.UpsertAsync([Game("src-1", start.AddHours(1), "Bulls", "Celtics")]);

        var games = (await _repo.GetRangeAsync(start.AddDays(-1), start.AddDays(1))).ToList();

        Assert.That(games, Has.Count.EqualTo(1));
        Assert.That(games[0].StartAt, Is.EqualTo(start.AddHours(1)));
    }

    [Test]
    public async Task UpsertAsync_Inserts_WhenNaturalKeyMatchesButIncomingHasNoSourceGameId()
    {
        var start = new DateTime(2026, 11, 21, 19, 30, 0, DateTimeKind.Utc);
        await _repo.UpsertAsync([Game("src-1", start, "Bulls", "Celtics")]);
        await _repo.UpsertAsync([Game(null, start, "Bulls", "Celtics")]);

        var games = (await _repo.GetRangeAsync(start.AddDays(-1), start.AddDays(1))).ToList();

        Assert.That(games, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task UpsertAsync_KeepsDistinctGames_WhenNeitherHasSourceGameId()
    {
        var start = new DateTime(2026, 11, 21, 19, 30, 0, DateTimeKind.Utc);
        await _repo.UpsertAsync([
            Game(null, start, "Bulls", "Celtics"),
            Game(null, start, "Lakers", "Warriors"),
        ]);

        var games = (await _repo.GetRangeAsync(start.AddDays(-1), start.AddDays(1))).ToList();

        Assert.That(games, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task UpsertAsync_UpdatesInPlace_WhenNaturalKeyMatchesAndNeitherHasSourceGameId()
    {
        var start = new DateTime(2026, 11, 21, 19, 30, 0, DateTimeKind.Utc);
        await _repo.UpsertAsync([Game(null, start, "Bulls", "Celtics")]);
        await _repo.UpsertAsync([Game(null, start, "Bulls", "Celtics")]);

        var games = (await _repo.GetRangeAsync(start.AddDays(-1), start.AddDays(1))).ToList();

        Assert.That(games, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task PruneBeforeAsync_RemovesOnlyGamesBeforeCutoff()
    {
        var old = new DateTime(2026, 11, 18, 19, 30, 0, DateTimeKind.Utc);
        var cutoff = new DateTime(2026, 11, 20, 0, 0, 0, DateTimeKind.Utc);
        var recent = new DateTime(2026, 11, 21, 19, 30, 0, DateTimeKind.Utc);
        await _repo.UpsertAsync([
            Game("old-1", old, "Bulls", "Celtics"),
            Game("recent-1", recent, "Lakers", "Warriors"),
        ]);

        await _repo.PruneBeforeAsync(cutoff);

        var games = (await _repo.GetRangeAsync(cutoff.AddDays(-7), cutoff.AddDays(7))).ToList();
        Assert.That(games, Has.Count.EqualTo(1));
        Assert.That(games[0].SourceGameId, Is.EqualTo("recent-1"));
    }

    [Test]
    public async Task GetRangeAsync_IncludesGamesOnBothBounds()
    {
        var from = new DateTime(2026, 11, 20, 0, 0, 0, DateTimeKind.Utc);
        var to = new DateTime(2026, 11, 22, 0, 0, 0, DateTimeKind.Utc);
        await _repo.UpsertAsync([
            Game("edge-from", from, "A", "B"),
            Game("mid", from.AddDays(1), "C", "D"),
            Game("edge-to", to, "E", "F"),
            Game("outside", to.AddSeconds(1), "G", "H"),
        ]);

        var games = (await _repo.GetRangeAsync(from, to)).ToList();

        Assert.That(games, Has.Count.EqualTo(3));
    }
}
