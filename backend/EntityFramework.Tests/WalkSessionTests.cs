using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Models;
using Microsoft.EntityFrameworkCore;
using EntityFramework;

namespace EntityFramework.Tests;

[TestFixture]
public class WalkSessionTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Test]
    public async Task WalkSessions_CanStoreAndRetrieveSession()
    {
        using var ctx = CreateContext();
        var session = new WalkSession
        {
            ClientId = Guid.NewGuid().ToString(),
            StartedAt = DateTime.UtcNow,
            DurationSeconds = 300,
            StepCount = 500,
            SyncedAt = DateTime.UtcNow,
        };

        ctx.WalkSessions.Add(session);
        await ctx.SaveChangesAsync();

        var saved = await ctx.WalkSessions.FirstOrDefaultAsync(s => s.ClientId == session.ClientId);
        Assert.That(saved, Is.Not.Null);
        Assert.That(saved!.StepCount, Is.EqualTo(500));
        Assert.That(saved.DurationSeconds, Is.EqualTo(300));
    }
}
