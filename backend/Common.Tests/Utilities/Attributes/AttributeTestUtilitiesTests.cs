using Common.Utilities.Attributes;
using TestUtilities;

namespace Common.Tests.Utilities.Attributes;

[MatchingProperty(nameof(AnyNumericProperty), nameof(NumericProperty))]
public class ExampleClass
{
	[NumericValue(1, 10)]
	public int NumericProperty { get; set; }

	public float AnotherNumericProperty { get; set; }

	public decimal AnyNumericProperty { get; set; }

	public string NonNumericProperty { get; set; } = string.Empty;
}

public class AttributeTestUtilitiesTests
{
	[Test]
	public void CorrectlyAssertsThatClassHasAttribute()
	{
		typeof(ExampleClass).HasAttribute<MatchingPropertyAttribute>();
	}

	[Test]
	public void CorrectlyAssertsThatPropertyExists()
	{
		var property = typeof(ExampleClass).GetProperty("NumericProperty");
		Assert.That(property, Is.Not.Null);
	}

	[Test]
	public void CorrectlyAssertsThatPropertyHasAttribute()
	{
		var property = typeof(ExampleClass)
			.GetProperty("NumericProperty")
			?.PropertyHasAttribute<NumericValueAttribute>();

		Assert.That(property, Is.Not.Null);
	}
}