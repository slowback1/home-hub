using Common.Utilities.Attributes;

namespace Common.Tests.Utilities.Attributes;

public class ValidGuidAttributeTests
{
    [Test]
    [TestCase(1)]
    [TestCase(1111111d)]
    [TestCase(4444231f)]
    public void ThrowsInvalidOperationExceptionIfGivenOnNonStringOrNullValue(object value)
    {
        Assert.Throws<InvalidOperationException>(() => new ValidGuidAttribute().CheckForValidationError(value));
    }

    [Test]
    public void ReturnsNullIfGivenNull()
    {
        Assert.That(new ValidGuidAttribute().CheckForValidationError(null), Is.Null);
    }

    [Test]
    public void ReturnsNullForAValidGuid()
    {
        var attr = new ValidGuidAttribute();

        var result = attr.CheckForValidationError(Guid.NewGuid().ToString());

        Assert.That(result, Is.Null);
    }

    [Test]
    public void ReturnsValidationErrorForPropertyForInvalidGuid()
    {
        var attr = new ValidGuidAttribute() { PropertyName = "Id" };

        var result = attr.CheckForValidationError("asdf");

        Assert.That(result, Is.EqualTo("'Id' is not a valid Globally Unique ID."));
    }
}