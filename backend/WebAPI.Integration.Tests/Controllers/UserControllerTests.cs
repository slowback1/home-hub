using System.Net;
using System.Net.Http.Json;
using Common.Models;
using InMemory;

namespace WebAPI.Integration.Tests.Controllers;

public class UserControllerTests : ControllerTestBase
{
    [SetUp]
    public void ClearState()
    {
        InMemoryCrud<AppUser>.ClearStaticState();
    }

    private async Task SeedUser(string id, string name)
    {
        var crud = new InMemoryGenericCrud<AppUser>();
        await crud.CreateAsync(new AppUser { Id = id, Name = name });
    }

    [Test]
    public async Task List_ReturnsSeededUsers()
    {
        await SeedUser("1", "Alex");
        await SeedUser("2", "Sam");

        var response = await GetRawAsync("/api/users", includeToken: false);
        var users = await response.Content.ReadFromJsonAsync<List<AppUser>>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(users!.Count, Is.EqualTo(2));
        Assert.That(users.Select(u => u.Name), Is.EquivalentTo(new[] { "Alex", "Sam" }));
    }

    [Test]
    public async Task List_ReturnsEmptyList_WhenNoUsersExist()
    {
        var response = await GetRawAsync("/api/users", includeToken: false);
        var users = await response.Content.ReadFromJsonAsync<List<AppUser>>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(users, Is.Empty);
    }

    [Test]
    public async Task List_DoesNotExposePassword()
    {
        var crud = new InMemoryGenericCrud<AppUser>();
        await crud.CreateAsync(new AppUser { Id = "1", Name = "Alex", Password = "secret-hash" });

        var response = await GetRawAsync("/api/users", includeToken: false);
        var body = await response.Content.ReadAsStringAsync();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(body, Does.Not.Contain("assword"));
    }

    [Test]
    public async Task Create_IsNotAvailable()
    {
        var response = await PostRawAsync("/api/users", new { Name = "Bob" }, includeToken: false);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound).Or.EqualTo(HttpStatusCode.MethodNotAllowed));
    }

    [Test]
    public async Task Update_IsNotAvailable()
    {
        var response = await PutRawAsync("/api/users/1", new { Name = "Bob" }, includeToken: false);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound).Or.EqualTo(HttpStatusCode.MethodNotAllowed));
    }

    [Test]
    public async Task Delete_IsNotAvailable()
    {
        var response = await DeleteRawAsync("/api/users/1", includeToken: false);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound).Or.EqualTo(HttpStatusCode.MethodNotAllowed));
    }
}
