using System.Net;
using System.Net.Http.Json;
using Common.Models;
using InMemory;

namespace WebAPI.Integration.Tests.Controllers;

public class ActivityPickControllerTests : ControllerTestBase
{
    private string _userId = string.Empty;

    [SetUp]
    public async Task ClearPickState()
    {
        InMemoryActivityPickRepository.ClearStaticState();
        InMemoryCrud<AppUser>.ClearStaticState();
        _userId = await SeedTestUserAsync("Pick Owner");
    }

    private Task<HttpResponseMessage> GetAsUser(string url) => GetRawAsUserAsync(url, _userId);

    [Test]
    public async Task GetCurrent_WithoutUserId_Returns401()
    {
        var response = await GetRawAsync("/api/activity/current", includeToken: false);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GetCurrent_WithUnknownUserId_Returns401()
    {
        var response = await GetRawAsUserAsync("/api/activity/current", "no-such-user-id");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GetCurrent_Returns204_WhenNoPickExists()
    {
        var response = await GetAsUser("/api/activity/current");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task GetCurrent_ReturnsMostRecentPick_WhenPickExists()
    {
        await SeedPick("Play Chess", DateTime.UtcNow.AddMinutes(-5), _userId);
        await SeedPick("Do Chores", DateTime.UtcNow, _userId);

        var response = await GetAsUser("/api/activity/current");
        var pick = await response.Content.ReadFromJsonAsync<ActivityPick>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(pick!.ActivityName, Is.EqualTo("Do Chores"));
    }

    [Test]
    public async Task GetCurrent_DoesNotReturnOtherUsersPick()
    {
        var otherUserId = await SeedTestUserAsync("Other Owner");
        await SeedPick("Theirs", DateTime.UtcNow, otherUserId);
        await SeedPick("Mine", DateTime.UtcNow.AddMinutes(-5), _userId);

        var response = await GetAsUser("/api/activity/current");
        var pick = await response.Content.ReadFromJsonAsync<ActivityPick>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(pick!.ActivityName, Is.EqualTo("Mine"));
    }

    [Test]
    public async Task GetCurrent_Returns204_WhenOnlyOtherUsersHavePicks()
    {
        var otherUserId = await SeedTestUserAsync("Other Owner");
        await SeedPick("Theirs", DateTime.UtcNow, otherUserId);

        var response = await GetAsUser("/api/activity/current");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task GetHistory_WithoutUserId_Returns401()
    {
        var response = await GetRawAsync("/api/activity/history", includeToken: false);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task GetHistory_ReturnsPicksWithinLast7Days()
    {
        await SeedPick("Old Pick", DateTime.UtcNow.AddDays(-10), _userId);
        await SeedPick("Recent Pick", DateTime.UtcNow.AddDays(-3), _userId);

        var response = await GetAsUser("/api/activity/history");
        var picks = await response.Content.ReadFromJsonAsync<List<ActivityPick>>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(picks!.Any(p => p.ActivityName == "Recent Pick"), Is.True);
        Assert.That(picks!.Any(p => p.ActivityName == "Old Pick"), Is.False);
    }

    [Test]
    public async Task GetHistory_ReturnsEmpty_WhenNoPicksInRange()
    {
        var response = await GetAsUser("/api/activity/history");
        var picks = await response.Content.ReadFromJsonAsync<List<ActivityPick>>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(picks, Is.Empty);
    }

    [Test]
    public async Task GetHistory_ReturnsOnlyRequestingUsersPicks()
    {
        var otherUserId = await SeedTestUserAsync("Other Owner");
        await SeedPick("Mine", DateTime.UtcNow.AddDays(-1), _userId);
        await SeedPick("Theirs", DateTime.UtcNow.AddDays(-1), otherUserId);

        var response = await GetAsUser("/api/activity/history");
        var picks = await response.Content.ReadFromJsonAsync<List<ActivityPick>>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(picks!.Count, Is.EqualTo(1));
        Assert.That(picks[0].ActivityName, Is.EqualTo("Mine"));
    }

    private async Task SeedPick(string name, DateTime pickedAt, string userId)
    {
        var response = await PostRawAsync("/api/test/activity-picks",
            new { ActivityName = name, PickedAt = pickedAt, UserId = userId }, includeToken: false);
        response.EnsureSuccessStatusCode();
    }
}
