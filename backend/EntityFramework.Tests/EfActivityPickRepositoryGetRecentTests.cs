using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Tests;

[TestFixture]
public class EfActivityPickRepositoryGetRecentTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static ActivityPick Pick(string name, DateTime pickedAt) =>
        new() { ActivityName = name, PickedAt = pickedAt };

    [Test]
    public async Task GetRecentAsync_ReturnsCorrectCount()
    {
        using var ctx = CreateContext();
        var repo = new EfActivityPickRepository(ctx);
        await repo.WriteAsync(Pick("A", DateTime.UtcNow.AddMinutes(-3)));
        await repo.WriteAsync(Pick("B", DateTime.UtcNow.AddMinutes(-2)));
        await repo.WriteAsync(Pick("C", DateTime.UtcNow.AddMinutes(-1)));

        var results = (await repo.GetRecentAsync(string.Empty, 2)).ToList();

        Assert.That(results, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetRecentAsync_ReturnsInDescendingOrder()
    {
        using var ctx = CreateContext();
        var repo = new EfActivityPickRepository(ctx);
        var older = DateTime.UtcNow.AddMinutes(-5);
        var newer = DateTime.UtcNow.AddMinutes(-1);
        await repo.WriteAsync(Pick("Older", older));
        await repo.WriteAsync(Pick("Newer", newer));

        var results = (await repo.GetRecentAsync(string.Empty, 2)).ToList();

        Assert.That(results[0].ActivityName, Is.EqualTo("Newer"));
        Assert.That(results[1].ActivityName, Is.EqualTo("Older"));
    }

    [Test]
    public async Task GetRecentAsync_ReturnsAllWhenFewerThanCount()
    {
        using var ctx = CreateContext();
        var repo = new EfActivityPickRepository(ctx);
        await repo.WriteAsync(Pick("A", DateTime.UtcNow));

        var results = (await repo.GetRecentAsync(string.Empty, 10)).ToList();

        Assert.That(results, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task GetRecentAsync_ReturnsEmptyWhenCountIsZero()
    {
        using var ctx = CreateContext();
        var repo = new EfActivityPickRepository(ctx);
        await repo.WriteAsync(Pick("A", DateTime.UtcNow));

        var results = (await repo.GetRecentAsync(string.Empty, 0)).ToList();

        Assert.That(results, Is.Empty);
    }

    [Test]
    public async Task GetRecentAsync_ReturnsOnlyRequestingUsersPicks()
    {
        using var ctx = CreateContext();
        var repo = new EfActivityPickRepository(ctx);
        await repo.WriteAsync(new ActivityPick { UserId = "user-a", ActivityName = "Mine", PickedAt = DateTime.UtcNow });
        await repo.WriteAsync(new ActivityPick { UserId = "user-b", ActivityName = "Theirs", PickedAt = DateTime.UtcNow });

        var results = (await repo.GetRecentAsync("user-a", 10)).ToList();

        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results[0].ActivityName, Is.EqualTo("Mine"));
    }

    [Test]
    public async Task GetCurrentAsync_ReturnsOnlyRequestingUsersLatestPick()
    {
        using var ctx = CreateContext();
        var repo = new EfActivityPickRepository(ctx);
        await repo.WriteAsync(new ActivityPick { UserId = "user-a", ActivityName = "Mine", PickedAt = DateTime.UtcNow.AddMinutes(-5) });
        await repo.WriteAsync(new ActivityPick { UserId = "user-b", ActivityName = "Theirs", PickedAt = DateTime.UtcNow });

        var current = await repo.GetCurrentAsync("user-a");

        Assert.That(current, Is.Not.Null);
        Assert.That(current!.ActivityName, Is.EqualTo("Mine"));
    }

    [Test]
    public async Task GetRangeAsync_ReturnsOnlyRequestingUsersPicksInRange()
    {
        using var ctx = CreateContext();
        var repo = new EfActivityPickRepository(ctx);
        var now = DateTime.UtcNow;
        await repo.WriteAsync(new ActivityPick { UserId = "user-a", ActivityName = "Mine", PickedAt = now.AddDays(-1) });
        await repo.WriteAsync(new ActivityPick { UserId = "user-b", ActivityName = "Theirs", PickedAt = now.AddDays(-1) });
        await repo.WriteAsync(new ActivityPick { UserId = "user-a", ActivityName = "Old", PickedAt = now.AddDays(-30) });

        var results = (await repo.GetRangeAsync("user-a", now.AddDays(-7), now)).ToList();

        Assert.That(results, Has.Count.EqualTo(1));
        Assert.That(results[0].ActivityName, Is.EqualTo("Mine"));
    }
}
