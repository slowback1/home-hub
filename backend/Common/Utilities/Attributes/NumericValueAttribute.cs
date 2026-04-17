using System;

namespace Common.Utilities.Attributes;

public class NumericValueAttribute : ValidationAttribute
{
	private readonly float _max;
	private readonly float _min;
	private readonly string _nameOverride;

	public NumericValueAttribute(float min = float.MinValue, float max = float.MaxValue, string propertyName = "")
	{
		_min = min;
		_max = max;
		_nameOverride = propertyName;

		if (_max < _min)
			throw new ArgumentException("Max value cannot be less than min value");
	}

	public NumericValueAttribute() : this(float.MinValue)
	{
	}

	public NumericValueAttribute(int min = int.MinValue, int max = int.MaxValue, string propertyName = "")
		: this(min, (float)max, propertyName)
	{
	}

	public NumericValueAttribute(decimal min = decimal.MinValue,
		decimal max = decimal.MaxValue,
		string propertyName = "")
		: this((float)min, (float)max, propertyName)
	{
	}

	public NumericValueAttribute(long min = long.MinValue, long max = long.MaxValue, string propertyName = "")
		: this(min, (float)max, propertyName)
	{
	}

	public override string? CheckForValidationError(object? value)
	{
		var isNumeric = float.TryParse(value?.ToString(), out var number);

		if (!isNumeric)
			return "Value is not a number";

		var propertyName = GetPropertyName();

		if (number < _min)
			return $"{propertyName} is below minimum of {_min}";

		if (number > _max)
			return $"{propertyName} is above maximum value of {_max}";

		return null;
	}

	private string GetPropertyName()
	{
		return string.IsNullOrWhiteSpace(_nameOverride) ? PropertyName : _nameOverride;
	}
}