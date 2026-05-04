using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace Logic.Weather;

public class MockWeatherProvider : IWeatherProvider
{
    public Task<WeatherConditions> GetCurrentAsync(string zipCode, string units)
    {
        return Task.FromResult(new WeatherConditions
        {
            Temperature = 72.0,
            ConditionLabel = "Sunny",
            HumidityPercent = 45,
            WindSpeed = 8.5,
            Units = units
        });
    }
}
