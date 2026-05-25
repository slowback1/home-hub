using Common.Models;
using Logic.Recurrence;

namespace Logic.Tests.Recurrence;

[TestFixture]
public class IntervalDaysRecurrenceCalculatorTests
{
    private IntervalDaysRecurrenceCalculator _calculator = null!;

    [SetUp]
    public void SetUp()
    {
        _calculator = new IntervalDaysRecurrenceCalculator();
    }

    [Test]
    public void GetNextDoDate_ReturnsCompletionDatePlusInterval()
    {
        var task = new ChoreTask { IsRecurring = true, IntervalDays = 7 };
        var completedAt = new DateTime(2026, 5, 24, 14, 30, 0, DateTimeKind.Utc);

        var result = _calculator.GetNextDoDate(task, completedAt);

        Assert.That(result, Is.EqualTo(new DateTime(2026, 5, 31, 0, 0, 0, DateTimeKind.Unspecified)));
    }

    [Test]
    public void GetNextDoDate_WithOneDayInterval_ReturnsNextDay()
    {
        var task = new ChoreTask { IsRecurring = true, IntervalDays = 1 };
        var completedAt = new DateTime(2026, 5, 24, 0, 0, 0, DateTimeKind.Utc);

        var result = _calculator.GetNextDoDate(task, completedAt);

        Assert.That(result.Date, Is.EqualTo(new DateTime(2026, 5, 25)));
    }

    [Test]
    public void GetNextDoDate_StripsTimeComponent_ReturnsDateOnly()
    {
        var task = new ChoreTask { IsRecurring = true, IntervalDays = 7 };
        var completedAt = new DateTime(2026, 5, 24, 23, 59, 59, DateTimeKind.Utc);

        var result = _calculator.GetNextDoDate(task, completedAt);

        Assert.That(result.TimeOfDay, Is.EqualTo(TimeSpan.Zero));
    }

    [Test]
    public void GetNextDoDate_WhenIntervalDaysIsNull_ThrowsInvalidOperationException()
    {
        var task = new ChoreTask { IsRecurring = true, IntervalDays = null };

        Assert.Throws<InvalidOperationException>(() =>
            _calculator.GetNextDoDate(task, DateTime.UtcNow));
    }

    [Test]
    public void GetNextDoDate_WhenIntervalDaysIsZero_ThrowsInvalidOperationException()
    {
        var task = new ChoreTask { IsRecurring = true, IntervalDays = 0 };

        Assert.Throws<InvalidOperationException>(() =>
            _calculator.GetNextDoDate(task, DateTime.UtcNow));
    }
}
