using Common.Interfaces;

namespace InMemory.Tests;

public class TestEntity : IIdentifyable
{
    public string Name { get; init; } = string.Empty;
    public string Id { get; set; } = string.Empty;
}

[TestFixture]
public class InMemoryGenericCrudTests
{
    private InMemoryGenericCrud<TestEntity> CreateCrud()
    {
        return new InMemoryGenericCrud<TestEntity>();
    }

    [Test]
    public async Task CreateAsync_AssignsIdAndStoresEntity()
    {
        var crud = CreateCrud();
        var entity = new TestEntity { Name = "Test" };
        var created = await crud.CreateAsync(entity);
        Assert.That(created.Id, Is.Not.Null.And.Not.Empty);
        Assert.That(created.Name, Is.EqualTo("Test"));
        Assert.That(await crud.GetByIdAsync(created.Id), Is.Not.Null);
    }

    [Test]
    public async Task GetByIdAsync_ReturnsNullIfNotFound()
    {
        var crud = CreateCrud();
        Assert.That(await crud.GetByIdAsync("999"), Is.Null);
    }

    [Test]
    public async Task UpdateAsync_ReplacesEntityIfExists()
    {
        var crud = CreateCrud();
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
        var crud = CreateCrud();
        var entity = new TestEntity { Id = "999", Name = "Ghost" };
        Assert.That(await crud.UpdateAsync("999", entity), Is.Null);
    }

    [Test]
    public async Task DeleteAsync_RemovesEntityIfExists()
    {
        var crud = CreateCrud();
        var entity = new TestEntity { Name = "ToDelete" };
        var created = await crud.CreateAsync(entity);
        Assert.That(await crud.DeleteAsync(created.Id), Is.True);
        Assert.That(await crud.GetByIdAsync(created.Id), Is.Null);
    }

    [Test]
    public async Task DeleteAsync_ReturnsFalseIfNotFound()
    {
        var crud = CreateCrud();
        Assert.That(await crud.DeleteAsync("notfound"), Is.False);
    }

    [Test]
    public async Task GetByQueryAsync_ReturnsMatchingEntity()
    {
        var crud = CreateCrud();
        await crud.CreateAsync(new TestEntity { Name = "A" });
        await crud.CreateAsync(new TestEntity { Name = "B" });
        var result = await crud.GetByQueryAsync(e => e.Name == "B");
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("B"));
    }

    [Test]
    public async Task QueryAsync_ReturnsAllMatchingEntities()
    {
        var crud = CreateCrud();
        await crud.CreateAsync(new TestEntity { Name = "X" });
        await crud.CreateAsync(new TestEntity { Name = "X" });
        await crud.CreateAsync(new TestEntity { Name = "Y" });
        var results = await crud.QueryAsync(e => e.Name == "X");
        Assert.That(results.Count(), Is.EqualTo(2));
    }
}