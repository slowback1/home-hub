using System.Net;
using System.Net.Http.Json;
using Common.Models;
using InMemory;

namespace WebAPI.Integration.Tests.Controllers;

public class WheelControllerTests : ControllerTestBase
{
    private string _userId = string.Empty;

    [SetUp]
    public async Task ClearState()
    {
        InMemoryCrud<Wheel>.ClearStaticState();
        InMemoryCrud<AppUser>.ClearStaticState();
        _userId = await SeedTestUserAsync("Wheel Owner");
    }

    private Task<HttpResponseMessage> GetAsUser() =>
        SendAsUserAsync(HttpMethod.Get, "/api/wheels", null, _userId);

    private Task<HttpResponseMessage> PostAsUser(object body, string? userId = null) =>
        SendAsUserAsync(HttpMethod.Post, "/api/wheels", body, userId ?? _userId);

    private Task<HttpResponseMessage> PutAsUser(string id, object body, string? userId = null) =>
        SendAsUserAsync(HttpMethod.Put, $"/api/wheels/{id}", body, userId ?? _userId);

    private Task<HttpResponseMessage> DeleteAsUser(string id, string? userId = null) =>
        SendAsUserAsync(HttpMethod.Delete, $"/api/wheels/{id}", null, userId ?? _userId);

    private async Task<Wheel> CreateWheelAsync(string name, string items, string? userId = null)
    {
        var response = await PostAsUser(new { Name = name, Items = items }, userId);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<Wheel>())!;
    }

    [Test]
    public async Task Post_CreatesWheel_Returns200_WithIdAndFields()
    {
        var before = DateTime.UtcNow;
        var response = await PostAsUser(new { Name = "Dinner", Items = "Pizza\nTacos\nSushi" });
        var after = DateTime.UtcNow;
        var created = await response.Content.ReadFromJsonAsync<Wheel>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(created!.Id, Is.Not.Null.And.Not.Empty);
        Assert.That(created.Name, Is.EqualTo("Dinner"));
        Assert.That(created.Items, Is.EqualTo("Pizza\nTacos\nSushi"));
        Assert.That(created.CreatedAt, Is.InRange(before, after));
        Assert.That(created.UserId, Is.EqualTo(_userId));
    }

    [Test]
    public async Task Post_WithoutUserId_Returns401()
    {
        var response = await SendAsUserAsync(HttpMethod.Post, "/api/wheels", new { Name = "Dinner", Items = "Pizza" });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task Post_EmptyName_Returns400()
    {
        var response = await PostAsUser(new { Name = "", Items = "Pizza" });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Post_WhitespaceName_Returns400()
    {
        var response = await PostAsUser(new { Name = "   ", Items = "Pizza" });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Post_AllowsEmptyItems()
    {
        var response = await PostAsUser(new { Name = "Empty", Items = "" });
        var created = await response.Content.ReadFromJsonAsync<Wheel>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(created!.Items, Is.EqualTo(""));
    }

    [Test]
    public async Task Post_AllowsDuplicateNames()
    {
        await CreateWheelAsync("Dinner", "Pizza");
        var second = await PostAsUser(new { Name = "Dinner", Items = "Tacos" });

        Assert.That(second.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task Get_NoWheels_ReturnsEmptyArray()
    {
        var response = await GetAsUser();
        var wheels = await response.Content.ReadFromJsonAsync<Wheel[]>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(wheels, Is.Empty);
    }

    [Test]
    public async Task Get_ReturnsCreatedWheels()
    {
        await CreateWheelAsync("Dinner", "Pizza\nTacos");
        await CreateWheelAsync("Movies", "Alien\nHeat");

        var response = await GetAsUser();
        var wheels = await response.Content.ReadFromJsonAsync<Wheel[]>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(wheels, Has.Length.EqualTo(2));
    }

    [Test]
    public async Task Get_ReturnsOnlyRequestingUsersWheels()
    {
        var otherUserId = await SeedTestUserAsync("Other Owner");
        await CreateWheelAsync("Mine", "Pizza");
        await CreateWheelAsync("Theirs", "Alien", otherUserId);

        var wheels = await (await GetAsUser()).Content.ReadFromJsonAsync<Wheel[]>();

        Assert.That(wheels, Has.Length.EqualTo(1));
        Assert.That(wheels![0].Name, Is.EqualTo("Mine"));
    }

    [Test]
    public async Task Get_WithoutUserId_Returns401()
    {
        var response = await SendAsUserAsync(HttpMethod.Get, "/api/wheels");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task Put_UpdatesNameAndItems()
    {
        var created = await CreateWheelAsync("Dinner", "Pizza\nTacos");

        var response = await PutAsUser(created.Id,
            new { Name = "Dinner Options", Items = "Pizza\nTacos\nRamen" });
        var updated = await response.Content.ReadFromJsonAsync<Wheel>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(updated!.Name, Is.EqualTo("Dinner Options"));
        Assert.That(updated.Items, Is.EqualTo("Pizza\nTacos\nRamen"));
    }

    [Test]
    public async Task Put_UnknownId_Returns404()
    {
        var response = await PutAsUser("does-not-exist", new { Name = "Nope", Items = "x" });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Put_Returns404_ForOtherUsersWheel()
    {
        var otherUserId = await SeedTestUserAsync("Other Owner");
        var theirs = await CreateWheelAsync("Theirs", "Alien", otherUserId);

        var response = await PutAsUser(theirs.Id, new { Name = "Hijacked", Items = "x" });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Put_WithoutUserId_Returns401()
    {
        var response = await SendAsUserAsync(HttpMethod.Put, "/api/wheels/any-id", new { Name = "Nope", Items = "x" });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task Delete_RemovesWheel()
    {
        var created = await CreateWheelAsync("Dinner", "Pizza");

        var deleteResponse = await DeleteAsUser(created.Id);
        Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var wheels = await (await GetAsUser()).Content.ReadFromJsonAsync<Wheel[]>();
        Assert.That(wheels, Is.Empty);
    }

    [Test]
    public async Task Delete_UnknownId_Returns404()
    {
        var response = await DeleteAsUser("does-not-exist");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Delete_Returns404_ForOtherUsersWheel()
    {
        var otherUserId = await SeedTestUserAsync("Other Owner");
        var theirs = await CreateWheelAsync("Theirs", "Alien", otherUserId);

        var response = await DeleteAsUser(theirs.Id);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var theirWheels = await (await SendAsUserAsync(HttpMethod.Get, "/api/wheels", null, otherUserId))
            .Content.ReadFromJsonAsync<Wheel[]>();
        Assert.That(theirWheels!.Any(w => w.Id == theirs.Id), Is.True);
    }

    [Test]
    public async Task Delete_WithoutUserId_Returns401()
    {
        var response = await SendAsUserAsync(HttpMethod.Delete, "/api/wheels/any-id");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}
