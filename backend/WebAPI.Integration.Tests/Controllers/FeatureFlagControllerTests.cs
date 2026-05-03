using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using Common.Interfaces;
using Common.Models;
using Logic.FeatureFlags;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace WebAPI.Integration.Tests.Controllers;

public class FeatureFlagControllerTests
{
    private static HttpClient CreateClient(Dictionary<string, bool> flags)
    {
        var store = new DictionaryFeatureFlagProvider(flags);
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton<IFeatureFlagProvider>(store);
                    services.AddSingleton<IFeatureFlagRepository>(store);
                });
            });
        return factory.CreateClient();
    }

    [Test]
    public async Task GetAll_ReturnsAllFlags()
    {
        var client = CreateClient(new Dictionary<string, bool> { ["DEMO_FEATURE_FLAG"] = false });

        var response = await client.GetAsync("/api/feature-flags");
        var flags = await response.Content.ReadFromJsonAsync<List<FeatureFlag>>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(flags!.Count, Is.EqualTo(1));
        Assert.That(flags[0].Name, Is.EqualTo("DEMO_FEATURE_FLAG"));
        Assert.That(flags[0].IsEnabled, Is.False);
    }

    [Test]
    public async Task GetAll_ReturnsEmptyArray_WhenNoFlagsExist()
    {
        var client = CreateClient(new Dictionary<string, bool>());

        var response = await client.GetAsync("/api/feature-flags");
        var flags = await response.Content.ReadFromJsonAsync<List<FeatureFlag>>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(flags, Is.Empty);
    }

    [Test]
    public async Task Patch_TogglesFlag_AndReturnsUpdatedFlag()
    {
        var client = CreateClient(new Dictionary<string, bool> { ["DEMO_FEATURE_FLAG"] = false });

        var response = await client.PatchAsJsonAsync(
            "/api/feature-flags/DEMO_FEATURE_FLAG",
            new { isEnabled = true });
        var updated = await response.Content.ReadFromJsonAsync<FeatureFlag>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(updated!.Name, Is.EqualTo("DEMO_FEATURE_FLAG"));
        Assert.That(updated.IsEnabled, Is.True);
    }

    [Test]
    public async Task Patch_Returns404_WhenFlagNotFound()
    {
        var client = CreateClient(new Dictionary<string, bool>());

        var response = await client.PatchAsJsonAsync(
            "/api/feature-flags/UNKNOWN_FLAG",
            new { isEnabled = true });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
