using Common.Interfaces;
using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Tests;

public class TestEntity : IIdentifyable
{
    public string Name { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
}

[TestFixture]
public class EfCrudTests
{
    private TestDbContextFactory CreateFactory(bool forTestEntity = false)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new TestDbContextFactory(options, forTestEntity);
    }

    private EfCrud<T> CreateCrud<T>(TestDbContextFactory factory) where T : class, IIdentifyable
        => new EfCrud<T>(factory);

    [Test]
    public async Task CreateAsync_AssignsIdAndStoresEntity()
    {
        using var factory = CreateFactory(true);
        var crud = CreateCrud<TestEntity>(factory);
        var entity = new TestEntity { Name = "Test" };
        var created = await crud.CreateAsync(entity);
        Assert.That(created.Id, Is.Not.Null.And.Not.Empty);
        Assert.That(created.Name, Is.EqualTo("Test"));
        Assert.That(await crud.GetByIdAsync(created.Id), Is.Not.Null);
    }

    [Test]
    public async Task GetByIdAsync_ReturnsNullIfNotFound()
    {
        using var factory = CreateFactory(true);
        var crud = CreateCrud<TestEntity>(factory);
        Assert.That(await crud.GetByIdAsync("999"), Is.Null);
    }

    [Test]
    public async Task UpdateAsync_ReplacesEntityIfExists()
    {
        using var factory = CreateFactory(true);
        var crud = CreateCrud<TestEntity>(factory);
        var entity = new TestEntity { Name = "Old" };
        var created = await crud.CreateAsync(entity);
        var updated = new TestEntity { Id = created.Id, Name = "New" };
        var result = await crud.UpdateAsync(created.Id, updated);
        Assert.That(result, Is.Not.Null);
        Assert.That((await crud.GetByIdAsync(created.Id))!.Name, Is.EqualTo("New"));
    }

    [Test]
    public async Task UpdateAsync_ReturnsNullIfNotFound()
    {
        using var factory = CreateFactory(true);
        var crud = CreateCrud<TestEntity>(factory);
        var entity = new TestEntity { Id = "999", Name = "Ghost" };
        Assert.That(await crud.UpdateAsync("999", entity), Is.Null);
    }

    [Test]
    public async Task DeleteAsync_RemovesEntityIfExists()
    {
        using var factory = CreateFactory(true);
        var crud = CreateCrud<TestEntity>(factory);
        var entity = new TestEntity { Name = "ToDelete" };
        var created = await crud.CreateAsync(entity);
        Assert.That(await crud.DeleteAsync(created.Id), Is.True);
        Assert.That(await crud.GetByIdAsync(created.Id), Is.Null);
    }

    [Test]
    public async Task DeleteAsync_ReturnsFalseIfNotFound()
    {
        using var factory = CreateFactory(true);
        var crud = CreateCrud<TestEntity>(factory);
        Assert.That(await crud.DeleteAsync("notfound"), Is.False);
    }

    [Test]
    public async Task GetByQueryAsync_ReturnsMatchingEntity()
    {
        using var factory = CreateFactory(true);
        var crud = CreateCrud<TestEntity>(factory);
        await crud.CreateAsync(new TestEntity { Name = "A" });
        await crud.CreateAsync(new TestEntity { Name = "B" });
        var result = await crud.GetByQueryAsync(e => e.Name == "B");
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("B"));
    }

    [Test]
    public async Task QueryAsync_ReturnsAllMatchingEntities()
    {
        using var factory = CreateFactory(true);
        var crud = CreateCrud<TestEntity>(factory);
        await crud.CreateAsync(new TestEntity { Name = "X" });
        await crud.CreateAsync(new TestEntity { Name = "X" });
        await crud.CreateAsync(new TestEntity { Name = "Y" });
        var results = await crud.QueryAsync(e => e.Name == "X");
        Assert.That(results.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task EfCrudFactory_ReturnsEfCrud()
    {
        using var testFactory = CreateFactory(true);
        var factory = new EfCrudFactory(testFactory);
        var crud = factory.GetCrud<TestEntity>();
        var entity = new TestEntity { Name = "ViaFactory" };
        var created = await crud.CreateAsync(entity);
        Assert.That(created.Id, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task AppDbContext_SupportsAppUserAndExampleData()
    {
        using var factory = CreateFactory();
        var userCrud = new EfCrud<AppUser>(factory);
        var exampleCrud = new EfCrud<ExampleData>(factory);
        var user = new AppUser { Name = "X" };
        var ex = new ExampleData { Name = "E", Value = 42 };
        var createdUser = await userCrud.CreateAsync(user);
        var createdEx = await exampleCrud.CreateAsync(ex);
        Assert.That(await userCrud.GetByIdAsync(createdUser.Id), Is.Not.Null);
        Assert.That(await exampleCrud.GetByIdAsync(createdEx.Id), Is.Not.Null);
    }
}
