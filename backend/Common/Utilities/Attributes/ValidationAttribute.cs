using System;

namespace Common.Utilities.Attributes;

public abstract class ValidationAttribute : Attribute
{
    public string PropertyName { get; set; } = string.Empty;

    public abstract string? CheckForValidationError(object? value);
}