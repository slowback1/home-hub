using System.Net;
using System.Net.Http.Json;
using Common.Models;
using InMemory;

namespace WebAPI.Integration.Tests.Controllers;

public class TestHelperUserSeedTests : ControllerTestBase
{
    [SetUp]
    public void ClearState()
    {
        InMemoryCrud<AppUser>.ClearStaticState();
        InMemoryActivityPickRepository.ClearStaticState();
    }

    [Test]
    public async Task SeedUser_MakesUserVisibleViaUsersApi()
    {
        var seedResponse = await PostRawAsync("/api/test/users", new { Name = "Alex" }, includeToken: false);
        Assert.That(seedResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var response = await GetRawAsync("/api/users", includeToken: false);
        var users = await response.Content.ReadFromJsonAsync<List<AppUser>>();

        Assert.That(users!.Count, Is.EqualTo(1));
        Assert.That(users[0].Name, Is.EqualTo("Alex"));
    }

    [Test]
    public async Task ClearUsers_EmptiesUsersApi()
    {
        await PostRawAsync("/api/test/users", new { Name = "Alex" }, includeToken: false);
        await PostRawAsync("/api/test/users", new { Name = "Sam" }, includeToken: false);

        var clearResponse = await DeleteRawAsync("/api/test/users", includeToken: false);
        Assert.That(clearResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var response = await GetRawAsync("/api/users", includeToken: false);
        var users = await response.Content.ReadFromJsonAsync<List<AppUser>>();

        Assert.That(users, Is.Empty);
    }

    [Test]
    public async Task SeedActivityPick_StampsSuppliedUserId()
    {
        var userId = await SeedTestUserAsync("Pick Seeder");

        var seedResponse = await PostRawAsync("/api/test/activity-picks",
            new { ActivityName = "Seeded Pick", PickedAt = DateTime.UtcNow, UserId = userId },
            includeToken: false);
        Assert.That(seedResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var response = await GetRawAsUserAsync("/api/activity/current", userId);
        var pick = await response.Content.ReadFromJsonAsync<ActivityPick>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(pick!.ActivityName, Is.EqualTo("Seeded Pick"));
    }

    [Test]
    public async Task ClearActivityPicks_RemovesPicksForAllUsers()
    {
        var firstUserId = await SeedTestUserAsync("First Owner");
        var secondUserId = await SeedTestUserAsync("Second Owner");

        (await PostRawAsync("/api/test/activity-picks",
            new { ActivityName = "First Pick", PickedAt = DateTime.UtcNow, UserId = firstUserId },
            includeToken: false)).EnsureSuccessStatusCode();
        (await PostRawAsync("/api/test/activity-picks",
            new { ActivityName = "Second Pick", PickedAt = DateTime.UtcNow, UserId = secondUserId },
            includeToken: false)).EnsureSuccessStatusCode();

        var clearResponse = await DeleteRawAsync("/api/test/activity-picks", includeToken: false);
        Assert.That(clearResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var firstResponse = await GetRawAsUserAsync("/api/activity/current", firstUserId);
        var secondResponse = await GetRawAsUserAsync("/api/activity/current", secondUserId);

        Assert.That(firstResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        Assert.That(secondResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }
}
