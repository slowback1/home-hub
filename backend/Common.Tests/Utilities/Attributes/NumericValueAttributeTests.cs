using Common.Utilities.Attributes;

namespace Common.Tests.Utilities.Attributes;

public class NumericValueAttributeTests
{
	[Test]
	public void CheckForValidationError_WhenCalledWithNonNumericValue_ShouldReturnErrorMessage()
	{
		var attribute = new NumericValueAttribute();
		var value = "test";

		var result = attribute.CheckForValidationError(value);

		Assert.That(result, Is.EqualTo("Value is not a number"));
	}

	[Test]
	public void CheckForValidationError_WhenCalledWithNumericValue_ShouldReturnNull()
	{
		var attribute = new NumericValueAttribute();
		var value = 1;

		var result = attribute.CheckForValidationError(value);

		Assert.IsNull(result);
	}

	[Test]
	public void CheckForValidationError_WhenCalledWithANumberBelowMin_ShouldReturnAnErrorMessage()
	{
		var attribute = new NumericValueAttribute(5) { PropertyName = "MyNumericProperty" };

		var value = 4;

		var result = attribute.CheckForValidationError(value);

		Assert.That(result, Is.EqualTo("MyNumericProperty is below minimum of 5"));
	}

	[Test]
	public void CheckForValidationError_WhenCalledWithANumberAboveMax_ShouldReturnAnErrorMessage()
	{
		var attribute = new NumericValueAttribute(max: 5) { PropertyName = "MyNumericProperty" };
		var value = 6;

		var result = attribute.CheckForValidationError(value);

		Assert.That(result, Is.EqualTo("MyNumericProperty is above maximum value of 5"));
	}

	[Test]
	public void CheckForValidationError_WhenCalledWithANumberEqualToMax_ShouldReturnNull()
	{
		var attribute = new NumericValueAttribute(max: 5) { PropertyName = "MyNumericProperty" };
		var value = 5;

		var result = attribute.CheckForValidationError(value);

		Assert.IsNull(result);
	}

	[Test]
	public void CheckForValidationError_WhenCalledWithANumberEqualToMin_ShouldReturnNull()
	{
		var attribute = new NumericValueAttribute(5) { PropertyName = "MyNumericProperty" };
		var value = 5;

		var result = attribute.CheckForValidationError(value);

		Assert.IsNull(result);
	}

	[Test]
	public void CheckForValidationError_WhenConstructedWithAMaxLowerThanMin_ShouldThrowAnException()
	{
		Assert.Throws<ArgumentException>(() =>
		{
			var unused = new NumericValueAttribute(max: 5, min: 10);
		});
	}

	[Test]
	public void CheckForValidationError_WorksForFloats()
	{
		var attribute = new NumericValueAttribute(0.0f, 1.1f);
		var value = 1.0f;

		var result = attribute.CheckForValidationError(value);

		Assert.IsNull(result);
	}

	[Test]
	public void CheckForValidationError_WorksForDecimals()
	{
		var attribute = new NumericValueAttribute(0.0m, 1.1m);
		var value = 1.0m;

		var result = attribute.CheckForValidationError(value);

		Assert.IsNull(result);
	}

	[Test]
	public void CheckForValidationError_WorksForLongs()
	{
		var attribute = new NumericValueAttribute(1L, 10000L);
		var value = 5L;

		var result = attribute.CheckForValidationError(value);

		Assert.IsNull(result);
	}

	[Test]
	public void CanCustomizeThePropertyNameInTheErrorMessage()
	{
		var attribute = new NumericValueAttribute(1L, 10000L, "My Prettified Name") { PropertyName = "MyProperty" };
		var value = 0L;

		var result = attribute.CheckForValidationError(value);

		Assert.That(result, Is.EqualTo("My Prettified Name is below minimum of 1"));
	}
}