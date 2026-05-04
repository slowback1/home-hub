using Common.Interfaces;
using Common.Models;
using Logic.SystemConfig;
using Logic.Weather;
using Microsoft.Extensions.Caching.Memory;
using SC = Common.Models.SystemConfig;

namespace Logic.Tests.Weather;

public class WeatherServiceTests
{
    private static ISystemConfigProvider MakeConfig(string zipCode = "10001", string units = "imperial") =>
        new DictionarySystemConfigProvider([
            new SC { Id = "weather::zip_code", Namespace = "weather", Key = "zip_code", Value = zipCode, Type = "text", IsSecret = false },
            new SC { Id = "weather::units",    Namespace = "weather", Key = "units",    Value = units,   Type = "select", IsSecret = false }
        ]);

    private static IMemoryCache MakeCache() =>
        new MemoryCache(new MemoryCacheOptions());

    private class StubProvider(WeatherConditions result) : IWeatherProvider
    {
        public int CallCount { get; private set; }
        public Task<WeatherConditions> GetCurrentAsync(string zipCode, string units)
        {
            CallCount++;
            return Task.FromResult(result);
        }
    }

    private class ThrowingProvider : IWeatherProvider
    {
        public Task<WeatherConditions> GetCurrentAsync(string zipCode, string units) =>
            throw new HttpRequestException("API unreachable");
    }

    private static WeatherConditions MakeConditions() => new()
    {
        Temperature = 72.0, ConditionLabel = "Sunny",
        HumidityPercent = 45, WindSpeed = 8.5, Units = "imperial"
    };

    [Test]
    public async Task GetCurrentAsync_CacheMiss_CallsProviderAndReturnsResult()
    {
        var stub = new StubProvider(MakeConditions());
        var service = new WeatherService(stub, MakeConfig(), MakeCache());

        var result = await service.GetCurrentAsync();

        Assert.That(result.Temperature, Is.EqualTo(72.0));
        Assert.That(stub.CallCount, Is.EqualTo(1));
    }

    [Test]
    public async Task GetCurrentAsync_CacheHit_DoesNotCallProviderAgain()
    {
        var stub = new StubProvider(MakeConditions());
        var service = new WeatherService(stub, MakeConfig(), MakeCache());

        await service.GetCurrentAsync();
        await service.GetCurrentAsync();

        Assert.That(stub.CallCount, Is.EqualTo(1));
    }

    [Test]
    public void GetCurrentAsync_PropagatesProviderException()
    {
        var service = new WeatherService(new ThrowingProvider(), MakeConfig(), MakeCache());

        Assert.ThrowsAsync<HttpRequestException>(() => service.GetCurrentAsync());
    }

    [Test]
    public async Task GetCurrentAsync_DifferentUnits_UsesSeparateCacheEntries()
    {
        var stub = new StubProvider(MakeConditions());
        var cache = MakeCache();
        var serviceImperial = new WeatherService(stub, MakeConfig(units: "imperial"), cache);
        var serviceMetric   = new WeatherService(stub, MakeConfig(units: "metric"), cache);

        await serviceImperial.GetCurrentAsync();
        await serviceMetric.GetCurrentAsync();

        Assert.That(stub.CallCount, Is.EqualTo(2));
    }
}
