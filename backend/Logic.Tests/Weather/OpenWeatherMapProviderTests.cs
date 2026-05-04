using System.Net;
using System.Net.Http;
using System.Text;
using Common.Interfaces;
using SC = Common.Models.SystemConfig;
using Logic.SystemConfig;
using Logic.Weather;

namespace Logic.Tests.Weather;

public class OpenWeatherMapProviderTests
{
    private static ISystemConfigProvider MakeConfigWithApiKey(string apiKey) =>
        new DictionarySystemConfigProvider([
            new SC
            {
                Id = "weather::api_key", Namespace = "weather", Key = "api_key",
                Value = apiKey, Type = "secret", IsSecret = true
            }
        ]);

    private static HttpClient MakeHttpClient(HttpStatusCode status, string body) =>
        new(new FakeHandler(status, body));

    private class FakeHandler(HttpStatusCode status, string body) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(status)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            });
    }

    private const string ValidResponse = """
        {
            "main": { "temp": 68.0, "humidity": 60 },
            "weather": [{ "description": "overcast clouds" }],
            "wind": { "speed": 5.2 }
        }
        """;

    [Test]
    public async Task GetCurrentAsync_MapsResponseToWeatherConditions()
    {
        var provider = new OpenWeatherMapProvider(
            MakeHttpClient(HttpStatusCode.OK, ValidResponse),
            MakeConfigWithApiKey("test-key"));

        var result = await provider.GetCurrentAsync("10001", "imperial");

        Assert.That(result.Temperature, Is.EqualTo(68.0));
        Assert.That(result.ConditionLabel, Is.EqualTo("overcast clouds"));
        Assert.That(result.HumidityPercent, Is.EqualTo(60));
        Assert.That(result.WindSpeed, Is.EqualTo(5.2));
        Assert.That(result.Units, Is.EqualTo("imperial"));
    }

    [Test]
    public void GetCurrentAsync_ThrowsOnNon2xxResponse()
    {
        var provider = new OpenWeatherMapProvider(
            MakeHttpClient(HttpStatusCode.Unauthorized, """{"message":"Invalid API key"}"""),
            MakeConfigWithApiKey("bad-key"));

        Assert.ThrowsAsync<HttpRequestException>(() =>
            provider.GetCurrentAsync("10001", "imperial"));
    }

    [Test]
    public void GetCurrentAsync_ThrowsWhenApiKeyIsEmpty()
    {
        var provider = new OpenWeatherMapProvider(
            MakeHttpClient(HttpStatusCode.OK, ValidResponse),
            MakeConfigWithApiKey(string.Empty));

        Assert.ThrowsAsync<InvalidOperationException>(() =>
            provider.GetCurrentAsync("10001", "imperial"));
    }
}
