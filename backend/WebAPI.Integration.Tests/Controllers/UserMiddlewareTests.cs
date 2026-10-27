using System.Net;
using Common.Models;
using InMemory;

namespace WebAPI.Integration.Tests.Controllers;

public class UserMiddlewareTests : ControllerTestBase
{
    [SetUp]
    public void ClearState()
    {
        InMemoryCrud<AppUser>.ClearStaticState();
    }

    [Test]
    public async Task UnknownUserIdHeader_Returns401()
    {
        var response = await GetRawAsUserAsync("/api/users", "bogus-user-id");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task ValidUserIdHeader_ReturnsOk()
    {
        var userId = await SeedTestUserAsync("Middleware User");

        var response = await GetRawAsUserAsync("/api/users", userId);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task MissingUserIdHeader_LeavesUnscopedEndpointsWorking()
    {
        var response = await GetRawAsync("/api/users", includeToken: false);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}
