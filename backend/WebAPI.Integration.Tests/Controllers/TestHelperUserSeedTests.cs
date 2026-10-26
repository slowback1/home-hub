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
}
