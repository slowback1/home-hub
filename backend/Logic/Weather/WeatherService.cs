using System;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Logic.Weather;

public class WeatherService(
    IWeatherProvider provider,
    ISystemConfigProvider configProvider,
    IMemoryCache cache)
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

    public async Task<WeatherConditions> GetCurrentAsync()
    {
        var zipCode = (await configProvider.GetAsync("weather", "zip_code")).Value;
        var units   = (await configProvider.GetAsync("weather", "units")).Value;
        var cacheKey = $"weather:{zipCode}:{units}";

        if (cache.TryGetValue(cacheKey, out WeatherConditions? cached) && cached is not null)
            return cached;

        var result = await provider.GetCurrentAsync(zipCode, units);
        cache.Set(cacheKey, result, CacheTtl);
        return result;
    }
}
