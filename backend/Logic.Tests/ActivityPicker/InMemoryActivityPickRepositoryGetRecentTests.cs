using System;
using System.Linq;
using Common.Models;
using InMemory;

namespace Logic.Tests.ActivityPicker;

public class InMemoryActivityPickRepositoryGetRecentTests
{
    [SetUp]
    public void SetUp() => InMemoryActivityPickRepository.ClearStaticState();

    private static ActivityPick Pick(string name, DateTime pickedAt) =>
        new() { ActivityName = name, PickedAt = pickedAt };

    [Test]
    public async Task GetRecentAsync_ReturnsCorrectCount()
    {
        var repo = new InMemoryActivityPickRepository();
        await repo.WriteAsync(Pick("A", DateTime.UtcNow.AddMinutes(-3)));
        await repo.WriteAsync(Pick("B", DateTime.UtcNow.AddMinutes(-2)));
        await repo.WriteAsync(Pick("C", DateTime.UtcNow.AddMinutes(-1)));

        var results = (await repo.GetRecentAsync(2)).ToList();

        Assert.That(results, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetRecentAsync_ReturnsInDescendingOrder()
    {
        var repo = new InMemoryActivityPickRepository();
        var older = DateTime.UtcNow.AddMinutes(-5);
        var newer = DateTime.UtcNow.AddMinutes(-1);
        await repo.WriteAsync(Pick("Older", older));
        await repo.WriteAsync(Pick("Newer", newer));

        var results = (await repo.GetRecentAsync(2)).ToList();

        Assert.That(results[0].ActivityName, Is.EqualTo("Newer"));
        Assert.That(results[1].ActivityName, Is.EqualTo("Older"));
    }

    [Test]
    public async Task GetRecentAsync_ReturnsAllWhenFewerThanCount()
    {
        var repo = new InMemoryActivityPickRepository();
        await repo.WriteAsync(Pick("A", DateTime.UtcNow));

        var results = (await repo.GetRecentAsync(10)).ToList();

        Assert.That(results, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task GetRecentAsync_ReturnsEmptyWhenCountIsZero()
    {
        var repo = new InMemoryActivityPickRepository();
        await repo.WriteAsync(Pick("A", DateTime.UtcNow));

        var results = (await repo.GetRecentAsync(0)).ToList();

        Assert.That(results, Is.Empty);
    }
}
