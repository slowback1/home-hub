using System;

namespace Common.Utilities.Attributes;

public class ValidGuidAttribute : ValidationAttribute
{
    public override string? CheckForValidationError(object? value)
    {
        if (value is null) return null;

        if (value is string stringValue)
            return CheckStringForGuidValidity(stringValue);

        throw new InvalidOperationException($"Guid Validation called for a non-string value! ({PropertyName})");
    }

    private string? CheckStringForGuidValidity(string value)
    {
        var isValidGuid = Guid.TryParse(value, out _);

        return isValidGuid ? null : $"'{PropertyName}' is not a valid Globally Unique ID.";
    }
}