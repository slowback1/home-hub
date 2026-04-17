using Common.Interfaces;

namespace FileData.Tests;

public class TestEntity : IIdentifyable
{
    public string Name { get; init; } = string.Empty;
    public string Id { get; set; } = string.Empty;
}

[TestFixture]
public class FileGenericCrudTests
{
    [SetUp]
    public void SetUp()
    {
        _testDataDirectory = Path.Combine(Path.GetTempPath(), "FileData.Tests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDataDirectory);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_testDataDirectory)) Directory.Delete(_testDataDirectory, true);
    }

    private string _testDataDirectory = string.Empty;

    private FileGenericCrud<TestEntity> CreateCrud()
    {
        return new FileGenericCrud<TestEntity>(_testDataDirectory);
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
    public async Task CreateAsync_PersistsDataToFile()
    {
        var crud = CreateCrud();
        var entity = new TestEntity { Name = "Persisted" };
        var created = await crud.CreateAsync(entity);

        var newCrud = CreateCrud();
        var retrieved = await newCrud.GetByIdAsync(created.Id);
        Assert.That(retrieved, Is.Not.Null);
        Assert.That(retrieved!.Name, Is.EqualTo("Persisted"));
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
    public async Task UpdateAsync_PreservesId()
    {
        var crud = CreateCrud();
        var entity = new TestEntity { Name = "Original" };
        var created = await crud.CreateAsync(entity);
        var originalId = created.Id;

        var updated = new TestEntity { Id = "different-id", Name = "Updated" };
        var result = await crud.UpdateAsync(originalId, updated);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(originalId));
        Assert.That(result.Name, Is.EqualTo("Updated"));
    }

    [Test]
    public async Task UpdateAsync_PersistsChangesToFile()
    {
        var crud = CreateCrud();
        var entity = new TestEntity { Name = "Original" };
        var created = await crud.CreateAsync(entity);
        var updated = new TestEntity { Name = "Updated" };
        await crud.UpdateAsync(created.Id, updated);

        var newCrud = CreateCrud();
        var retrieved = await newCrud.GetByIdAsync(created.Id);
        Assert.That(retrieved, Is.Not.Null);
        Assert.That(retrieved!.Name, Is.EqualTo("Updated"));
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
    public async Task DeleteAsync_PersistsRemovalToFile()
    {
        var crud = CreateCrud();
        var entity = new TestEntity { Name = "ToDelete" };
        var created = await crud.CreateAsync(entity);
        await crud.DeleteAsync(created.Id);

        var newCrud = CreateCrud();
        Assert.That(await newCrud.GetByIdAsync(created.Id), Is.Null);
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

    [Test]
    public async Task ConcurrentOperations_AreThreadSafe()
    {
        var crud = CreateCrud();
        var tasks = new List<Task<TestEntity>>();

        for (var i = 0; i < 10; i++)
        {
            var entity = new TestEntity { Name = $"Entity{i}" };
            tasks.Add(crud.CreateAsync(entity));
        }

        var results = await Task.WhenAll(tasks);

        Assert.That(results.Length, Is.EqualTo(10));
        var ids = results.Select(r => r.Id).ToHashSet();
        Assert.That(ids.Count, Is.EqualTo(10)); // All IDs should be unique

        foreach (var result in results)
        {
            var retrieved = await crud.GetByIdAsync(result.Id);
            Assert.That(retrieved, Is.Not.Null);
            Assert.That(retrieved!.Name, Is.EqualTo(result.Name));
        }
    }

    [Test]
    public void ClearData_RemovesAllDataFromFile()
    {
        var crud = CreateCrud();
        var entity = new TestEntity { Name = "Test" };
        var created = crud.CreateAsync(entity).Result;

        Assert.That(crud.GetByIdAsync(created.Id).Result, Is.Not.Null);

        crud.ClearData();

        var newCrud = CreateCrud();
        Assert.That(newCrud.GetByIdAsync(created.Id).Result, Is.Null);
    }
}