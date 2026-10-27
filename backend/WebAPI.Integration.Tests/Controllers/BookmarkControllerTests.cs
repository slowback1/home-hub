using System.Net;
using System.Net.Http.Json;
using Common.Models;
using InMemory;

namespace WebAPI.Integration.Tests.Controllers;

public class BookmarkControllerTests : ControllerTestBase
{
    private string _userId = string.Empty;

    [SetUp]
    public async Task ClearState()
    {
        InMemoryCrud<Bookmark>.ClearStaticState();
        InMemoryCrud<AppUser>.ClearStaticState();
        _userId = await SeedTestUserAsync("Bookmark Owner");
    }

    private Task<HttpResponseMessage> GetAsUser() =>
        SendAsUserAsync(HttpMethod.Get, "/api/bookmarks", null, _userId);

    private Task<HttpResponseMessage> PostAsUser(object body, string? userId = null) =>
        SendAsUserAsync(HttpMethod.Post, "/api/bookmarks", body, userId ?? _userId);

    private Task<HttpResponseMessage> PutAsUser(string id, object body, string? userId = null) =>
        SendAsUserAsync(HttpMethod.Put, $"/api/bookmarks/{id}", body, userId ?? _userId);

    private Task<HttpResponseMessage> DeleteAsUser(string id, string? userId = null) =>
        SendAsUserAsync(HttpMethod.Delete, $"/api/bookmarks/{id}", null, userId ?? _userId);

    private Task<HttpResponseMessage> StarAsUser(string id, string? userId = null) =>
        SendAsUserAsync(HttpMethod.Patch, $"/api/bookmarks/{id}/star", null, userId ?? _userId);

    private async Task<Bookmark> CreateBookmark(string url, string name, string? userId = null)
    {
        var response = await PostAsUser(new { Url = url, Name = name, Description = (string?)null }, userId);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<Bookmark>())!;
    }

    // --- GET /api/bookmarks ---

    [Test]
    public async Task List_ReturnsEmptyList_WhenNoneExist()
    {
        var response = await GetAsUser();
        var items = await response.Content.ReadFromJsonAsync<List<Bookmark>>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(items, Is.Empty);
    }

    [Test]
    public async Task List_ReturnsUsersBookmarks()
    {
        await CreateBookmark("https://github.com", "GitHub");
        await CreateBookmark("https://example.com", "Example");

        var response = await GetAsUser();
        var items = await response.Content.ReadFromJsonAsync<List<Bookmark>>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(items!.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task List_ReturnsOnlyRequestingUsersBookmarks()
    {
        var otherUserId = await SeedTestUserAsync("Other Owner");
        await CreateBookmark("https://github.com", "Mine");
        await CreateBookmark("https://example.com", "Theirs", otherUserId);

        var items = await (await GetAsUser()).Content.ReadFromJsonAsync<List<Bookmark>>();

        Assert.That(items!.Count, Is.EqualTo(1));
        Assert.That(items[0].Name, Is.EqualTo("Mine"));
    }

    [Test]
    public async Task List_WithoutUserId_Returns401()
    {
        var response = await SendAsUserAsync(HttpMethod.Get, "/api/bookmarks");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    // --- POST /api/bookmarks ---

    [Test]
    public async Task Create_ReturnsBookmark_WithCorrectFields()
    {
        var response = await PostAsUser(new { Url = "https://github.com", Name = "GitHub", Description = "Code hosting." });
        var created = await response.Content.ReadFromJsonAsync<Bookmark>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(created!.Url, Is.EqualTo("https://github.com"));
        Assert.That(created.Name, Is.EqualTo("GitHub"));
        Assert.That(created.Description, Is.EqualTo("Code hosting."));
        Assert.That(created.Starred, Is.False);
        Assert.That(created.Id, Is.Not.Empty);
        Assert.That(created.UserId, Is.EqualTo(_userId));
    }

    [Test]
    public async Task Create_Returns400_WhenUrlIsEmpty()
    {
        var response = await PostAsUser(new { Url = "", Name = "GitHub", Description = (string?)null });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Create_Returns400_WhenUrlIsNull()
    {
        var response = await PostAsUser(new { Url = (string?)null, Name = "GitHub", Description = (string?)null });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Create_AcceptsNullDescription()
    {
        var response = await PostAsUser(new { Url = "https://example.com", Name = "Example", Description = (string?)null });
        var created = await response.Content.ReadFromJsonAsync<Bookmark>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(created!.Description, Is.Null);
    }

    [Test]
    public async Task Create_WithoutUserId_Returns401()
    {
        var response = await SendAsUserAsync(HttpMethod.Post, "/api/bookmarks",
            new { Url = "https://github.com", Name = "GitHub", Description = (string?)null });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    // --- PUT /api/bookmarks/{id} ---

    [Test]
    public async Task Update_ChangesNameUrlAndDescription()
    {
        var created = await CreateBookmark("https://github.com", "GitHub");

        var response = await PutAsUser(created.Id,
            new { Url = "https://gitlab.com", Name = "GitLab", Description = "Alternative." });
        var updated = await response.Content.ReadFromJsonAsync<Bookmark>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(updated!.Name, Is.EqualTo("GitLab"));
        Assert.That(updated.Url, Is.EqualTo("https://gitlab.com"));
        Assert.That(updated.Description, Is.EqualTo("Alternative."));
    }

    [Test]
    public async Task Update_Returns404_WhenNotFound()
    {
        var response = await PutAsUser("nonexistent",
            new { Url = "https://example.com", Name = "Example", Description = (string?)null });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Update_Returns404_ForOtherUsersBookmark()
    {
        var otherUserId = await SeedTestUserAsync("Other Owner");
        var theirs = await CreateBookmark("https://example.com", "Theirs", otherUserId);

        var response = await PutAsUser(theirs.Id,
            new { Url = "https://evil.com", Name = "Hijacked", Description = (string?)null });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Update_WithoutUserId_Returns401()
    {
        var response = await SendAsUserAsync(HttpMethod.Put, "/api/bookmarks/any-id",
            new { Url = "https://example.com", Name = "Example", Description = (string?)null });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    // --- DELETE /api/bookmarks/{id} ---

    [Test]
    public async Task Delete_Returns204_AndBookmarkNoLongerReturned()
    {
        var created = await CreateBookmark("https://github.com", "GitHub");

        var deleteResponse = await DeleteAsUser(created.Id);
        Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var items = await (await GetAsUser()).Content.ReadFromJsonAsync<List<Bookmark>>();
        Assert.That(items!.Any(b => b.Id == created.Id), Is.False);
    }

    [Test]
    public async Task Delete_Returns404_WhenNotFound()
    {
        var response = await DeleteAsUser("nonexistent");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Delete_Returns404_ForOtherUsersBookmark()
    {
        var otherUserId = await SeedTestUserAsync("Other Owner");
        var theirs = await CreateBookmark("https://example.com", "Theirs", otherUserId);

        var response = await DeleteAsUser(theirs.Id);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var theirItems = await (await SendAsUserAsync(HttpMethod.Get, "/api/bookmarks", null, otherUserId))
            .Content.ReadFromJsonAsync<List<Bookmark>>();
        Assert.That(theirItems!.Any(b => b.Id == theirs.Id), Is.True);
    }

    [Test]
    public async Task Delete_WithoutUserId_Returns401()
    {
        var response = await SendAsUserAsync(HttpMethod.Delete, "/api/bookmarks/any-id");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    // --- PATCH /api/bookmarks/{id}/star ---

    [Test]
    public async Task ToggleStar_TogglesStarredFromFalseToTrue()
    {
        var created = await CreateBookmark("https://github.com", "GitHub");
        Assert.That(created.Starred, Is.False);

        var response = await StarAsUser(created.Id);
        var updated = await response.Content.ReadFromJsonAsync<Bookmark>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(updated!.Starred, Is.True);
    }

    [Test]
    public async Task ToggleStar_TogglesStarredFromTrueToFalse()
    {
        var created = await CreateBookmark("https://github.com", "GitHub");

        await StarAsUser(created.Id);
        var response = await StarAsUser(created.Id);
        var updated = await response.Content.ReadFromJsonAsync<Bookmark>();

        Assert.That(updated!.Starred, Is.False);
    }

    [Test]
    public async Task ToggleStar_Returns404_WhenNotFound()
    {
        var response = await StarAsUser("nonexistent");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task ToggleStar_Returns404_ForOtherUsersBookmark()
    {
        var otherUserId = await SeedTestUserAsync("Other Owner");
        var theirs = await CreateBookmark("https://example.com", "Theirs", otherUserId);

        var response = await StarAsUser(theirs.Id);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task ToggleStar_WithoutUserId_Returns401()
    {
        var response = await SendAsUserAsync(HttpMethod.Patch, "/api/bookmarks/any-id/star");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    // --- DELETE /api/test/bookmarks ---

    [Test]
    public async Task TestHelper_ClearBookmarks_DeletesAll()
    {
        await CreateBookmark("https://github.com", "GitHub");
        var otherUserId = await SeedTestUserAsync("Other Owner");
        await CreateBookmark("https://example.com", "Theirs", otherUserId);

        var clearResponse = await DeleteRawAsync("/api/test/bookmarks");
        Assert.That(clearResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var items = await (await GetAsUser()).Content.ReadFromJsonAsync<List<Bookmark>>();
        Assert.That(items, Is.Empty);
    }
}
