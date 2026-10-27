using Common.Interfaces;
using Common.Models;
using InMemory;
using Logic.ActivityPicker;

namespace Logic.Tests.ActivityPicker;

public class ActivityPickerJobTests
{
    private InMemoryCrudFactory _crudFactory = null!;
    private InMemoryActivityPickRepository _pickRepository = null!;
    private ActivityPickerJob _job = null!;

    [SetUp]
    public void SetUp()
    {
        InMemoryCrud<Activity>.ClearStaticState();
        InMemoryCrud<AppUser>.ClearStaticState();
        InMemoryActivityPickRepository.ClearStaticState();
        _crudFactory = new InMemoryCrudFactory();
        _pickRepository = new InMemoryActivityPickRepository();
        _job = new ActivityPickerJob(_crudFactory, _pickRepository, new RandomActivitySelector());
    }

    private async Task<AppUser> SeedUser(string name) =>
        await _crudFactory.GetCrud<AppUser>().CreateAsync(new AppUser { Name = name });

    private Task SeedActivity(string userId, string name, int weight) =>
        _crudFactory.GetCrud<Activity>().CreateAsync(new Activity { UserId = userId, Name = name, Weight = weight });

    [Test]
    public async Task ExecuteAsync_DoesNotWritePick_WhenNoUsersExist()
    {
        await _job.ExecuteAsync();

        var pick = await _pickRepository.GetCurrentAsync(string.Empty);
        Assert.That(pick, Is.Null);
    }

    [Test]
    public async Task ExecuteAsync_DoesNotWritePick_WhenUserHasNoActivities()
    {
        var user = await SeedUser("Solo");

        await _job.ExecuteAsync();

        var pick = await _pickRepository.GetCurrentAsync(user.Id);
        Assert.That(pick, Is.Null);
    }

    [Test]
    public async Task ExecuteAsync_WritesOnePick_WhenActivitiesExist()
    {
        var user = await SeedUser("Owner");
        await SeedActivity(user.Id, "Play Chess", 1);
        await SeedActivity(user.Id, "Read a Book", 1);

        await _job.ExecuteAsync();

        var pick = await _pickRepository.GetCurrentAsync(user.Id);
        Assert.That(pick, Is.Not.Null);
        Assert.That(pick!.UserId, Is.EqualTo(user.Id));
        Assert.That(new[] { "Play Chess", "Read a Book" }, Does.Contain(pick.ActivityName));
    }

    [Test]
    public async Task ExecuteAsync_WritesOnePickPerUser()
    {
        var userA = await SeedUser("A");
        var userB = await SeedUser("B");
        await SeedActivity(userA.Id, "A Activity", 1);
        await SeedActivity(userB.Id, "B Activity", 1);

        await _job.ExecuteAsync();

        var allPicks = (await _pickRepository.GetRecentAsync(userA.Id, 100)).ToList();
        var picksB = (await _pickRepository.GetRecentAsync(userB.Id, 100)).ToList();

        Assert.That(allPicks, Has.Count.EqualTo(1));
        Assert.That(allPicks[0].UserId, Is.EqualTo(userA.Id));
        Assert.That(allPicks[0].ActivityName, Is.EqualTo("A Activity"));
        Assert.That(picksB, Has.Count.EqualTo(1));
        Assert.That(picksB[0].UserId, Is.EqualTo(userB.Id));
        Assert.That(picksB[0].ActivityName, Is.EqualTo("B Activity"));
    }

    [Test]
    public async Task ExecuteAsync_PickedAt_IsInUtc()
    {
        var user = await SeedUser("Owner");
        await SeedActivity(user.Id, "Play Chess", 1);
        var before = DateTime.UtcNow;

        await _job.ExecuteAsync();

        var pick = await _pickRepository.GetCurrentAsync(user.Id);
        Assert.That(pick!.PickedAt.Kind, Is.EqualTo(DateTimeKind.Utc));
        Assert.That(pick.PickedAt, Is.GreaterThanOrEqualTo(before));
    }

    [Test]
    public async Task ExecuteAsync_WeightedSelection_FavoursHigherWeightActivity()
    {
        var user = await SeedUser("Owner");
        await SeedActivity(user.Id, "Rare", 1);
        await SeedActivity(user.Id, "Common", 9);

        var counts = new Dictionary<string, int> { ["Rare"] = 0, ["Common"] = 0 };
        const int iterations = 500;

        for (var i = 0; i < iterations; i++)
        {
            InMemoryActivityPickRepository.ClearStaticState();
            await _job.ExecuteAsync();
            var pick = await _pickRepository.GetCurrentAsync(user.Id);
            counts[pick!.ActivityName]++;
        }

        Assert.That(counts["Common"], Is.GreaterThan(counts["Rare"] * 2),
            "Higher-weight activity should be picked significantly more often");
    }

    [Test]
    public async Task ExecuteAsync_PassesAllActivitiesToSelector()
    {
        var user = await SeedUser("Owner");
        await SeedActivity(user.Id, "A", 1);
        await SeedActivity(user.Id, "B", 1);
        var capturing = new CapturingSelector(new Activity { Name = "A" });
        var job = new ActivityPickerJob(_crudFactory, _pickRepository, capturing);

        await job.ExecuteAsync();

        Assert.That(capturing.Calls.Single().Activities.Select(a => a.Name),
            Is.EquivalentTo(new[] { "A", "B" }));
    }

    [Test]
    public async Task ExecuteAsync_SelectionUsesOnlyUsersOwnActivities()
    {
        var userA = await SeedUser("A");
        var userB = await SeedUser("B");
        await SeedActivity(userA.Id, "A Activity", 1);
        await SeedActivity(userB.Id, "B Activity", 1);
        var capturing = new CapturingSelector(new Activity { Name = "A" });
        var job = new ActivityPickerJob(_crudFactory, _pickRepository, capturing);

        await job.ExecuteAsync();

        var callForB = capturing.Calls.Single(c => c.Activities.Any(a => a.Name == "B Activity"));
        Assert.That(callForB.Activities.Select(a => a.Name), Is.EquivalentTo(new[] { "B Activity" }));
    }

    [Test]
    public async Task ExecuteAsync_PassesRecentPicksToSelector_CountIsDoubleActivityCount()
    {
        var user = await SeedUser("Owner");
        await SeedActivity(user.Id, "A", 1);
        await SeedActivity(user.Id, "B", 1);

        for (var i = 0; i < 6; i++)
            await _pickRepository.WriteAsync(new ActivityPick { UserId = user.Id, ActivityName = "A", PickedAt = DateTime.UtcNow.AddMinutes(-i) });

        var capturing = new CapturingSelector(new Activity { Name = "A" });
        var job = new ActivityPickerJob(_crudFactory, _pickRepository, capturing);

        await job.ExecuteAsync();

        Assert.That(capturing.Calls.Single().RecentPicks.Count, Is.EqualTo(4));
    }

    [Test]
    public async Task ExecuteAsync_RecentPicksAreScopedToUser_OtherUsersPicksIgnored()
    {
        var userA = await SeedUser("A");
        var userB = await SeedUser("B");
        await SeedActivity(userA.Id, "A1", 1);
        await SeedActivity(userA.Id, "A2", 1);
        await SeedActivity(userB.Id, "B1", 1);

        for (var i = 0; i < 6; i++)
            await _pickRepository.WriteAsync(new ActivityPick { UserId = userA.Id, ActivityName = "A1", PickedAt = DateTime.UtcNow.AddMinutes(-i) });

        var capturing = new CapturingSelector(new Activity { Name = "B1" });
        var job = new ActivityPickerJob(_crudFactory, _pickRepository, capturing);

        await job.ExecuteAsync();

        var callForB = capturing.Calls.Single(c => c.Activities.Any(a => a.Name == "B1"));
        Assert.That(callForB.RecentPicks, Is.Empty);
    }

    private class CapturingSelector(Activity result) : IActivitySelector
    {
        public record Call(IList<Activity> Activities, IList<ActivityPick> RecentPicks);

        public List<Call> Calls { get; } = [];

        public Task<Activity> SelectAsync(IList<Activity> activities, IList<ActivityPick> recentPicks)
        {
            Calls.Add(new Call(activities, recentPicks));
            return Task.FromResult(result);
        }
    }
}
