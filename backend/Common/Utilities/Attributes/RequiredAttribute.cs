namespace Common.Utilities.Attributes;

public class RequiredAttribute(string? name = null) : ValidationAttribute
{
    public override string? CheckForValidationError(object? value)
    {
        if (value is null || IsEmptyString(value))
            return $"'{name ?? PropertyName}' is required.";

        return null;
    }

    private bool IsEmptyString(object? value)
    {
        if (value is null) return false;

        if (value.GetType() != typeof(string))
            return false;

        var stringValue = (string)value;
        return string.IsNullOrWhiteSpace(stringValue);
    }
}