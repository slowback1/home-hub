using System.Net;
using System.Net.Http.Json;
using Common.Interfaces;
using Common.Models;
using Logic.SystemConfig;
using Logic.Weather;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using SC = Common.Models.SystemConfig;

namespace WebAPI.Integration.Tests.Controllers;

public class WeatherControllerTests
{
    private static ISystemConfigProvider MakeConfig() =>
        new DictionarySystemConfigProvider([
            new SC { Id = "weather::zip_code", Namespace = "weather", Key = "zip_code", Value = "10001", Type = "text", IsSecret = false },
            new SC { Id = "weather::units",    Namespace = "weather", Key = "units",    Value = "imperial", Type = "select", IsSecret = false },
            new SC { Id = "weather::api_key",  Namespace = "weather", Key = "api_key",  Value = "", Type = "secret", IsSecret = true }
        ]);

    private static HttpClient CreateClient(IWeatherProvider weatherProvider)
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                builder.ConfigureServices(services =>
                {
                    services.AddScoped<ISystemConfigProvider>(_ => MakeConfig());
                    services.AddSingleton(weatherProvider);
                    services.AddScoped<WeatherService>();
                    services.AddMemoryCache();
                });
            });
        return factory.CreateClient();
    }

    [Test]
    public async Task GetCurrent_Returns200WithConditions_WhenProviderSucceeds()
    {
        var client = CreateClient(new MockWeatherProvider());

        var response = await client.GetAsync("/api/weather/current");
        var conditions = await response.Content.ReadFromJsonAsync<WeatherConditions>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(conditions!.Temperature, Is.EqualTo(72.0));
        Assert.That(conditions.ConditionLabel, Is.EqualTo("Sunny"));
        Assert.That(conditions.Units, Is.EqualTo("imperial"));
    }

    [Test]
    public async Task GetCurrent_Returns503_WhenProviderThrows()
    {
        var client = CreateClient(new ThrowingWeatherProvider());

        var response = await client.GetAsync("/api/weather/current");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.ServiceUnavailable));
    }

    private class ThrowingWeatherProvider : IWeatherProvider
    {
        public Task<WeatherConditions> GetCurrentAsync(string zipCode, string units) =>
            throw new HttpRequestException("API unreachable");
    }
}
