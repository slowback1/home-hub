using System.Net;
using System.Net.Http.Json;
using Common.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace WebAPI.Integration.Tests.Controllers;

[TestFixture]
public class NbaScheduleFeatureFlagTests
{
    [Test]
    public async Task FeatureFlags_IncludeNbaScheduleFlag_DisabledByDefault()
    {
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.UseEnvironment("Test"));
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/feature-flags");
        var flags = await response.Content.ReadFromJsonAsync<List<FeatureFlag>>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var flag = flags!.SingleOrDefault(f => f.Name == "NBA_SCHEDULE_ENABLED");
        Assert.That(flag, Is.Not.Null);
        Assert.That(flag!.IsEnabled, Is.False);
    }
}
