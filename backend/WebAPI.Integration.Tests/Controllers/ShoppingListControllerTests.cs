using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Common.Models;
using InMemory;

namespace WebAPI.Integration.Tests.Controllers;

public class ShoppingListControllerTests : ControllerTestBase
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower) }
    };

    private string _userId = string.Empty;

    [SetUp]
    public async Task ClearState()
    {
        InMemoryCrud<ShoppingList>.ClearStaticState();
        InMemoryCrud<ShoppingListItem>.ClearStaticState();
        InMemoryCrud<AppUser>.ClearStaticState();
        _userId = await SeedTestUserAsync("Shopping Owner");
    }

    private Task<HttpResponseMessage> GetAsUser() =>
        SendAsUserAsync(HttpMethod.Get, "/api/shopping-lists", null, _userId);

    [Test]
    public async Task List_ReturnsEmptyList_WhenNoneExist()
    {
        var response = await GetAsUser();
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var items = await response.Content.ReadFromJsonAsync<List<ShoppingList>>(JsonOptions);
        Assert.That(items, Is.Empty);
    }

    private Task<HttpResponseMessage> PostAsUser(object body, string? userId = null) =>
        SendAsUserAsync(HttpMethod.Post, "/api/shopping-lists", body, userId ?? _userId);

    private async Task<ShoppingList> CreateList(string name, string? userId = null)
    {
        var response = await PostAsUser(new { Name = name }, userId);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ShoppingList>(JsonOptions))!;
    }

    private Task<HttpResponseMessage> PostItemAsync(string listId, object body, string? userId = null) =>
        SendAsUserAsync(HttpMethod.Post, $"/api/shopping-lists/{listId}/items", body, userId ?? _userId);

    [Test]
    public async Task AddItem_ReturnsCreatedItem_WithCorrectFields()
    {
        var list = await CreateList("Groceries");

        var response = await PostItemAsync(list.Id, new { Text = "Milk" });
        var item = await response.Content.ReadFromJsonAsync<ShoppingListItem>(JsonOptions);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(item!.Text, Is.EqualTo("Milk"));
        Assert.That(item.IsChecked, Is.False);
        Assert.That(item.ListId, Is.EqualTo(list.Id));
        Assert.That(item.Id, Is.Not.Empty);
    }

    private record ShoppingListWithItems(string Id, string Name, ShoppingListItem[] Items);

    [Test]
    public async Task List_IncludesItemsInline_PerList()
    {
        var mine = await CreateList("Mine");
        var otherUserId = await SeedTestUserAsync("Other Owner");
        var theirs = await CreateList("Theirs", otherUserId);
        await PostItemAsync(mine.Id, new { Text = "Milk" });
        await PostItemAsync(mine.Id, new { Text = "Eggs" });
        await PostItemAsync(theirs.Id, new { Text = "Secret" }, otherUserId);

        var lists = await (await GetAsUser()).Content.ReadFromJsonAsync<List<ShoppingListWithItems>>(JsonOptions);

        Assert.That(lists!.Count, Is.EqualTo(1));
        Assert.That(lists[0].Name, Is.EqualTo("Mine"));
        Assert.That(lists[0].Items.Select(i => i.Text), Is.EquivalentTo(new[] { "Milk", "Eggs" }));
    }

    [Test]
    public async Task AddItem_Returns400_WhenTextIsEmpty()
    {
        var list = await CreateList("Groceries");
        var response = await PostItemAsync(list.Id, new { Text = "" });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task AddItem_Returns400_WhenTextIsNull()
    {
        var list = await CreateList("Groceries");
        var response = await PostItemAsync(list.Id, new { Text = (string?)null });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task AddItem_Returns404_WhenListBelongsToOtherUser()
    {
        var otherUserId = await SeedTestUserAsync("Other Owner");
        var theirs = await CreateList("Theirs", otherUserId);

        var response = await PostItemAsync(theirs.Id, new { Text = "Intruder" });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task AddItem_Returns404_WhenListDoesNotExist()
    {
        var response = await PostItemAsync("missing-list-id", new { Text = "Milk" });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task AddItem_WithoutUserId_Returns401()
    {
        var list = await CreateList("Groceries");
        var response = await SendAsUserAsync(
            HttpMethod.Post, $"/api/shopping-lists/{list.Id}/items", new { Text = "Milk" });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task Create_ReturnsList_WithCorrectFields()
    {
        var response = await PostAsUser(new { Name = "Groceries" });
        var created = await response.Content.ReadFromJsonAsync<ShoppingList>(JsonOptions);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(created!.Name, Is.EqualTo("Groceries"));
        Assert.That(created.Status, Is.EqualTo(ShoppingListStatus.Active));
        Assert.That(created.ArchivedAt, Is.Null);
        Assert.That(created.Id, Is.Not.Empty);
        Assert.That(created.UserId, Is.EqualTo(_userId));
    }

    [Test]
    public async Task Create_Returns400_WhenNameIsEmpty()
    {
        var response = await PostAsUser(new { Name = "" });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Create_Returns400_WhenNameIsNull()
    {
        var response = await PostAsUser(new { Name = (string?)null });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Create_WithoutUserId_Returns401()
    {
        var response = await SendAsUserAsync(HttpMethod.Post, "/api/shopping-lists", new { Name = "Groceries" });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task List_ReturnsOnlyRequestingUsersActiveLists()
    {
        var otherUserId = await SeedTestUserAsync("Other Owner");
        await CreateList("Mine");
        await CreateList("Theirs", otherUserId);

        var items = await (await GetAsUser()).Content.ReadFromJsonAsync<List<ShoppingList>>(JsonOptions);

        Assert.That(items!.Count, Is.EqualTo(1));
        Assert.That(items[0].Name, Is.EqualTo("Mine"));
    }

    [Test]
    public async Task List_WithoutUserId_Returns401()
    {
        var response = await SendAsUserAsync(HttpMethod.Get, "/api/shopping-lists");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    // --- DELETE /api/test/shopping-lists ---

    [Test]
    public async Task TestHelper_ClearShoppingLists_DeletesAll()
    {
        await CreateList("Mine");
        var otherUserId = await SeedTestUserAsync("Other Owner");
        await CreateList("Theirs", otherUserId);

        var clearResponse = await DeleteRawAsync("/api/test/shopping-lists");
        Assert.That(clearResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var items = await (await GetAsUser()).Content.ReadFromJsonAsync<List<ShoppingList>>(JsonOptions);
        Assert.That(items, Is.Empty);
    }
}
