using System.Net;
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

    private class DisallowedEnvironmentWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Staging");
            builder.UseSetting("CrudFactory:Implementation", "InMemory");
        }
    }
}
