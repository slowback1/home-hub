using System.Threading.Tasks;
using Common.Models;

namespace Common.Interfaces;

public interface IWeatherProvider
{
    Task<WeatherConditions> GetCurrentAsync(string zipCode, string units);
}
