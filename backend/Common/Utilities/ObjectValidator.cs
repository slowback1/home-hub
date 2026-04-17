using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Common.Utilities.Attributes;

namespace Common.Utilities;

public static class ObjectValidator
{
	public static List<string> ValidateObject<T>(T? input)
	{
		if (ShouldSkipValidating(input)) return new List<string>();

		if (input == null) return new List<string>();

		var propertiesToValidate =
			GetValidationAttributesFromProperties(input).Concat(GetValidationAttributesFromClass(input));

		var result = ValidateEachAttribute(propertiesToValidate);

		return result;
	}

	private static bool ShouldSkipValidating<T>(T? input)
	{
		if (input is null || input is string) return true;

		return false;
	}

	private static List<string> ValidateEachAttribute(IEnumerable<PropertyAttributeValuePair> propertiesToValidate)
	{
		var result = new List<string>();

		foreach (var property in propertiesToValidate)
		{
			var attributes = property.CustomAttributes;
			foreach (var attribute in attributes)
			{
				var validation = attribute.CheckForValidationError(property.Value);
				if (validation != null) result.Add(validation);
			}
		}

		return result;
	}

	private static IEnumerable<PropertyAttributeValuePair> GetValidationAttributesFromClass<T>([DisallowNull] T input)
	{
		var attributes = input.GetType().GetCustomAttributes(typeof(ValidationAttribute), true)
			.Cast<ValidationAttribute>();

		return new List<PropertyAttributeValuePair>
		{
			new()
			{
				Value = input,
				CustomAttributes = attributes
			}
		};
	}

	private static IEnumerable<PropertyAttributeValuePair> GetValidationAttributesFromProperties<T>(
		[DisallowNull] T input)
	{
		var properties = input.GetType().GetProperties();
		var propertiesToValidate = properties.Select(p =>
				new PropertyAttributeValuePair
				{
					CustomAttributes = p.GetCustomAttributes(typeof(ValidationAttribute), true)
						.Cast<ValidationAttribute>()
						.Select(a =>
						{
							a.PropertyName = p.Name;
							return a;
						}),
					Value = p.GetValue(input)
				})
			.Where(v => v.CustomAttributes.Any())
			.ToList();

		var enumerableProperties = properties.Where(p => typeof(IEnumerable).IsAssignableFrom(p.PropertyType));

		foreach (var enumerable in enumerableProperties)
		{
			var value = enumerable.GetValue(input) as IEnumerable;

			if (value is null) continue;

			foreach (var item in value) propertiesToValidate.AddRange(GetValidationAttributesFromProperties(item));
		}

		return propertiesToValidate;
	}

	private class PropertyAttributeValuePair
	{
		public IEnumerable<ValidationAttribute> CustomAttributes { get; set; } = [];

		public object? Value { get; set; }
	}
}