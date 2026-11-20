using Common.Interfaces;
using Common.Models;
using Logic.NbaSchedule;

namespace Logic.Tests.NbaSchedule;

public class NbaScheduleFetchJobTests
{
    private FakeNbaScheduleProvider _provider = null!;
    private FakeNbaScheduleRepository _repository = null!;
    private NbaScheduleFetchJob _job = null!;

    [SetUp]
    public void SetUp()
    {
        _provider = new FakeNbaScheduleProvider();
        _repository = new FakeNbaScheduleRepository();
        _job = new NbaScheduleFetchJob(_provider, _repository);
    }

    private static NbaScheduledGame Game(string sourceId, DateTime startAt, string home, string away) =>
        new() { SourceGameId = sourceId, StartAt = startAt, HomeTeam = home, AwayTeam = away };

    [Test]
    public async Task ExecuteAsync_UpsertsFetchedGames()
    {
        var games = new[]
        {
            Game("g1", DateTime.UtcNow.AddDays(1), "Lakers", "Celtics"),
            Game("g2", DateTime.UtcNow.AddDays(2), "Bulls", "Heat"),
        };
        _provider.GamesToReturn = games;

        await _job.ExecuteAsync();

        Assert.That(_repository.UpsertedGames, Is.EquivalentTo(games));
    }

    [Test]
    public async Task ExecuteAsync_FetchWindow_SpansThreeDaysFromTodayUtc()
    {
        await _job.ExecuteAsync();

        Assert.That(_provider.From, Is.EqualTo(DateTime.UtcNow.Date));
        Assert.That(_provider.To, Is.EqualTo(DateTime.UtcNow.Date.AddDays(3)));
    }

    [Test]
    public async Task ExecuteAsync_PrunesStaleGames_WithCutoffThreeDaysAgo()
    {
        var before = DateTime.UtcNow;

        await _job.ExecuteAsync();

        Assert.That(_repository.PruneCutoffs, Has.Count.EqualTo(1));
        Assert.That(_repository.PruneCutoffs[0], Is.GreaterThanOrEqualTo(before.AddDays(-3)));
        Assert.That(_repository.PruneCutoffs[0], Is.LessThanOrEqualTo(DateTime.UtcNow.AddDays(-3)));
    }

    [Test]
    public async Task ExecuteAsync_ProviderFails_PerformsNoUpsertAndNoPrune()
    {
        _provider.ThrowOnCall = true;

        await _job.ExecuteAsync();

        Assert.That(_repository.UpsertedGames, Is.Empty);
        Assert.That(_repository.PruneCutoffs, Is.Empty);
    }

    private class FakeNbaScheduleProvider : INbaScheduleProvider
    {
        public IEnumerable<NbaScheduledGame> GamesToReturn { get; set; } = [];
        public DateTime? From { get; private set; }
        public DateTime? To { get; private set; }
        public bool ThrowOnCall { get; set; }

        public Task<IEnumerable<NbaScheduledGame>> GetScheduleAsync(DateTime from, DateTime to)
        {
            From = from;
            To = to;
            if (ThrowOnCall)
                throw new HttpRequestException("NBA API unavailable");
            return Task.FromResult(GamesToReturn);
        }
    }

    private class FakeNbaScheduleRepository : INbaScheduleRepository
    {
        public List<NbaScheduledGame> UpsertedGames { get; } = [];
        public List<DateTime> PruneCutoffs { get; } = [];

        public Task UpsertAsync(IEnumerable<NbaScheduledGame> games)
        {
            UpsertedGames.AddRange(games);
            return Task.CompletedTask;
        }

        public Task PruneBeforeAsync(DateTime cutoff)
        {
            PruneCutoffs.Add(cutoff);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<NbaGame>> GetRangeAsync(DateTime from, DateTime to) =>
            Task.FromResult<IEnumerable<NbaGame>>([]);

        public Task ClearAllAsync() => Task.CompletedTask;
    }
}
