using System.Net;
using System.Net.Http.Json;
using Common.Models;
using InMemory;

namespace WebAPI.Integration.Tests.Controllers;

public class BookmarkControllerTests : ControllerTestBase
{
    private record CreateRequest(string? Url, string? Name, string? Description);
    private record UpdateRequest(string? Url, string? Name, string? Description);

    [SetUp]
    public void ClearState()
    {
        InMemoryCrud<Bookmark>.ClearStaticState();
    }

    // --- GET /api/bookmarks ---

    [Test]
    public async Task List_ReturnsEmptyList_WhenNoneExist()
    {
        var response = await GetRawAsync("/api/bookmarks", includeToken: false);
        var items = await response.Content.ReadFromJsonAsync<List<Bookmark>>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(items, Is.Empty);
    }

    [Test]
    public async Task List_ReturnsAllBookmarks()
    {
        await PostRawAsync("/api/bookmarks", new CreateRequest("https://github.com", "GitHub", null));
        await PostRawAsync("/api/bookmarks", new CreateRequest("https://example.com", "Example", null));

        var response = await GetRawAsync("/api/bookmarks", includeToken: false);
        var items = await response.Content.ReadFromJsonAsync<List<Bookmark>>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(items!.Count, Is.EqualTo(2));
    }

    // --- POST /api/bookmarks ---

    [Test]
    public async Task Create_ReturnsBookmark_WithCorrectFields()
    {
        var response = await PostRawAsync("/api/bookmarks",
            new CreateRequest("https://github.com", "GitHub", "Code hosting."));
        var created = await response.Content.ReadFromJsonAsync<Bookmark>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(created!.Url, Is.EqualTo("https://github.com"));
        Assert.That(created.Name, Is.EqualTo("GitHub"));
        Assert.That(created.Description, Is.EqualTo("Code hosting."));
        Assert.That(created.Starred, Is.False);
        Assert.That(created.Id, Is.Not.Empty);
    }

    [Test]
    public async Task Create_Returns400_WhenUrlIsEmpty()
    {
        var response = await PostRawAsync("/api/bookmarks",
            new CreateRequest("", "GitHub", null));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Create_Returns400_WhenUrlIsNull()
    {
        var response = await PostRawAsync("/api/bookmarks",
            new CreateRequest(null, "GitHub", null));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Create_AcceptsNullDescription()
    {
        var response = await PostRawAsync("/api/bookmarks",
            new CreateRequest("https://example.com", "Example", null));
        var created = await response.Content.ReadFromJsonAsync<Bookmark>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(created!.Description, Is.Null);
    }

    // --- PUT /api/bookmarks/{id} ---

    [Test]
    public async Task Update_ChangesNameUrlAndDescription()
    {
        var created = await PostAsync<CreateRequest, Bookmark>("/api/bookmarks",
            new CreateRequest("https://github.com", "GitHub", null));

        var response = await PutRawAsync($"/api/bookmarks/{created!.Id}",
            new UpdateRequest("https://gitlab.com", "GitLab", "Alternative."));
        var updated = await response.Content.ReadFromJsonAsync<Bookmark>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(updated!.Name, Is.EqualTo("GitLab"));
        Assert.That(updated.Url, Is.EqualTo("https://gitlab.com"));
        Assert.That(updated.Description, Is.EqualTo("Alternative."));
    }

    [Test]
    public async Task Update_Returns404_WhenNotFound()
    {
        var response = await PutRawAsync("/api/bookmarks/nonexistent",
            new UpdateRequest("https://example.com", "Example", null));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    // --- DELETE /api/bookmarks/{id} ---

    [Test]
    public async Task Delete_Returns204_AndBookmarkNoLongerReturned()
    {
        var created = await PostAsync<CreateRequest, Bookmark>("/api/bookmarks",
            new CreateRequest("https://github.com", "GitHub", null));

        var deleteResponse = await DeleteRawAsync($"/api/bookmarks/{created!.Id}");
        Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var listResponse = await GetRawAsync("/api/bookmarks", includeToken: false);
        var items = await listResponse.Content.ReadFromJsonAsync<List<Bookmark>>();
        Assert.That(items!.Any(b => b.Id == created.Id), Is.False);
    }

    [Test]
    public async Task Delete_Returns404_WhenNotFound()
    {
        var response = await DeleteRawAsync("/api/bookmarks/nonexistent");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    // --- PATCH /api/bookmarks/{id}/star ---

    [Test]
    public async Task ToggleStar_TogglesStarredFromFalseToTrue()
    {
        var created = await PostAsync<CreateRequest, Bookmark>("/api/bookmarks",
            new CreateRequest("https://github.com", "GitHub", null));
        Assert.That(created!.Starred, Is.False);

        var response = await PatchRawAsync($"/api/bookmarks/{created.Id}/star");
        var updated = await response.Content.ReadFromJsonAsync<Bookmark>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(updated!.Starred, Is.True);
    }

    [Test]
    public async Task ToggleStar_TogglesStarredFromTrueToFalse()
    {
        var created = await PostAsync<CreateRequest, Bookmark>("/api/bookmarks",
            new CreateRequest("https://github.com", "GitHub", null));

        await PatchRawAsync($"/api/bookmarks/{created!.Id}/star");
        var response = await PatchRawAsync($"/api/bookmarks/{created.Id}/star");
        var updated = await response.Content.ReadFromJsonAsync<Bookmark>();

        Assert.That(updated!.Starred, Is.False);
    }

    [Test]
    public async Task ToggleStar_Returns404_WhenNotFound()
    {
        var response = await PatchRawAsync("/api/bookmarks/nonexistent/star");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    // --- DELETE /api/test/bookmarks ---

    [Test]
    public async Task TestHelper_ClearBookmarks_DeletesAll()
    {
        await PostRawAsync("/api/bookmarks", new CreateRequest("https://github.com", "GitHub", null));
        await PostRawAsync("/api/bookmarks", new CreateRequest("https://example.com", "Example", null));

        var clearResponse = await DeleteRawAsync("/api/test/bookmarks");
        Assert.That(clearResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var listResponse = await GetRawAsync("/api/bookmarks", includeToken: false);
        var items = await listResponse.Content.ReadFromJsonAsync<List<Bookmark>>();
        Assert.That(items, Is.Empty);
    }
}
