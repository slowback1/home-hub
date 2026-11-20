using System.Net;
using System.Net.Http.Json;
using Common.Models;
using InMemory;

namespace WebAPI.Integration.Tests.Controllers;

public class NbaScheduleControllerTests : ControllerTestBase
{
    [SetUp]
    public void ClearNbaGames() => InMemoryNbaScheduleRepository.ClearStaticState();

    private static async Task SeedGame(string sourceId, DateTime startAt, string home, string away) =>
        await new InMemoryNbaScheduleRepository().UpsertAsync([
            new NbaScheduledGame { SourceGameId = sourceId, StartAt = startAt, HomeTeam = home, AwayTeam = away }
        ]);

    [Test]
    public async Task GetSchedule_ReturnsSeededGames_OrderedByStartAtAscending()
    {
        await SeedGame("g-late", DateTime.UtcNow.AddDays(2), "Bulls", "Heat");
        await SeedGame("g-early", DateTime.UtcNow.AddDays(1), "Lakers", "Celtics");

        var games = await GetAsync<List<NbaGame>>("/api/nba/schedule", includeToken: false);

        Assert.That(games, Is.Not.Null);
        Assert.That(games!.Select(g => g.SourceGameId), Is.EqualTo(new[] { "g-early", "g-late" }));
    }

    [Test]
    public async Task GetSchedule_FromToParams_RespectFilterRange()
    {
        await SeedGame("g-in", DateTime.UtcNow.AddDays(1), "Bucks", "Knicks");
        await SeedGame("g-out", DateTime.UtcNow.AddDays(10), "Nets", "Sixers");

        var from = Uri.EscapeDataString(DateTime.UtcNow.ToString("O"));
        var to = Uri.EscapeDataString(DateTime.UtcNow.AddDays(3).ToString("O"));

        var games = await GetAsync<List<NbaGame>>($"/api/nba/schedule?from={from}&to={to}", includeToken: false);

        Assert.That(games, Is.Not.Null);
        Assert.That(games!.Select(g => g.SourceGameId), Is.EquivalentTo(new[] { "g-in" }));
    }

    [Test]
    public async Task GetSchedule_EmptyCache_Returns200WithEmptyList()
    {
        var response = await GetRawAsync("/api/nba/schedule", includeToken: false);
        var games = await response.Content.ReadFromJsonAsync<List<NbaGame>>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(games, Is.Empty);
    }
}
