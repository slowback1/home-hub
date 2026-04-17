using Common.Utilities.Attributes;

namespace Common.Tests.Utilities.Attributes;

public class MatchingPropertyTestClass
{
    public string A { get; set; } = string.Empty;
    public string B { get; set; } = string.Empty;
}

public class MatchingPropertyAttributeTests
{
    [Test]
    public void ReturnsAnErrorWhenPropertiesDoNotMatch()
    {
        var obj = new MatchingPropertyTestClass { A = "a", B = "b" };

        var attr = new MatchingPropertyAttribute(nameof(MatchingPropertyTestClass.A),
            nameof(MatchingPropertyTestClass.B));

        var result = attr.CheckForValidationError(obj);

        Assert.That(result, Is.EqualTo("'A' must equal 'B'."));
    }

    [Test]
    public void DoesNotReturnAnErrorWhenPropertiesMatch()
    {
        var obj = new MatchingPropertyTestClass { A = "a", B = "a" };

        var attr = new MatchingPropertyAttribute(nameof(MatchingPropertyTestClass.A),
            nameof(MatchingPropertyTestClass.B));

        var result = attr.CheckForValidationError(obj);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void ReturnsNullIfClassIsNull()
    {
        var attr = new MatchingPropertyAttribute(nameof(MatchingPropertyTestClass.A),
            nameof(MatchingPropertyTestClass.B));

        var result = attr.CheckForValidationError(null);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void ThrowsInvalidOperationExceptionIfGivenInvalidPropertyA()
    {
        var obj = new MatchingPropertyTestClass { A = "a", B = "a" };

        var attr = new MatchingPropertyAttribute("I DONT EXIST",
            nameof(MatchingPropertyTestClass.B));

        Assert.Throws<InvalidOperationException>(() => attr.CheckForValidationError(obj),
            "Given invalid property name");
    }

    [Test]
    public void ThrowsInvalidOperationExceptionIfGivenInvalidPropertyB()
    {
        var obj = new MatchingPropertyTestClass { A = "a", B = "a" };

        var attr = new MatchingPropertyAttribute(nameof(MatchingPropertyTestClass.A),
            "I DONT EXIST");

        Assert.Throws<InvalidOperationException>(() => attr.CheckForValidationError(obj),
            "Given invalid property name");
    }

    [Test]
    public void ThrowsInvalidOperationExceptionIfGivenTheSamePropertyTwice()
    {
        var obj = new MatchingPropertyTestClass { A = "a", B = "a" };

        var attr = new MatchingPropertyAttribute(nameof(MatchingPropertyTestClass.A),
            nameof(MatchingPropertyTestClass.A));

        Assert.Throws<InvalidOperationException>(() => attr.CheckForValidationError(obj),
            "Both properties are the same!");
    }

    [Test]
    public void CanCustomizeTheErrorMessage()
    {
        var obj = new MatchingPropertyTestClass { A = "a", B = "b" };

        var attr = new MatchingPropertyAttribute(nameof(MatchingPropertyTestClass.A),
            nameof(MatchingPropertyTestClass.B), "YOU DONE DID IT!");

        var result = attr.CheckForValidationError(obj);

        Assert.That(result, Is.EqualTo("YOU DONE DID IT!"));
    }
}