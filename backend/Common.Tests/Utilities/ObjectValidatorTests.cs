using Common.Utilities;
using Common.Utilities.Attributes;

namespace Common.Tests.Utilities;

public class TestObject
{
	[Length(5)]
	[Required]
	public string Value { get; set; } = string.Empty;
}

[MatchingProperty(nameof(A), nameof(B))]
public class MatchingPropertyTest
{
	public string A { get; set; } = string.Empty;
	public string B { get; set; } = string.Empty;
}

public class TestEmpty
{
	public string A { get; set; } = string.Empty;
}

public class Child
{
	[Length(min: 2)]
	public string Value { get; set; } = string.Empty;
}

public class ParentList
{
	public List<Child> Children { get; set; } = new();
}

public class ObjectValidatorTests
{
	[Test]
	public void ReturnsEmptyListWhenGivenAValidObject()
	{
		var test = new TestObject { Value = "test" };

		var result = ObjectValidator.ValidateObject(test);

		Assert.That(result, Is.Empty);
	}

	[Test]
	public void ReturnsListOfValidationErrorsWhenGivenInvalidObject()
	{
		var test = new TestObject { Value = "really long" };

		var result = ObjectValidator.ValidateObject(test);

		Assert.That(result.Count, Is.EqualTo(1));
	}

	[Test]
	public void ReturnsAnEmptyListForANullObject()
	{
		TestObject? test = null;

		var result = ObjectValidator.ValidateObject(test);

		Assert.That(result, Is.Empty);
	}

	[Test]
	public void AssignsPropertyNameToPropertyNameValidationAttributes()
	{
		var test = new TestObject { Value = "" };

		var result = ObjectValidator.ValidateObject(test);

		Assert.That(result, Has.Count.EqualTo(1));
		Assert.That(result.First(), Contains.Substring("Value"));
	}

	[Test]
	public void CanPickUpOnClassLevelAttributes()
	{
		var test = new MatchingPropertyTest
		{
			A = "a",
			B = "b"
		};

		var result = ObjectValidator.ValidateObject(test);

		Assert.That(result, Has.Count.EqualTo(1));
	}

	[Test]
	public void ReturnsEmptyListForAClassWithNoValidationAttributes()
	{
		var test = new TestEmpty { A = "value" };

		var result = ObjectValidator.ValidateObject(test);

		Assert.That(result, Is.Empty);
	}

	[Test]
	public void CanGetValidationErrorsFromChildObjectsInAList()
	{
		var test = new ParentList { Children = [new Child { Value = "A" }] };

		var result = ObjectValidator.ValidateObject(test);

		Assert.That(result, Has.Count.EqualTo(1));
	}

	[Test]
	[TestCase("test")]
	[TestCase(1)]
	[TestCase(1.11)]
	[TestCase(false)]
	[TestCase('c')]
	[TestCase(0x11)]
	public void DoesntBreakWhenGivenANonClassObject(object value)
	{
		var result = ObjectValidator.ValidateObject(value);

		Assert.That(result.Count, Is.EqualTo(0));
	}
}