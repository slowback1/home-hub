using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Models;
using Logic.ActivityPicker;

namespace Logic.Tests.ActivityPicker;

public class RandomActivitySelectorTests
{
    private static Activity Act(string name, int weight = 1) =>
        new() { Id = name, Name = name, Weight = weight };

    [Test]
    public async Task SelectAsync_ReturnsSingleActivity_WhenOnlyOneExists()
    {
        var selector = new RandomActivitySelector();
        var activities = new List<Activity> { Act("swim") };

        var result = await selector.SelectAsync(activities, []);

        Assert.That(result.Name, Is.EqualTo("swim"));
    }

    [Test]
    public async Task SelectAsync_ReturnsActivityFromList()
    {
        var selector = new RandomActivitySelector();
        var activities = new List<Activity> { Act("swim"), Act("run"), Act("read") };

        var result = await selector.SelectAsync(activities, []);

        Assert.That(activities.Select(a => a.Name), Contains.Item(result.Name));
    }

    [Test]
    public async Task SelectAsync_EventuallyPicksAllActivities_GivenEqualWeights()
    {
        var selector = new RandomActivitySelector();
        var activities = new List<Activity> { Act("A"), Act("B"), Act("C") };
        var seen = new HashSet<string>();

        for (var i = 0; i < 300; i++)
        {
            var pick = await selector.SelectAsync(activities, []);
            seen.Add(pick.Name);
            if (seen.Count == 3) break;
        }

        Assert.That(seen, Has.Count.EqualTo(3));
    }

    [Test]
    public async Task SelectAsync_IgnoresRecentPicks()
    {
        var selector = new RandomActivitySelector();
        var activities = new List<Activity> { Act("swim") };
        var recent = new List<ActivityPick> { new() { ActivityName = "swim" } };

        var result = await selector.SelectAsync(activities, recent);

        Assert.That(result.Name, Is.EqualTo("swim"));
    }

    [Test]
    public async Task SelectAsync_RespectsWeight_HigherWeightPickedMoreOften()
    {
        var selector = new RandomActivitySelector();
        var activities = new List<Activity> { Act("rare", weight: 1), Act("common", weight: 99) };
        var picks = new List<string>();

        for (var i = 0; i < 1000; i++)
        {
            var pick = await selector.SelectAsync(activities, []);
            picks.Add(pick.Name);
        }

        var commonCount = picks.Count(p => p == "common");
        Assert.That(commonCount, Is.GreaterThan(900));
    }
}
