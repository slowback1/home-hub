using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common.Utilities.Attributes;

public class UniqueListValueAttribute(string propertyName, string? errorMessage = null) : ValidationAttribute
{
	public override string? CheckForValidationError(object? value)
	{
		var enumerable = value as IEnumerable<object>;

		var array = enumerable as object[] ?? enumerable?.ToArray() ?? [];
		if (enumerable is null || !array.Any())
			return null;

		var objects = array.ToList();

		var hasDuplicates = CheckIfListHasDuplicates(objects);

		if (hasDuplicates)
			return ErrorResult();

		return null;
	}

	private bool CheckIfListHasDuplicates(List<object> enumerable)
	{
		var values = GetPropertyValues(enumerable);

		var array = values as object[] ?? values.ToArray();
		var hasDuplicates = array.GroupBy(v => v).Count() != array.Length;
		return hasDuplicates;
	}

	private string ErrorResult()
	{
		if (errorMessage != null)
			return errorMessage;

		return $"Non-unique values found for {propertyName}";
	}

	private IEnumerable<object?> GetPropertyValues(List<object> enumerable)
	{
		var objectType = GetListType(enumerable);
		var propertyType = GetPropertyType(objectType);

		var values = enumerable.Select(obj => GetValueForProperty(obj, propertyType));
		return values;
	}

	private object? GetValueForProperty(object obj, PropertyInfo propertyValue)
	{
		return propertyValue.GetValue(obj);
	}

	private PropertyInfo GetPropertyType(Type objectType)
	{
		var property = objectType.GetProperty(propertyName);

		if (property is null)
			throw new InvalidOperationException($"Property Name {propertyName} is invalid!");

		return property;
	}

	private Type GetListType(IEnumerable<object> list)
	{
		var first = list.First();

		return first.GetType();
	}
}