using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace Logic.Weather;

public class OpenWeatherMapProvider(HttpClient httpClient, ISystemConfigProvider configProvider)
    : IWeatherProvider
{
    private const string BaseUrl = "https://api.openweathermap.org/data/2.5/weather";

    public async Task<WeatherConditions> GetCurrentAsync(string zipCode, string units)
    {
        var apiKeyEntry = await configProvider.GetSecretAsync("weather", "api_key");
        if (string.IsNullOrWhiteSpace(apiKeyEntry.Value))
            throw new InvalidOperationException("OpenWeatherMap API key is not configured.");

        var url = $"{BaseUrl}?zip={Uri.EscapeDataString(zipCode)}&units={units}&appid={apiKeyEntry.Value}";
        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var owm = JsonSerializer.Deserialize<OwmResponse>(json)
            ?? throw new InvalidOperationException("Empty response from OpenWeatherMap.");

        return new WeatherConditions
        {
            Temperature = owm.Main.Temp,
            ConditionLabel = owm.Weather[0].Description,
            HumidityPercent = owm.Main.Humidity,
            WindSpeed = owm.Wind.Speed,
            Units = units
        };
    }

    private sealed class OwmResponse
    {
        [JsonPropertyName("main")] public OwmMain Main { get; set; } = new();
        [JsonPropertyName("weather")] public OwmWeather[] Weather { get; set; } = [];
        [JsonPropertyName("wind")] public OwmWind Wind { get; set; } = new();
    }

    private sealed class OwmMain
    {
        [JsonPropertyName("temp")] public double Temp { get; set; }
        [JsonPropertyName("humidity")] public int Humidity { get; set; }
    }

    private sealed class OwmWeather
    {
        [JsonPropertyName("description")] public string Description { get; set; } = string.Empty;
    }

    private sealed class OwmWind
    {
        [JsonPropertyName("speed")] public double Speed { get; set; }
    }
}
