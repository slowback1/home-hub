using Common.Utilities.Attributes;

namespace Common.Tests.Utilities.Attributes;

public class LengthAttributeTests
{
    [Test]
    public void ThrowsExceptionWhenGivenNonStringValue()
    {
        var attribute = new LengthAttribute(5);

        Assert.Throws<InvalidOperationException>(() =>
            attribute.CheckForValidationError(new LengthAttribute(5))
        );
    }

    [Test]
    public void ReturnsNullWhenGivenAStringWithinTheMaxLengthConstraints()
    {
        var attribute = new LengthAttribute(5);

        var result = attribute.CheckForValidationError("pugi");

        Assert.That(result, Is.Null);
    }

    [Test]
    public void ReturnsNullWhenGivenNullValue()
    {
        var attribute = new LengthAttribute(5);

        var result = attribute.CheckForValidationError(null);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void ReturnsErrorWhenStringIsBiggerThanMaxLength()
    {
        var attribute = new LengthAttribute(5);

        var result = attribute.CheckForValidationError("pugiiii");

        Assert.That(result, Is.EqualTo("'pugiiii' is longer than 5 characters."));
    }

    [Test]
    public void ReturnsErrorWhenStringIsSmallerThanMinLength()
    {
        var attribute = new LengthAttribute(5, 3);

        var result = attribute.CheckForValidationError("p");

        Assert.That(result, Is.EqualTo("'p' is shorter than 3 characters."));
    }

    [Test]
    public void CanValidateLengthOfAnEnumerable()
    {
        var attribute = new LengthAttribute(min: 1);

        var result = attribute.CheckForValidationError(new List<string> { "value" });

        Assert.That(result, Is.Null);
    }

    [Test]
    public void ValidatesWhenAListIsTooShort()
    {
        var attribute = new LengthAttribute(min: 3);

        var result = attribute.CheckForValidationError(new List<string> { "value", "value2" });

        Assert.That(result, Is.EqualTo("List ('value', 'value2') is shorter than 3 items."));
    }

    [Test]
    public void ValidatesWhenAListIsTooLong()
    {
        var attribute = new LengthAttribute(2);

        var result = attribute.CheckForValidationError(new List<string> { "value", "value2", "value3" });

        Assert.That(result, Is.EqualTo("List ('value', 'value2', 'value3') is longer than 2 items."));
    }
}