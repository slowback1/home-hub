using Common.Interfaces;
using TestUtilities;

namespace Common.Tests.Utilities.Time;

[TestFixture]
public class TestTimeProviderTests
{
    [SetUp]
    public void SetUp()
    {
        _fixedDateTime = new DateTime(2023, 10, 1, 12, 0, 0);
        _timeProvider = new TestTimeProvider(_fixedDateTime);
    }

    private ITimeProvider _timeProvider;
    private DateTime _fixedDateTime;

    [Test]
    public void TestNow()
    {
        var now = _timeProvider.Now();
        Assert.That(now, Is.EqualTo(_fixedDateTime));
    }

    [Test]
    public void TestToday()
    {
        var today = _timeProvider.Today();
        Assert.That(today, Is.EqualTo(_fixedDateTime.Date));
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
    public void TestUtcNow()
    {
        var utcNow = _timeProvider.UtcNow();
        Assert.That(utcNow, Is.EqualTo(_fixedDateTime.ToUniversalTime()));
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