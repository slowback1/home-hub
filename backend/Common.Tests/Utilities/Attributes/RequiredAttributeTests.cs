using Common.Utilities.Attributes;

namespace Common.Tests.Utilities.Attributes;

public class RequiredAttributeTests
{
    [Test]
    public void ReturnsAnErrorIfValueIsNull()
    {
        var attribute = new RequiredAttribute("name");

        var result = attribute.CheckForValidationError(null);

        Assert.That(result, Is.EqualTo("'name' is required."));
    }

    [Test]
    public void ErrorWillUsePropertyNameIfGivenNameIsNotGiven()
    {
        var attribute = new RequiredAttribute
        {
            PropertyName = "Property Name"
        };

        var result = attribute.CheckForValidationError(null);

        Assert.That(result, Is.EqualTo("'Property Name' is required."));
    }

    [Test]
    [TestCase(1)]
    [TestCase("test")]
    [TestCase(TestEnum.Value)]
    [TestCase(1.3333333d)]
    [TestCase(1.998f)]
    public void DoesNotContainAnErrorWhenHasValue(object value)
    {
        var attribute = new RequiredAttribute("name");

        var result = attribute.CheckForValidationError(value);

        Assert.That(result, Is.Null);
    }

    [Test]
    [TestCase("")]
    [TestCase("   ")]
    [TestCase(" ")]
    public void ContainsAnErrorIfGivenEmptyOrWhitespaceString(string value)
    {
        var attribute = new RequiredAttribute("name");

        var result = attribute.CheckForValidationError(value);

        Assert.That(result, Is.EqualTo("'name' is required."));
    }

    private enum TestEnum
    {
        Value
    }
}