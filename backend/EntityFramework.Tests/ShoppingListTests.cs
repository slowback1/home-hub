using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Tests;

[TestFixture]
public class ShoppingListTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Test]
    public async Task ShoppingLists_CanStoreAndRetrieveList()
    {
        using var ctx = CreateContext();
        var list = new ShoppingList
        {
            Id = Guid.NewGuid().ToString(),
            UserId = "user-1",
            Name = "Groceries",
            Status = ShoppingListStatus.Active,
            CreatedAt = DateTime.UtcNow,
        };

        ctx.ShoppingLists.Add(list);
        await ctx.SaveChangesAsync();

        var saved = await ctx.ShoppingLists.FirstOrDefaultAsync(l => l.Id == list.Id);
        Assert.That(saved, Is.Not.Null);
        Assert.That(saved!.Name, Is.EqualTo("Groceries"));
        Assert.That(saved.Status, Is.EqualTo(ShoppingListStatus.Active));
        Assert.That(saved.ArchivedAt, Is.Null);
    }

    [Test]
    public async Task ShoppingListItems_CanStoreAndRetrieveItem()
    {
        using var ctx = CreateContext();
        var list = new ShoppingList
        {
            Id = Guid.NewGuid().ToString(),
            UserId = "user-1",
            Name = "Groceries",
            Status = ShoppingListStatus.Active,
            CreatedAt = DateTime.UtcNow,
        };
        var item = new ShoppingListItem
        {
            Id = Guid.NewGuid().ToString(),
            ListId = list.Id,
            Text = "Milk",
            IsChecked = false,
            CreatedAt = DateTime.UtcNow,
        };

        ctx.ShoppingLists.Add(list);
        ctx.ShoppingListItems.Add(item);
        await ctx.SaveChangesAsync();

        var saved = await ctx.ShoppingListItems.FirstOrDefaultAsync(i => i.Id == item.Id);
        Assert.That(saved, Is.Not.Null);
        Assert.That(saved!.Text, Is.EqualTo("Milk"));
        Assert.That(saved.IsChecked, Is.False);
        Assert.That(saved.ListId, Is.EqualTo(list.Id));
    }

    [Test]
    public async Task ShoppingListItems_CascadeDeleteWithList()
    {
        using var ctx = CreateContext();
        var list = new ShoppingList
        {
            Id = Guid.NewGuid().ToString(),
            UserId = "user-1",
            Name = "Groceries",
            Status = ShoppingListStatus.Active,
            CreatedAt = DateTime.UtcNow,
        };
        var item = new ShoppingListItem
        {
            Id = Guid.NewGuid().ToString(),
            ListId = list.Id,
            Text = "Milk",
            CreatedAt = DateTime.UtcNow,
        };

        ctx.ShoppingLists.Add(list);
        ctx.ShoppingListItems.Add(item);
        await ctx.SaveChangesAsync();

        ctx.ShoppingLists.Remove(list);
        await ctx.SaveChangesAsync();

        var remaining = await ctx.ShoppingListItems.CountAsync(i => i.ListId == list.Id);
        Assert.That(remaining, Is.EqualTo(0));
    }
}
