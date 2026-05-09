using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        InMemoryActivityPickRepository.ClearStaticState();
        _crudFactory = new InMemoryCrudFactory();
        _pickRepository = new InMemoryActivityPickRepository();
        _job = new ActivityPickerJob(_crudFactory, _pickRepository, new RandomActivitySelector());
    }

    [Test]
    public async Task ExecuteAsync_DoesNotWritePick_WhenActivityListIsEmpty()
    {
        await _job.ExecuteAsync();

        var pick = await _pickRepository.GetCurrentAsync();
        Assert.That(pick, Is.Null);
    }

    [Test]
    public async Task ExecuteAsync_WritesOnePick_WhenActivitiesExist()
    {
        var activityCrud = _crudFactory.GetCrud<Activity>();
        await activityCrud.CreateAsync(new Activity { Name = "Play Chess", Weight = 1 });
        await activityCrud.CreateAsync(new Activity { Name = "Read a Book", Weight = 1 });

        await _job.ExecuteAsync();

        var pick = await _pickRepository.GetCurrentAsync();
        Assert.That(pick, Is.Not.Null);
        Assert.That(new[] { "Play Chess", "Read a Book" }, Does.Contain(pick!.ActivityName));
    }

    [Test]
    public async Task ExecuteAsync_PickedAt_IsInUtc()
    {
        var activityCrud = _crudFactory.GetCrud<Activity>();
        await activityCrud.CreateAsync(new Activity { Name = "Play Chess", Weight = 1 });
        var before = DateTime.UtcNow;

        await _job.ExecuteAsync();

        var pick = await _pickRepository.GetCurrentAsync();
        Assert.That(pick!.PickedAt.Kind, Is.EqualTo(DateTimeKind.Utc));
        Assert.That(pick.PickedAt, Is.GreaterThanOrEqualTo(before));
    }

    [Test]
    public async Task ExecuteAsync_WeightedSelection_FavoursHigherWeightActivity()
    {
        var activityCrud = _crudFactory.GetCrud<Activity>();
        await activityCrud.CreateAsync(new Activity { Name = "Rare", Weight = 1 });
        await activityCrud.CreateAsync(new Activity { Name = "Common", Weight = 9 });

        var counts = new Dictionary<string, int> { ["Rare"] = 0, ["Common"] = 0 };
        const int iterations = 500;

        for (var i = 0; i < iterations; i++)
        {
            InMemoryActivityPickRepository.ClearStaticState();
            await _job.ExecuteAsync();
            var pick = await _pickRepository.GetCurrentAsync();
            counts[pick!.ActivityName]++;
        }

        Assert.That(counts["Common"], Is.GreaterThan(counts["Rare"] * 2),
            "Higher-weight activity should be picked significantly more often");
    }

    [Test]
    public async Task ExecuteAsync_PassesAllActivitiesToSelector()
    {
        var activityCrud = _crudFactory.GetCrud<Activity>();
        await activityCrud.CreateAsync(new Activity { Name = "A", Weight = 1 });
        await activityCrud.CreateAsync(new Activity { Name = "B", Weight = 1 });
        var capturing = new CapturingSelector(new Activity { Name = "A" });
        var job = new ActivityPickerJob(_crudFactory, _pickRepository, capturing);

        await job.ExecuteAsync();

        Assert.That(capturing.CapturedActivities!.Select(a => a.Name),
            Is.EquivalentTo(new[] { "A", "B" }));
    }

    [Test]
    public async Task ExecuteAsync_PassesRecentPicksToSelector_CountIsDoubleActivityCount()
    {
        var activityCrud = _crudFactory.GetCrud<Activity>();
        await activityCrud.CreateAsync(new Activity { Name = "A", Weight = 1 });
        await activityCrud.CreateAsync(new Activity { Name = "B", Weight = 1 });

        for (var i = 0; i < 6; i++)
            await _pickRepository.WriteAsync(new ActivityPick { ActivityName = "A", PickedAt = DateTime.UtcNow.AddMinutes(-i) });

        var capturing = new CapturingSelector(new Activity { Name = "A" });
        var job = new ActivityPickerJob(_crudFactory, _pickRepository, capturing);

        await job.ExecuteAsync();

        Assert.That(capturing.CapturedRecentPicks!.Count, Is.EqualTo(4));
    }

    private class CapturingSelector(Activity result) : IActivitySelector
    {
        public IList<Activity>? CapturedActivities { get; private set; }
        public IList<ActivityPick>? CapturedRecentPicks { get; private set; }

        public Task<Activity> SelectAsync(IList<Activity> activities, IList<ActivityPick> recentPicks)
        {
            CapturedActivities = activities;
            CapturedRecentPicks = recentPicks;
            return Task.FromResult(result);
        }
    }
}
