using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Tests;

[TestFixture]
public class PremadeListTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Test]
    public async Task PremadeLists_CanStoreAndRetrieveList()
    {
        using var ctx = CreateContext();
        var list = new PremadeList
        {
            Id = Guid.NewGuid().ToString(),
            UserId = "user-1",
            Name = "Weekly groceries",
            CreatedAt = DateTime.UtcNow,
        };

        ctx.PremadeLists.Add(list);
        await ctx.SaveChangesAsync();

        var saved = await ctx.PremadeLists.FirstOrDefaultAsync(l => l.Id == list.Id);
        Assert.That(saved, Is.Not.Null);
        Assert.That(saved!.Name, Is.EqualTo("Weekly groceries"));
        Assert.That(saved.UserId, Is.EqualTo("user-1"));
    }

    [Test]
    public async Task PremadeListItems_CanStoreAndRetrieveMembership()
    {
        using var ctx = CreateContext();
        var list = new PremadeList
        {
            Id = Guid.NewGuid().ToString(),
            UserId = "user-1",
            Name = "Weekly groceries",
            CreatedAt = DateTime.UtcNow,
        };
        var membership = new PremadeListItem
        {
            Id = Guid.NewGuid().ToString(),
            PremadeListId = list.Id,
            SavedItemId = "saved-1",
            CreatedAt = DateTime.UtcNow,
        };

        ctx.PremadeLists.Add(list);
        ctx.PremadeListItems.Add(membership);
        await ctx.SaveChangesAsync();

        var saved = await ctx.PremadeListItems.FirstOrDefaultAsync(i => i.Id == membership.Id);
        Assert.That(saved, Is.Not.Null);
        Assert.That(saved!.PremadeListId, Is.EqualTo(list.Id));
        Assert.That(saved.SavedItemId, Is.EqualTo("saved-1"));
    }

    [Test]
    public async Task PremadeListItems_CascadeDeleteWithList()
    {
        using var ctx = CreateContext();
        var list = new PremadeList
        {
            Id = Guid.NewGuid().ToString(),
            UserId = "user-1",
            Name = "Weekly groceries",
            CreatedAt = DateTime.UtcNow,
        };
        var membership = new PremadeListItem
        {
            Id = Guid.NewGuid().ToString(),
            PremadeListId = list.Id,
            SavedItemId = "saved-1",
            CreatedAt = DateTime.UtcNow,
        };

        ctx.PremadeLists.Add(list);
        ctx.PremadeListItems.Add(membership);
        await ctx.SaveChangesAsync();

        ctx.PremadeLists.Remove(list);
        await ctx.SaveChangesAsync();

        var remaining = await ctx.PremadeListItems.CountAsync(i => i.PremadeListId == list.Id);
        Assert.That(remaining, Is.EqualTo(0));
    }
}
