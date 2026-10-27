using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Tests;

[TestFixture]
public class PerUserModelTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Test]
    public void Activity_HasUserIdForeignKeyToAppUsers()
    {
        using var ctx = CreateContext();
        var entity = ctx.Model.FindEntityType(typeof(Activity));
        Assert.That(entity, Is.Not.Null);
        Assert.That(entity.FindProperty(nameof(Activity.UserId)), Is.Not.Null);
        AssertForeignKeyToAppUsers(entity);
    }

    [Test]
    public void ActivityPick_HasUserIdForeignKeyToAppUsers()
    {
        using var ctx = CreateContext();
        var entity = ctx.Model.FindEntityType(typeof(ActivityPick));
        Assert.That(entity, Is.Not.Null);
        Assert.That(entity.FindProperty(nameof(ActivityPick.UserId)), Is.Not.Null);
        AssertForeignKeyToAppUsers(entity);
    }

    [Test]
    public void Bookmark_HasUserIdForeignKeyToAppUsers()
    {
        using var ctx = CreateContext();
        var entity = ctx.Model.FindEntityType(typeof(Bookmark));
        Assert.That(entity, Is.Not.Null);
        Assert.That(entity.FindProperty(nameof(Bookmark.UserId)), Is.Not.Null);
        AssertForeignKeyToAppUsers(entity);
    }

    [Test]
    public void Wheel_HasUserIdForeignKeyToAppUsers()
    {
        using var ctx = CreateContext();
        var entity = ctx.Model.FindEntityType(typeof(Wheel));
        Assert.That(entity, Is.Not.Null);
        Assert.That(entity.FindProperty(nameof(Wheel.UserId)), Is.Not.Null);
        AssertForeignKeyToAppUsers(entity);
    }

    private static void AssertForeignKeyToAppUsers(Microsoft.EntityFrameworkCore.Metadata.IEntityType entity)
    {
        var hasFk = entity.GetForeignKeys().Any(fk => fk.PrincipalEntityType.ClrType == typeof(AppUser));
        Assert.That(hasFk, Is.True);
    }
}
