using Common.Models;
using Logic.NbaSchedule;

namespace Logic.Tests.NbaSchedule;

[TestFixture]
public class MockNbaScheduleProviderTests
{
    private MockNbaScheduleProvider _provider = null!;

    [SetUp]
    public void SetUp() => _provider = new MockNbaScheduleProvider();

    [Test]
    public async Task GetScheduleAsync_ReturnsGames_WithinRequestedWindow()
    {
        var from = new DateTime(2026, 11, 20, 0, 0, 0, DateTimeKind.Utc);
        var to = from.AddDays(3);

        var games = (await _provider.GetScheduleAsync(from, to)).ToList();

        Assert.That(games, Is.Not.Empty);
        Assert.That(games, Is.All.Matches<NbaScheduledGame>(g => g.StartAt >= from && g.StartAt <= to));
    }

    [Test]
    public async Task GetScheduleAsync_IsDeterministic()
    {
        var from = new DateTime(2026, 11, 20, 0, 0, 0, DateTimeKind.Utc);
        var to = from.AddDays(3);

        var first = (await _provider.GetScheduleAsync(from, to)).ToList();
        var second = (await _provider.GetScheduleAsync(from, to)).ToList();

        Assert.That(second.Select(g => g.StartAt), Is.EqualTo(first.Select(g => g.StartAt)));
        Assert.That(second.Select(g => g.HomeTeam), Is.EqualTo(first.Select(g => g.HomeTeam)));
        Assert.That(second.Select(g => g.AwayTeam), Is.EqualTo(first.Select(g => g.AwayTeam)));
    }

    [Test]
    public async Task GetScheduleAsync_Games_HaveSourceGameIds()
    {
        var from = DateTime.UtcNow;
        var games = (await _provider.GetScheduleAsync(from, from.AddDays(3))).ToList();

        Assert.That(games, Is.All.Matches<NbaScheduledGame>(g => !string.IsNullOrWhiteSpace(g.SourceGameId)));
    }
}
