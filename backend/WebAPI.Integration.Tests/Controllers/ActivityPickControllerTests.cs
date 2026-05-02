using System.Net;
using System.Net.Http.Json;
using Common.Models;
using InMemory;

namespace WebAPI.Integration.Tests.Controllers;

public class ActivityPickControllerTests : ControllerTestBase
{
    [SetUp]
    public void ClearPickState() => InMemoryActivityPickRepository.ClearStaticState();
    [Test]
    public async Task GetCurrent_Returns204_WhenNoPickExists()
    {
        var response = await GetRawAsync("/api/activity/current", includeToken: false);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task GetCurrent_ReturnsMostRecentPick_WhenPickExists()
    {
        await SeedPick("Play Chess", DateTime.UtcNow.AddMinutes(-5));
        await SeedPick("Do Chores", DateTime.UtcNow);

        var response = await GetRawAsync("/api/activity/current", includeToken: false);
        var pick = await response.Content.ReadFromJsonAsync<ActivityPick>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(pick!.ActivityName, Is.EqualTo("Do Chores"));
    }

    [Test]
    public async Task GetHistory_ReturnsPicksWithinLast7Days()
    {
        await SeedPick("Old Pick", DateTime.UtcNow.AddDays(-10));
        await SeedPick("Recent Pick", DateTime.UtcNow.AddDays(-3));

        var response = await GetRawAsync("/api/activity/history", includeToken: false);
        var picks = await response.Content.ReadFromJsonAsync<List<ActivityPick>>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(picks!.Any(p => p.ActivityName == "Recent Pick"), Is.True);
        Assert.That(picks.Any(p => p.ActivityName == "Old Pick"), Is.False);
    }

    [Test]
    public async Task GetHistory_ReturnsEmpty_WhenNoPicksInRange()
    {
        var response = await GetRawAsync("/api/activity/history", includeToken: false);
        var picks = await response.Content.ReadFromJsonAsync<List<ActivityPick>>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(picks, Is.Empty);
    }

    private async Task SeedPick(string name, DateTime pickedAt)
    {
        var response = await PostRawAsync("/api/test/activity-picks",
            new { ActivityName = name, PickedAt = pickedAt });
        response.EnsureSuccessStatusCode();
    }
}
