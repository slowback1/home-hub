using Common.Interfaces;

namespace InMemory.Tests;

public class TestEntityA : IIdentifyable
{
    public string Name { get; init; } = string.Empty;
    public string Id { get; set; } = string.Empty;
}

public class TestEntityB : IIdentifyable
{
    public int Value { get; init; }
    public string Id { get; set; } = string.Empty;
}

[TestFixture]
public class InMemoryCrudStaticTests
{
    [SetUp]
    public void SetUp()
    {
        InMemoryCrud<TestEntityA>.ClearStaticState();
        InMemoryCrud<TestEntityB>.ClearStaticState();
    }

    [Test]
    public async Task Items_Are_Stored_Separately_By_Type()
    {
        var crudA = new InMemoryGenericCrud<TestEntityA>();
        var crudB = new InMemoryGenericCrud<TestEntityB>();
        var a = await crudA.CreateAsync(new TestEntityA { Name = "A1" });
        var b = await crudB.CreateAsync(new TestEntityB { Value = 42 });
        var aResult = await crudA.GetByIdAsync(a.Id);
        var bResult = await crudB.GetByIdAsync(b.Id);
        Assert.That(aResult, Is.Not.Null);
        Assert.That(bResult, Is.Not.Null);
        Assert.That(aResult!.Name, Is.EqualTo("A1"));
        Assert.That(bResult!.Value, Is.EqualTo(42));
    }

    [Test]
    public async Task Query_And_Delete_Are_Type_Specific()
    {
        var crudA = new InMemoryGenericCrud<TestEntityA>();
        var crudB = new InMemoryGenericCrud<TestEntityB>();
        var a = await crudA.CreateAsync(new TestEntityA { Name = "A3" });
        var b = await crudB.CreateAsync(new TestEntityB { Value = 123 });
        var deletedA = await crudA.DeleteAsync(a.Id);
        var deletedB = await crudB.DeleteAsync(b.Id);
        Assert.That(deletedA, Is.True);
        Assert.That(deletedB, Is.True);
        Assert.That(await crudA.GetByIdAsync(a.Id), Is.Null);
        Assert.That(await crudB.GetByIdAsync(b.Id), Is.Null);
    }

    [Test]
    public async Task ClearStaticState_Resets_Items_And_Ids()
    {
        var crudA = new InMemoryGenericCrud<TestEntityA>();
        var a1 = await crudA.CreateAsync(new TestEntityA { Name = "reset1" });
        InMemoryCrud<TestEntityA>.ClearStaticState();
        var a2 = await crudA.CreateAsync(new TestEntityA { Name = "reset2" });
        Assert.That(int.Parse(a2.Id), Is.EqualTo(1));
        InMemoryCrud<TestEntityA>.ClearStaticState();
        var result = await crudA.GetByIdAsync(a1.Id);
        Assert.That(result, Is.Null);
    }
}