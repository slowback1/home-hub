using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Tests;

[TestFixture]
public class SavedItemTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Test]
    public async Task SavedItems_CanStoreAndRetrieveItem()
    {
        using var ctx = CreateContext();
        var item = new SavedItem
        {
            Id = Guid.NewGuid().ToString(),
            UserId = "user-1",
            Name = "5lb sack of potatoes",
            CreatedAt = DateTime.UtcNow,
        };

        ctx.SavedItems.Add(item);
        await ctx.SaveChangesAsync();

        var saved = await ctx.SavedItems.FirstOrDefaultAsync(s => s.Id == item.Id);
        Assert.That(saved, Is.Not.Null);
        Assert.That(saved!.Name, Is.EqualTo("5lb sack of potatoes"));
        Assert.That(saved.UserId, Is.EqualTo("user-1"));
    }
}
