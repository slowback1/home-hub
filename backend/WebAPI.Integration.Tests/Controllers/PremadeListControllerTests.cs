using System.Net;
using System.Net.Http.Json;
using Common.Models;
using InMemory;
using WebAPI.Controllers;

namespace WebAPI.Integration.Tests.Controllers;

public class PremadeListControllerTests : ControllerTestBase
{
    private string _userId = string.Empty;

    [SetUp]
    public async Task ClearState()
    {
        InMemoryCrud<PremadeList>.ClearStaticState();
        InMemoryCrud<PremadeListItem>.ClearStaticState();
        InMemoryCrud<SavedItem>.ClearStaticState();
        InMemoryCrud<AppUser>.ClearStaticState();
        _userId = await SeedTestUserAsync("Premade Lists Owner");
    }

    private Task<HttpResponseMessage> GetAsUser() =>
        SendAsUserAsync(HttpMethod.Get, "/api/premade-lists", null, _userId);

    private Task<HttpResponseMessage> PostAsUser(object body, string? userId = null) =>
        SendAsUserAsync(HttpMethod.Post, "/api/premade-lists", body, userId ?? _userId);

    private Task<HttpResponseMessage> DeleteAsUser(string id, string? userId = null) =>
        SendAsUserAsync(HttpMethod.Delete, $"/api/premade-lists/{id}", null, userId ?? _userId);

    private async Task<PremadeList> CreatePremadeListAsync(string name, string? userId = null)
    {
        var response = await PostAsUser(new { Name = name }, userId);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<PremadeList>())!;
    }

    [Test]
    public async Task Post_CreatesPremadeList_Returns200_WithIdAndFields()
    {
        var before = DateTime.UtcNow;
        var response = await PostAsUser(new { Name = "Weekly groceries" });
        var after = DateTime.UtcNow;
        var created = await response.Content.ReadFromJsonAsync<PremadeList>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(created!.Id, Is.Not.Null.And.Not.Empty);
        Assert.That(created.Name, Is.EqualTo("Weekly groceries"));
        Assert.That(created.CreatedAt, Is.InRange(before, after));
        Assert.That(created.UserId, Is.EqualTo(_userId));
    }

    [Test]
    public async Task Post_TrimsName()
    {
        var response = await PostAsUser(new { Name = "  Weekly groceries  " });
        var created = await response.Content.ReadFromJsonAsync<PremadeList>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(created!.Name, Is.EqualTo("Weekly groceries"));
    }

    [Test]
    public async Task Post_WithoutUserId_Returns401()
    {
        var response = await SendAsUserAsync(HttpMethod.Post, "/api/premade-lists", new { Name = "Weekly" });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task Post_EmptyName_Returns400()
    {
        var response = await PostAsUser(new { Name = "" });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Get_NoPremadeLists_ReturnsEmptyArray()
    {
        var response = await GetAsUser();
        var lists = await response.Content.ReadFromJsonAsync<PremadeList[]>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(lists, Is.Empty);
    }

    [Test]
    public async Task Get_ReturnsCreatedPremadeLists()
    {
        await CreatePremadeListAsync("Weekly groceries");
        await CreatePremadeListAsync("Party supplies");

        var response = await GetAsUser();
        var lists = await response.Content.ReadFromJsonAsync<PremadeList[]>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(lists, Has.Length.EqualTo(2));
    }

    [Test]
    public async Task Get_ReturnsOnlyRequestingUsersPremadeLists()
    {
        var otherUserId = await SeedTestUserAsync("Other Owner");
        await CreatePremadeListAsync("Mine");
        await CreatePremadeListAsync("Theirs", otherUserId);

        var lists = await (await GetAsUser()).Content.ReadFromJsonAsync<PremadeList[]>();

        Assert.That(lists, Has.Length.EqualTo(1));
        Assert.That(lists![0].Name, Is.EqualTo("Mine"));
    }

    [Test]
    public async Task Get_WithoutUserId_Returns401()
    {
        var response = await SendAsUserAsync(HttpMethod.Get, "/api/premade-lists");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task Delete_RemovesPremadeList()
    {
        var created = await CreatePremadeListAsync("Weekly groceries");

        var deleteResponse = await DeleteAsUser(created.Id);
        Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var lists = await (await GetAsUser()).Content.ReadFromJsonAsync<PremadeList[]>();
        Assert.That(lists, Is.Empty);
    }

    [Test]
    public async Task Delete_UnknownId_Returns404()
    {
        var response = await DeleteAsUser("does-not-exist");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Delete_Returns404_ForOtherUsersPremadeList()
    {
        var otherUserId = await SeedTestUserAsync("Other Owner");
        var theirs = await CreatePremadeListAsync("Theirs", otherUserId);

        var response = await DeleteAsUser(theirs.Id);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var theirLists = await (await SendAsUserAsync(HttpMethod.Get, "/api/premade-lists", null, otherUserId))
            .Content.ReadFromJsonAsync<PremadeList[]>();
        Assert.That(theirLists!.Any(l => l.Id == theirs.Id), Is.True);
    }

    [Test]
    public async Task Delete_WithoutUserId_Returns401()
    {
        var response = await SendAsUserAsync(HttpMethod.Delete, "/api/premade-lists/any-id");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    // --- Membership endpoints ---

    private async Task<SavedItem> CreateSavedItemAsync(string name, string? userId = null)
    {
        var response = await SendAsUserAsync(
            HttpMethod.Post, "/api/saved-items", new { Name = name }, userId ?? _userId);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<SavedItem>())!;
    }

    private Task<HttpResponseMessage> PostMembershipAsync(string listId, string savedItemId, string? userId = null) =>
        SendAsUserAsync(
            HttpMethod.Post, $"/api/premade-lists/{listId}/items", new { SavedItemId = savedItemId }, userId ?? _userId);

    private Task<HttpResponseMessage> DeleteMembershipAsync(
        string listId, string savedItemId, string? userId = null) =>
        SendAsUserAsync(HttpMethod.Delete, $"/api/premade-lists/{listId}/items/{savedItemId}", null, userId ?? _userId);

    private async Task<string[]> GetSavedItemIdsAsync(string listId)
    {
        var lists = await (await GetAsUser()).Content.ReadFromJsonAsync<PremadeListController.PremadeListResponse[]>();
        return lists!.Single(l => l.Id == listId).SavedItemIds;
    }

    [Test]
    public async Task PostMembership_AddsSavedItem_AndGetReflectsItInline()
    {
        var list = await CreatePremadeListAsync("Weekly groceries");
        var saved = await CreateSavedItemAsync("Milk");

        var response = await PostMembershipAsync(list.Id, saved.Id);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var ids = await GetSavedItemIdsAsync(list.Id);
        Assert.That(ids, Is.EquivalentTo(new[] { saved.Id }));
    }

    [Test]
    public async Task PostMembership_UnknownSavedItem_Returns404()
    {
        var list = await CreatePremadeListAsync("Weekly groceries");

        var response = await PostMembershipAsync(list.Id, "does-not-exist");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task PostMembership_OtherUsersSavedItem_Returns404()
    {
        var otherUserId = await SeedTestUserAsync("Other Owner");
        var list = await CreatePremadeListAsync("Weekly groceries");
        var theirs = await CreateSavedItemAsync("Theirs", otherUserId);

        var response = await PostMembershipAsync(list.Id, theirs.Id);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task PostMembership_OtherUsersPremadeList_Returns404()
    {
        var otherUserId = await SeedTestUserAsync("Other Owner");
        var theirs = await CreatePremadeListAsync("Theirs", otherUserId);
        var saved = await CreateSavedItemAsync("Milk");

        var response = await PostMembershipAsync(theirs.Id, saved.Id);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task PostMembership_Duplicate_Returns409()
    {
        var list = await CreatePremadeListAsync("Weekly groceries");
        var saved = await CreateSavedItemAsync("Milk");

        await PostMembershipAsync(list.Id, saved.Id);
        var second = await PostMembershipAsync(list.Id, saved.Id);

        Assert.That(second.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));

        var ids = await GetSavedItemIdsAsync(list.Id);
        Assert.That(ids, Is.EquivalentTo(new[] { saved.Id }));
    }

    [Test]
    public async Task PostMembership_WithoutUserId_Returns401()
    {
        var list = await CreatePremadeListAsync("Weekly groceries");
        var saved = await CreateSavedItemAsync("Milk");

        var response = await SendAsUserAsync(
            HttpMethod.Post, $"/api/premade-lists/{list.Id}/items", new { SavedItemId = saved.Id });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task DeleteMembership_RemovesSavedItem_AndGetReflectsItInline()
    {
        var list = await CreatePremadeListAsync("Weekly groceries");
        var saved = await CreateSavedItemAsync("Milk");
        await PostMembershipAsync(list.Id, saved.Id);

        var response = await DeleteMembershipAsync(list.Id, saved.Id);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var ids = await GetSavedItemIdsAsync(list.Id);
        Assert.That(ids, Is.Empty);
    }

    [Test]
    public async Task DeleteMembership_NotPresent_Returns404()
    {
        var list = await CreatePremadeListAsync("Weekly groceries");
        var saved = await CreateSavedItemAsync("Milk");

        var response = await DeleteMembershipAsync(list.Id, saved.Id);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task DeleteMembership_OtherUsersPremadeList_Returns404()
    {
        var otherUserId = await SeedTestUserAsync("Other Owner");
        var theirs = await CreatePremadeListAsync("Theirs", otherUserId);
        var saved = await CreateSavedItemAsync("Milk", otherUserId);
        await PostMembershipAsync(theirs.Id, saved.Id, otherUserId);

        var response = await DeleteMembershipAsync(theirs.Id, saved.Id);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var theirIds = await (await SendAsUserAsync(
            HttpMethod.Get, "/api/premade-lists", null, otherUserId)).Content.ReadFromJsonAsync<PremadeListController.PremadeListResponse[]>();
        Assert.That(theirIds!.Single(l => l.Id == theirs.Id).SavedItemIds, Is.EquivalentTo(new[] { saved.Id }));
    }
}
