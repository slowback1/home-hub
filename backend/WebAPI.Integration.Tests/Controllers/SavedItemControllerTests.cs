using System.Net;
using System.Net.Http.Json;
using Common.Models;
using InMemory;

namespace WebAPI.Integration.Tests.Controllers;

public class SavedItemControllerTests : ControllerTestBase
{
    private string _userId = string.Empty;

    [SetUp]
    public async Task ClearState()
    {
        InMemoryCrud<SavedItem>.ClearStaticState();
        InMemoryCrud<AppUser>.ClearStaticState();
        _userId = await SeedTestUserAsync("Saved Items Owner");
    }

    private Task<HttpResponseMessage> GetAsUser() =>
        SendAsUserAsync(HttpMethod.Get, "/api/saved-items", null, _userId);

    private Task<HttpResponseMessage> PostAsUser(object body, string? userId = null) =>
        SendAsUserAsync(HttpMethod.Post, "/api/saved-items", body, userId ?? _userId);

    private Task<HttpResponseMessage> PutAsUser(string id, object body, string? userId = null) =>
        SendAsUserAsync(HttpMethod.Put, $"/api/saved-items/{id}", body, userId ?? _userId);

    private Task<HttpResponseMessage> DeleteAsUser(string id, string? userId = null) =>
        SendAsUserAsync(HttpMethod.Delete, $"/api/saved-items/{id}", null, userId ?? _userId);

    private async Task<SavedItem> CreateSavedItemAsync(string name, string? userId = null)
    {
        var response = await PostAsUser(new { Name = name }, userId);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<SavedItem>())!;
    }

    [Test]
    public async Task Post_CreatesSavedItem_Returns200_WithIdAndFields()
    {
        var before = DateTime.UtcNow;
        var response = await PostAsUser(new { Name = "5lb sack of potatoes" });
        var after = DateTime.UtcNow;
        var created = await response.Content.ReadFromJsonAsync<SavedItem>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(created!.Id, Is.Not.Null.And.Not.Empty);
        Assert.That(created.Name, Is.EqualTo("5lb sack of potatoes"));
        Assert.That(created.CreatedAt, Is.InRange(before, after));
        Assert.That(created.UserId, Is.EqualTo(_userId));
    }

    [Test]
    public async Task Post_TrimsName()
    {
        var response = await PostAsUser(new { Name = "  Sourdough loaf  " });
        var created = await response.Content.ReadFromJsonAsync<SavedItem>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(created!.Name, Is.EqualTo("Sourdough loaf"));
    }

    [Test]
    public async Task Post_WithoutUserId_Returns401()
    {
        var response = await SendAsUserAsync(HttpMethod.Post, "/api/saved-items", new { Name = "Milk" });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task Post_EmptyName_Returns400()
    {
        var response = await PostAsUser(new { Name = "" });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Post_WhitespaceName_Returns400()
    {
        var response = await PostAsUser(new { Name = "   " });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Post_AllowsDuplicateNames()
    {
        await CreateSavedItemAsync("Milk");
        var second = await PostAsUser(new { Name = "Milk" });

        Assert.That(second.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task Get_NoSavedItems_ReturnsEmptyArray()
    {
        var response = await GetAsUser();
        var items = await response.Content.ReadFromJsonAsync<SavedItem[]>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(items, Is.Empty);
    }

    [Test]
    public async Task Get_ReturnsCreatedSavedItems()
    {
        await CreateSavedItemAsync("5lb sack of potatoes");
        await CreateSavedItemAsync("Sourdough loaf");

        var response = await GetAsUser();
        var items = await response.Content.ReadFromJsonAsync<SavedItem[]>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(items, Has.Length.EqualTo(2));
    }

    [Test]
    public async Task Get_ReturnsOnlyRequestingUsersSavedItems()
    {
        var otherUserId = await SeedTestUserAsync("Other Owner");
        await CreateSavedItemAsync("Mine");
        await CreateSavedItemAsync("Theirs", otherUserId);

        var items = await (await GetAsUser()).Content.ReadFromJsonAsync<SavedItem[]>();

        Assert.That(items, Has.Length.EqualTo(1));
        Assert.That(items![0].Name, Is.EqualTo("Mine"));
    }

    [Test]
    public async Task Get_WithoutUserId_Returns401()
    {
        var response = await SendAsUserAsync(HttpMethod.Get, "/api/saved-items");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task Put_UpdatesName()
    {
        var created = await CreateSavedItemAsync("5lb sack of potatoes");

        var response = await PutAsUser(created.Id, new { Name = "10lb sack of potatoes" });
        var updated = await response.Content.ReadFromJsonAsync<SavedItem>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(updated!.Name, Is.EqualTo("10lb sack of potatoes"));
    }

    [Test]
    public async Task Put_TrimsName()
    {
        var created = await CreateSavedItemAsync("Sourdough loaf");

        var response = await PutAsUser(created.Id, new { Name = "  Sourdough boule  " });
        var updated = await response.Content.ReadFromJsonAsync<SavedItem>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(updated!.Name, Is.EqualTo("Sourdough boule"));
    }

    [Test]
    public async Task Put_EmptyName_Returns400()
    {
        var created = await CreateSavedItemAsync("Milk");

        var response = await PutAsUser(created.Id, new { Name = "" });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Put_UnknownId_Returns404()
    {
        var response = await PutAsUser("does-not-exist", new { Name = "Nope" });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Put_Returns404_ForOtherUsersSavedItem()
    {
        var otherUserId = await SeedTestUserAsync("Other Owner");
        var theirs = await CreateSavedItemAsync("Theirs", otherUserId);

        var response = await PutAsUser(theirs.Id, new { Name = "Hijacked" });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Put_WithoutUserId_Returns401()
    {
        var response = await SendAsUserAsync(HttpMethod.Put, "/api/saved-items/any-id", new { Name = "Nope" });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task Delete_RemovesSavedItem()
    {
        var created = await CreateSavedItemAsync("5lb sack of potatoes");

        var deleteResponse = await DeleteAsUser(created.Id);
        Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var items = await (await GetAsUser()).Content.ReadFromJsonAsync<SavedItem[]>();
        Assert.That(items, Is.Empty);
    }

    [Test]
    public async Task Delete_UnknownId_Returns404()
    {
        var response = await DeleteAsUser("does-not-exist");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Delete_Returns404_ForOtherUsersSavedItem()
    {
        var otherUserId = await SeedTestUserAsync("Other Owner");
        var theirs = await CreateSavedItemAsync("Theirs", otherUserId);

        var response = await DeleteAsUser(theirs.Id);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var theirItems = await (await SendAsUserAsync(HttpMethod.Get, "/api/saved-items", null, otherUserId))
            .Content.ReadFromJsonAsync<SavedItem[]>();
        Assert.That(theirItems!.Any(s => s.Id == theirs.Id), Is.True);
    }

    [Test]
    public async Task Delete_WithoutUserId_Returns401()
    {
        var response = await SendAsUserAsync(HttpMethod.Delete, "/api/saved-items/any-id");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}
