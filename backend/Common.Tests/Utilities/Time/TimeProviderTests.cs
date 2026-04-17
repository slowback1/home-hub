using Common.Interfaces;
using TimeProvider = Common.Utilities.Time.TimeProvider;

namespace Common.Tests.Utilities.Time;

[TestFixture]
public class TimeProviderTests
{
    [SetUp]
    public void SetUp()
    {
        _timeProvider = new TimeProvider();
    }

    private ITimeProvider _timeProvider;

    [Test]
    public void TestNow()
    {
        var now = _timeProvider.Now();
        Assert.That(now, Is.EqualTo(DateTime.Now).Within(TimeSpan.FromSeconds(1)));
    }

    [Test]
    public void TestToday()
    {
        var today = _timeProvider.Today();
        Assert.That(today, Is.EqualTo(DateTime.Today));
    }

    [Test]
    public void TestFromString()
    {
        var dateString = "2023-10-01";
        var expectedDate = new DateTime(2023, 10, 1);
        var parsedDate = _timeProvider.FromString(dateString);
        Assert.That(parsedDate, Is.EqualTo(expectedDate));
    }

    [Test]
    public void TestFromString_EmptyString()
    {
        Assert.Throws<FormatException>(() => _timeProvider.FromString(""));
    }

    [Test]
    public void TestFromString_NullString()
    {
        Assert.Throws<ArgumentNullException>(() => _timeProvider.FromString(null!));
    }

    [Test]
    public void TestFromString_InvalidFormat()
    {
        Assert.Throws<FormatException>(() => _timeProvider.FromString("invalid-date"));
    }

    [Test]
    public void TestFromString_ValidDateWithTime()
    {
        var dateString = "2023-10-01T12:30:45";
        var expectedDate = new DateTime(2023, 10, 1, 12, 30, 45);
        var parsedDate = _timeProvider.FromString(dateString);
        Assert.That(parsedDate, Is.EqualTo(expectedDate));
    }

    [Test]
    public void TestFromString_ValidDateDifferentFormat()
    {
        var dateString = "01/10/2023";
        var expectedDate = new DateTime(2023, 1, 10);
        var parsedDate = _timeProvider.FromString(dateString);
        Assert.That(parsedDate, Is.EqualTo(expectedDate));
    }

    [Test]
    public void TestUtcNow()
    {
        var utcNow = _timeProvider.UtcNow();
        Assert.That(utcNow, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(1)));
    }

    [Test]
    public void TestAddDays()
    {
        var date = new DateTime(2023, 10, 1);
        var result = _timeProvider.AddDays(date, 5);
        Assert.That(result, Is.EqualTo(new DateTime(2023, 10, 6)));
    }

    [Test]
    public void TestAddHours()
    {
        var date = new DateTime(2023, 10, 1, 12, 0, 0);
        var result = _timeProvider.AddHours(date, 5);
        Assert.That(result, Is.EqualTo(new DateTime(2023, 10, 1, 17, 0, 0)));
    }

    [Test]
    public void TestAddMinutes()
    {
        var date = new DateTime(2023, 10, 1, 12, 0, 0);
        var result = _timeProvider.AddMinutes(date, 30);
        Assert.That(result, Is.EqualTo(new DateTime(2023, 10, 1, 12, 30, 0)));
    }

    [Test]
    public void TestAddSeconds()
    {
        var date = new DateTime(2023, 10, 1, 12, 0, 0);
        var result = _timeProvider.AddSeconds(date, 45);
        Assert.That(result, Is.EqualTo(new DateTime(2023, 10, 1, 12, 0, 45)));
    }
}