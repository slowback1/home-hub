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
        _job = new ActivityPickerJob(_crudFactory, _pickRepository);
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
}
