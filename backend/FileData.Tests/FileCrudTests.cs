using System.Text.Json;
using Common.Interfaces;

namespace FileData.Tests;

public class TestFileCrud<T>(string dataDirectory = "data") : FileCrud<T>(dataDirectory) where T : class, IIdentifyable;

public class TestEntityForFileCrud : IIdentifyable
{
	public string Name { get; set; } = string.Empty;
	public int Number { get; init; }
	public string Id { get; set; } = string.Empty;
}

[TestFixture]
public class FileCrudTests
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

	private TestFileCrud<TestEntityForFileCrud> CreateCrud()
	{
		return new TestFileCrud<TestEntityForFileCrud>(_testDataDirectory);
	}

	[Test]
	public async Task FileOperations_CreateProperJsonFile()
	{
		var crud = CreateCrud();
		var entity = new TestEntityForFileCrud { Name = "JsonTest", Number = 42 };

		var created = await crud.CreateAsync(entity);

		var filePath = Path.Combine(_testDataDirectory, "TestEntityForFileCrud.json");
		Assert.That(File.Exists(filePath), Is.True);

		var json = await File.ReadAllTextAsync(filePath);
		Assert.That(json, Does.Contain("JsonTest"));
		Assert.That(json, Does.Contain("42"));
		Assert.That(json, Does.Contain(created.Id));

		var deserializedEntities = JsonSerializer.Deserialize<List<TestEntityForFileCrud>>(json);
		Assert.That(deserializedEntities, Is.Not.Null);
		Assert.That(deserializedEntities!.Count, Is.EqualTo(1));
		Assert.That(deserializedEntities[0].Name, Is.EqualTo("JsonTest"));
		Assert.That(deserializedEntities[0].Number, Is.EqualTo(42));
	}

	[Test]
	public async Task FileOperations_HandleEmptyFileCorrectly()
	{
		var crud = CreateCrud();

		Assert.That(await crud.GetByIdAsync("nonexistent"), Is.Null);
		Assert.That(await crud.QueryAsync(_ => true), Is.Empty);
		Assert.That(await crud.DeleteAsync("nonexistent"), Is.False);
	}

	[Test]
	public async Task FileOperations_HandleCorruptedJsonFile()
	{
		var crud = CreateCrud();
		var filePath = Path.Combine(_testDataDirectory, "TestEntityForFileCrud.json");

		await File.WriteAllTextAsync(filePath, "{ invalid json content }");

		var entity = new TestEntityForFileCrud { Name = "Recovery", Number = 1 };
		var created = await crud.CreateAsync(entity);

		Assert.That(created, Is.Not.Null);
		Assert.That(created.Name, Is.EqualTo("Recovery"));

		var json = await File.ReadAllTextAsync(filePath);
		var deserializedEntities = JsonSerializer.Deserialize<List<TestEntityForFileCrud>>(json);
		Assert.That(deserializedEntities, Is.Not.Null);
		Assert.That(deserializedEntities!.Count, Is.EqualTo(1));
	}

	[Test]
	public async Task FileOperations_MaintainDataIntegrityWithMultipleOperations()
	{
		var crud = CreateCrud();
		var entities = new[]
		{
			new TestEntityForFileCrud { Name = "Entity1", Number = 1 },
			new TestEntityForFileCrud { Name = "Entity2", Number = 2 },
			new TestEntityForFileCrud { Name = "Entity3", Number = 3 }
		};

		var created = new List<TestEntityForFileCrud>();
		foreach (var entity in entities) created.Add(await crud.CreateAsync(entity));

		created[1].Name = "UpdatedEntity2";
		await crud.UpdateAsync(created[1].Id, created[1]);

		await crud.DeleteAsync(created[2].Id);

		var filePath = Path.Combine(_testDataDirectory, "TestEntityForFileCrud.json");
		var json = await File.ReadAllTextAsync(filePath);
		var deserializedEntities = JsonSerializer.Deserialize<List<TestEntityForFileCrud>>(json);

		Assert.That(deserializedEntities, Is.Not.Null);
		Assert.That(deserializedEntities!.Count, Is.EqualTo(2)); // One deleted

		var entity1 = deserializedEntities.First(e => e.Id == created[0].Id);
		var entity2 = deserializedEntities.First(e => e.Id == created[1].Id);

		Assert.That(entity1.Name, Is.EqualTo("Entity1"));
		Assert.That(entity1.Number, Is.EqualTo(1));

		Assert.That(entity2.Name, Is.EqualTo("UpdatedEntity2")); // Updated
		Assert.That(entity2.Number, Is.EqualTo(2));

		Assert.That(deserializedEntities.Any(e => e.Id == created[2].Id), Is.False); // Deleted
	}

	[Test]
	public async Task DataDirectory_CreatedAutomatically()
	{
		var nonExistentDirectory = Path.Combine(_testDataDirectory, "non-existent-subdir");
		Assert.That(Directory.Exists(nonExistentDirectory), Is.False);

		var crud = new TestFileCrud<TestEntityForFileCrud>(nonExistentDirectory);
		var entity = new TestEntityForFileCrud { Name = "AutoCreate", Number = 1 };

		await crud.CreateAsync(entity);

		Assert.That(Directory.Exists(nonExistentDirectory), Is.True);
		var filePath = Path.Combine(nonExistentDirectory, "TestEntityForFileCrud.json");
		Assert.That(File.Exists(filePath), Is.True);
	}
}