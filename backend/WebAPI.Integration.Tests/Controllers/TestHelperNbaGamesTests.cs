using System.Net;
using System.Text;
using System.Text.Json;
using Common.Models;
using InMemory;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace WebAPI.Integration.Tests.Controllers;

public class TestHelperNbaGamesTests : ControllerTestBase
{
    [SetUp]
    public void ClearNbaGames() => InMemoryNbaScheduleRepository.ClearStaticState();

    private static async Task SeedGame(string sourceId, DateTime startAt) =>
        await new InMemoryNbaScheduleRepository().UpsertAsync([
            new NbaScheduledGame { SourceGameId = sourceId, StartAt = startAt, HomeTeam = "Bulls", AwayTeam = "Heat" }
        ]);

    [Test]
    public async Task ClearNbaGames_Returns204_AndClearsSeededGames()
    {
        await SeedGame("g-1", DateTime.UtcNow.AddDays(1));

        var response = await DeleteRawAsync("/api/test/nba-games", includeToken: false);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var games = await GetAsync<List<NbaGame>>("/api/nba/schedule", includeToken: false);
        Assert.That(games, Is.Empty);
    }

    [Test]
    public async Task ClearNbaGames_Returns404_InDisallowedEnvironment()
    {
        await SeedGame("g-keep", DateTime.UtcNow.AddDays(1));

        using var factory = new DisallowedEnvironmentWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "/api/test/nba-games"));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var games = await new InMemoryNbaScheduleRepository()
            .GetRangeAsync(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(7));
        Assert.That(games.Select(g => g.SourceGameId), Is.EquivalentTo(new[] { "g-keep" }));
    }

    [Test]
    public async Task SeedNbaGame_Returns200_AndGameAppearsInSchedule()
    {
        var startAt = DateTime.UtcNow.AddDays(1);

        var response = await PostRawAsync("/api/test/nba-games",
            new { StartAt = startAt, HomeTeam = "Lakers", AwayTeam = "Celtics", SourceGameId = "seed-1" },
            includeToken: false);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var games = await GetAsync<List<NbaGame>>("/api/nba/schedule", includeToken: false);
        Assert.That(games, Is.Not.Null);
        Assert.That(games.Select(g => g.SourceGameId), Is.EquivalentTo(new[] { "seed-1" }));
        var seeded = games.Single();
        Assert.That(seeded.HomeTeam, Is.EqualTo("Lakers"));
        Assert.That(seeded.AwayTeam, Is.EqualTo("Celtics"));
        Assert.That(seeded.StartAt, Is.EqualTo(startAt).Within(TimeSpan.FromSeconds(1)));
    }

    [Test]
    public async Task SeedNbaGame_WithoutSourceGameId_StillSeeds()
    {
        var response = await PostRawAsync("/api/test/nba-games",
            new { StartAt = DateTime.UtcNow.AddDays(1), HomeTeam = "Bulls", AwayTeam = "Heat" },
            includeToken: false);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var games = await GetAsync<List<NbaGame>>("/api/nba/schedule", includeToken: false);
        Assert.That(games, Is.Not.Null);
        Assert.That(games.Select(g => g.HomeTeam), Is.EquivalentTo(new[] { "Bulls" }));
    }

    [Test]
    public async Task SeedNbaGame_Returns404_InDisallowedEnvironment()
    {
        using var factory = new DisallowedEnvironmentWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsync("/api/test/nba-games",
            new StringContent(JsonSerializer.Serialize(new { StartAt = DateTime.UtcNow.AddDays(1), HomeTeam = "Lakers", AwayTeam = "Celtics" }), Encoding.UTF8, "application/json"));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var games = await new InMemoryNbaScheduleRepository()
            .GetRangeAsync(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(7));
        Assert.That(games, Is.Empty);
    }

    private class DisallowedEnvironmentWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Staging");
            builder.UseSetting("CrudFactory:Implementation", "InMemory");
        }
    }
}
