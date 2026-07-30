using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Tests;

[TestFixture]
public class WheelTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Test]
    public async Task Wheels_CanStoreAndRetrieveWheel()
    {
        using var ctx = CreateContext();
        var wheel = new Wheel
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Dinner",
            Items = "Pizza\nTacos\nSushi",
            CreatedAt = DateTime.UtcNow,
        };

        ctx.Wheels.Add(wheel);
        await ctx.SaveChangesAsync();

        var saved = await ctx.Wheels.FirstOrDefaultAsync(w => w.Id == wheel.Id);
        Assert.That(saved, Is.Not.Null);
        Assert.That(saved!.Name, Is.EqualTo("Dinner"));
        Assert.That(saved.Items, Is.EqualTo("Pizza\nTacos\nSushi"));
    }
}
