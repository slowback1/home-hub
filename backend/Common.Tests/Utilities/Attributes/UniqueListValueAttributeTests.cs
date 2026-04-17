using Common.Utilities.Attributes;

namespace Common.Tests.Utilities.Attributes;

public class UniqueListValueAttributeTests
{
    [Test]
    public void ReturnsNullForAZeroLengthList()
    {
        var attr = new UniqueListValueAttribute(nameof(TestUniqueListValue.BoolValue));

        var result = attr.CheckForValidationError(new List<TestUniqueListValue>());

        Assert.That(result, Is.Null);
    }

    [Test]
    [TestCase(nameof(TestUniqueListValue.BoolValue), false, "", 0, false, "", 0)]
    [TestCase(nameof(TestUniqueListValue.StringValue), false, "test", 0, false, "test", 1)]
    [TestCase(nameof(TestUniqueListValue.IntValue), false, "", 123, false, "", 123)]
    public void ReturnsAnErrorMessageWhenGivenTwoNonUniqueValues(string propertyName,
        bool firstB,
        string firstC,
        int firstD,
        bool secondB,
        string secondC,
        int secondD)
    {
        var list = new List<TestUniqueListValue>
        {
            new(firstB, firstC, firstD),
            new(secondB, secondC, secondD)
        };

        var attr = new UniqueListValueAttribute(propertyName);

        var result = attr.CheckForValidationError(list);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Contains.Substring(propertyName));
    }

    [Test]
    [TestCase(nameof(TestUniqueListValue.BoolValue), false, "", 0, true, "", 0)]
    [TestCase(nameof(TestUniqueListValue.StringValue), false, "test", 0, false, "test 2", 1)]
    [TestCase(nameof(TestUniqueListValue.IntValue), false, "", 123, false, "", 1234)]
    public void DoesNotReturnAnErrorMessageWhenAllOfTheValuesAreUnique(string propertyName,
        bool firstB,
        string firstC,
        int firstD,
        bool secondB,
        string secondC,
        int secondD)
    {
        var list = new List<TestUniqueListValue>
        {
            new(firstB, firstC, firstD),
            new(secondB, secondC, secondD)
        };

        var attr = new UniqueListValueAttribute(propertyName);

        var result = attr.CheckForValidationError(list);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void ReturnsNullIfNotGivenAnEnumerable()
    {
        var attr = new UniqueListValueAttribute("test");

        var result = attr.CheckForValidationError(32);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void ThrowsAnInvalidOperationExceptionWhenGivenAnInvalidPropetyName()
    {
        var attr = new UniqueListValueAttribute("invalid");

        var list = new List<TestUniqueListValue>
        {
            new(false, "1", 1)
        };

        Assert.Throws<InvalidOperationException>(() => attr.CheckForValidationError(list));
    }

    [Test]
    public void CanCustomizeTheErrorMessage()
    {
        var attr = new UniqueListValueAttribute(nameof(TestUniqueListValue.BoolValue), "YOU DONE MESSED UP!!!");

        var list = new List<TestUniqueListValue>
        {
            new(false, "1", 1),
            new(false, "1", 1)
        };

        var result = attr.CheckForValidationError(list);

        Assert.That(result, Is.EqualTo("YOU DONE MESSED UP!!!"));
    }

    private class TestUniqueListValue(bool boolValue, string stringValue, int intValue)
    {
        public bool BoolValue { get; set; } = boolValue;
        public string StringValue { get; set; } = stringValue;
        public int IntValue { get; set; } = intValue;
    }
}