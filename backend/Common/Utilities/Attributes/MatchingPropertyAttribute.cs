using System;
using System.Linq;

namespace Common.Utilities.Attributes;

public class MatchingPropertyAttribute(string propertyA, string propertyB, string? errorMessage = null) : ValidationAttribute
{
    public override string? CheckForValidationError(object? value)
    {
        if (propertyA == propertyB) throw new InvalidOperationException("Both properties are the same!");

        if (value is null) return null;

        var properties = value.GetType().GetProperties();

        var propA = properties.FirstOrDefault(p => p.Name == propertyA);
        var propB = properties.FirstOrDefault(p => p.Name == propertyB);

        if (propA is null || propB is null) throw new InvalidOperationException("Given invalid property name");

        var valueA = propA.GetValue(value);
        var valueB = propB.GetValue(value);

        var bothValuesAreNull = valueA == null && valueB == null;
        var bothValuesHaveTheSameNotNullValue = valueA != null && valueA.Equals(valueB);

        if (bothValuesAreNull || bothValuesHaveTheSameNotNullValue) return null;

        return errorMessage ?? $"'{propertyA}' must equal '{propertyB}'.";
    }
}