using Common.Interfaces;

namespace InMemory.Tests;

public abstract class DummyEntity : IIdentifyable
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
public class InMemoryCrudFactoryTests
{
	[Test]
	public void GetCrud_ReturnsInMemoryGenericCrud_ForAGivenType()
	{
		var factory = new InMemoryCrudFactory();
		var crud = factory.GetCrud<DummyEntity>();
		Assert.That(crud, Is.InstanceOf<InMemoryGenericCrud<DummyEntity>>());
		Assert.That(crud, Is.InstanceOf<ICrud<DummyEntity>>());
	}

	[Test]
	public void GetCrud_ReturnsDifferentInstances_ForDifferentTypes()
	{
		var factory = new InMemoryCrudFactory();
		var otherCrud = factory.GetCrud<OtherEntity>();
		var dummyCrud = factory.GetCrud<DummyEntity>();
		Assert.That(otherCrud, Is.Not.SameAs(dummyCrud));
	}
}