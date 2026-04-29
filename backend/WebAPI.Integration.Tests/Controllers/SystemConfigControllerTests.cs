using System.Net;
using System.Net.Http.Json;
using Common.Interfaces;
using Logic.SystemConfig;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using SC = Common.Models.SystemConfig;

namespace WebAPI.Integration.Tests.Controllers;

public class SystemConfigControllerTests
{
    private static SC MakeEntry(string @namespace, string key, string value, bool isSecret = false) =>
        new()
        {
            Id = $"{@namespace}::{key}",
            Namespace = @namespace,
            Key = key,
            Value = value,
            Type = "string",
            IsSecret = isSecret
        };

    private static HttpClient CreateClient(params SC[] seed)
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                builder.ConfigureServices(services =>
                {
                    services.AddScoped<ISystemConfigProvider>(_ =>
                        new DictionarySystemConfigProvider(seed));
                });
            });
        return factory.CreateClient();
    }

    [Test]
    public async Task GetAll_ReturnsAllEntries_WithSecretsMasked()
    {
        var client = CreateClient(
            MakeEntry("general", "site_name", "HomeHub"),
            MakeEntry("weather", "api_key", "secret-value", isSecret: true));

        var response = await client.GetAsync("/api/system-config");
        var entries = await response.Content.ReadFromJsonAsync<List<SC>>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(entries!.Count, Is.EqualTo(2));
        Assert.That(entries.First(e => e.Key == "site_name").Value, Is.EqualTo("HomeHub"));
        Assert.That(entries.First(e => e.Key == "api_key").Value, Is.EqualTo("***"));
    }

    [Test]
    public async Task GetOne_ReturnsEntry_WhenKeyExists()
    {
        var client = CreateClient(MakeEntry("general", "site_name", "HomeHub"));

        var response = await client.GetAsync("/api/system-config/general/site_name");
        var entry = await response.Content.ReadFromJsonAsync<SC>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(entry!.Value, Is.EqualTo("HomeHub"));
        Assert.That(entry.Namespace, Is.EqualTo("general"));
        Assert.That(entry.Key, Is.EqualTo("site_name"));
    }

    [Test]
    public async Task GetOne_Returns404_WhenKeyNotFound()
    {
        var client = CreateClient();

        var response = await client.GetAsync("/api/system-config/general/missing");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Put_UpdatesValue_AndReturnsUpdatedEntry()
    {
        var client = CreateClient(MakeEntry("general", "site_name", "OldName"));

        var response = await client.PutAsJsonAsync(
            "/api/system-config/general/site_name",
            new { value = "NewName" });
        var updated = await response.Content.ReadFromJsonAsync<SC>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(updated!.Value, Is.EqualTo("NewName"));
    }

    [Test]
    public async Task Put_Returns404_WhenKeyNotFound()
    {
        var client = CreateClient();

        var response = await client.PutAsJsonAsync(
            "/api/system-config/general/missing",
            new { value = "anything" });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
