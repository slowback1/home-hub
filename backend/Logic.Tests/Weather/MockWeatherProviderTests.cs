using Logic.Weather;

namespace Logic.Tests.Weather;

public class MockWeatherProviderTests
{
    [Test]
    public async Task GetCurrentAsync_Imperial_ReturnsDeterministicConditions()
    {
        var provider = new MockWeatherProvider();

        var result = await provider.GetCurrentAsync("10001", "imperial");

        Assert.That(result.Temperature, Is.EqualTo(72.0));
        Assert.That(result.ConditionLabel, Is.EqualTo("Sunny"));
        Assert.That(result.HumidityPercent, Is.EqualTo(45));
        Assert.That(result.WindSpeed, Is.EqualTo(8.5));
        Assert.That(result.Units, Is.EqualTo("imperial"));
    }

    [Test]
    public async Task GetCurrentAsync_Metric_ReturnsMetricUnits()
    {
        var provider = new MockWeatherProvider();

        var result = await provider.GetCurrentAsync("10001", "metric");

        Assert.That(result.Units, Is.EqualTo("metric"));
        Assert.That(result.ConditionLabel, Is.EqualTo("Sunny"));
    }
}
