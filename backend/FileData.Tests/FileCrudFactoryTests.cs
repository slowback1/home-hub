using Common.Interfaces;

namespace FileData.Tests;

public class DummyEntity : IIdentifyable
{
	public string Name { get; set; } = string.Empty;
	public string Id { get; set; } = string.Empty;
}

public abstract class OtherEntity : IIdentifyable
{
	public string Description { get; set; } = string.Empty;
	public string Id { get; set; } = string.Empty;
}

[TestFixture]
public class FileCrudFactoryTests
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

	[Test]
	public void GetCrud_ReturnsFileGenericCrud_ForOtherTypes()
	{
		var factory = new FileCrudFactory(_testDataDirectory);
		var crud = factory.GetCrud<DummyEntity>();
		Assert.That(crud, Is.InstanceOf<FileGenericCrud<DummyEntity>>());
		Assert.That(crud, Is.InstanceOf<ICrud<DummyEntity>>());
	}

	[Test]
	public void GetCrud_ReturnsDifferentInstances_ForDifferentCalls()
	{
		var factory = new FileCrudFactory(_testDataDirectory);
		var crud1 = factory.GetCrud<DummyEntity>();
		var crud2 = factory.GetCrud<DummyEntity>();
		Assert.That(crud1, Is.Not.SameAs(crud2));
	}

	[Test]
	public void GetCrud_ReturnsDifferentInstances_ForDifferentTypes()
	{
		var factory = new FileCrudFactory(_testDataDirectory);
		var otherCrud = factory.GetCrud<OtherEntity>();
		var dummyCrud = factory.GetCrud<DummyEntity>();
		Assert.That(otherCrud, Is.Not.SameAs(dummyCrud));
	}

	[Test]
	public async Task GetCrud_CreatesInstancesWithCorrectDataDirectory()
	{
		var factory = new FileCrudFactory(_testDataDirectory);
		var crud = factory.GetCrud<DummyEntity>();

		var entity = new DummyEntity { Name = "Test" };
		var created = await crud.CreateAsync(entity);

		var expectedFilePath = Path.Combine(_testDataDirectory, "DummyEntity.json");
		Assert.That(File.Exists(expectedFilePath), Is.True);

		var fileContent = await File.ReadAllTextAsync(expectedFilePath);
		Assert.That(fileContent, Does.Contain("Test"));
		Assert.That(fileContent, Does.Contain(created.Id));
	}
}