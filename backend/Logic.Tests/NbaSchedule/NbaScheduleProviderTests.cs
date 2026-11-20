using System.Net;
using System.Text;
using Logic.NbaSchedule;

namespace Logic.Tests.NbaSchedule;

public class NbaScheduleProviderTests
{
    private static (NbaScheduleProvider Provider, FakeHandler Handler) MakeClient(HttpStatusCode status, string body)
    {
        var handler = new FakeHandler(status, body);
        return (new NbaScheduleProvider(new HttpClient(handler)), handler);
    }

    private class FakeHandler(HttpStatusCode status, string body) : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(new HttpResponseMessage(status)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            });
        }
    }

    [Test]
    public async Task GetScheduleAsync_RequestsGameDatesCoveringWindow()
    {
        var (provider, handler) = MakeClient(HttpStatusCode.OK, """{"scoreboard":{"games":[]}}""");
        var from = new DateTime(2026, 11, 20, 0, 0, 0, DateTimeKind.Utc);
        var to = new DateTime(2026, 11, 22, 0, 0, 0, DateTimeKind.Utc);

        await provider.GetScheduleAsync(from, to);

        var url = handler.LastRequest!.RequestUri!.ToString();
        Assert.That(url, Does.Contain("gameDates=2026-11-20,2026-11-21,2026-11-22"));
    }

    private const string ScoreboxPayload = """
        {
          "scoreboard": {
            "games": [
              {
                "gameId": "2026112001",
                "gameDate": "2026-11-20",
                "startDate": "2026-11-20T7:00:00",
                "seasonType": 2,
                "homeTeam": { "teamName": "Chicago Bulls" },
                "awayTeam": { "teamName": "Boston Celtics" }
              }
            ]
          }
        }
        """;

    [Test]
    public void GetScheduleAsync_ThrowsOnNonSuccessStatusCode()
    {
        var (provider, _) = MakeClient(HttpStatusCode.InternalServerError, "boom");
        var from = new DateTime(2026, 11, 20, 0, 0, 0, DateTimeKind.Utc);

        Assert.ThrowsAsync<HttpRequestException>(
            async () => await provider.GetScheduleAsync(from, from.AddDays(1)));
    }

    [Test]
    public async Task GetScheduleAsync_ParsesGamesWithUtcStartTimes()
    {
        var (provider, _) = MakeClient(HttpStatusCode.OK, ScoreboxPayload);
        var from = new DateTime(2026, 11, 20, 0, 0, 0, DateTimeKind.Utc);

        var games = (await provider.GetScheduleAsync(from, from.AddDays(2))).ToList();

        Assert.That(games, Has.Count.EqualTo(1));
        Assert.That(games[0].SourceGameId, Is.EqualTo("2026112001"));
        Assert.That(games[0].HomeTeam, Is.EqualTo("Chicago Bulls"));
        Assert.That(games[0].AwayTeam, Is.EqualTo("Boston Celtics"));
        Assert.That(games[0].StartAt, Is.EqualTo(new DateTime(2026, 11, 20, 12, 0, 0, DateTimeKind.Utc)));
    }

    [Test]
    public async Task GetScheduleAsync_KeepsOnlyRegularSeasonAndPlayoffsGames()
    {
        var payload = """
            {
              "scoreboard": {
                "games": [
                  { "gameId": "g1", "startDate": "2026-11-20T7:00:00", "seasonType": 1,
                    "homeTeam": { "teamName": "Team A" }, "awayTeam": { "teamName": "Team B" } },
                  { "gameId": "g2", "startDate": "2026-11-20T8:00:00", "seasonType": 2,
                    "homeTeam": { "teamName": "Team C" }, "awayTeam": { "teamName": "Team D" } },
                  { "gameId": "g3", "startDate": "2026-11-20T9:00:00", "seasonType": 3,
                    "homeTeam": { "teamName": "Team E" }, "awayTeam": { "teamName": "Team F" } }
                ]
              }
            }
            """;
        var (provider, _) = MakeClient(HttpStatusCode.OK, payload);
        var from = new DateTime(2026, 11, 20, 0, 0, 0, DateTimeKind.Utc);

        var games = (await provider.GetScheduleAsync(from, from)).ToList();

        Assert.That(games, Has.Count.EqualTo(2));
        Assert.That(games.Select(g => g.SourceGameId), Is.EquivalentTo(new[] { "g2", "g3" }));
    }
}
