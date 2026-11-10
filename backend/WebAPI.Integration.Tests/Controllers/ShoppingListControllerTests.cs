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
        InMemoryCrud<PremadeList>.ClearStaticState();
        InMemoryCrud<PremadeListItem>.ClearStaticState();
        InMemoryCrud<SavedItem>.ClearStaticState();
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

    private record ShoppingListWithItems(string Name, ShoppingListItem[] Items);

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

    private Task<HttpResponseMessage> PatchItemAsync(string listId, string itemId, string? userId = null) =>
        SendAsUserAsync(HttpMethod.Patch, $"/api/shopping-lists/{listId}/items/{itemId}", null, userId ?? _userId);

    private async Task<ShoppingListItem> AddItemAsync(string listId, string text, string? userId = null)
    {
        var response = await PostItemAsync(listId, new { Text = text }, userId);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ShoppingListItem>(JsonOptions))!;
    }

    [Test]
    public async Task ToggleItem_ChecksOffAndBackOn_ItemStaysInList()
    {
        var list = await CreateList("Groceries");
        var item = await AddItemAsync(list.Id, "Milk");

        var off = await PatchItemAsync(list.Id, item.Id);
        var offItem = await off.Content.ReadFromJsonAsync<ShoppingListItem>(JsonOptions);
        Assert.That(off.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(offItem!.IsChecked, Is.True);

        var on = await PatchItemAsync(list.Id, item.Id);
        var onItem = await on.Content.ReadFromJsonAsync<ShoppingListItem>(JsonOptions);
        Assert.That(on.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(onItem!.IsChecked, Is.False);

        var lists = await (await GetAsUser()).Content.ReadFromJsonAsync<List<ShoppingListWithItems>>(JsonOptions);
        Assert.That(lists![0].Items.Select(i => i.Text), Is.EquivalentTo(new[] { "Milk" }));
    }

    [Test]
    public async Task ToggleItem_Returns404_WhenListBelongsToOtherUser()
    {
        var otherUserId = await SeedTestUserAsync("Other Owner");
        var theirs = await CreateList("Theirs", otherUserId);
        var item = await AddItemAsync(theirs.Id, "Secret", otherUserId);

        var response = await PatchItemAsync(theirs.Id, item.Id);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task ToggleItem_Returns404_WhenItemDoesNotExist()
    {
        var list = await CreateList("Groceries");
        var response = await PatchItemAsync(list.Id, "missing-item-id");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task ToggleItem_Returns404_WhenListDoesNotExist()
    {
        var list = await CreateList("Groceries");
        var item = await AddItemAsync(list.Id, "Milk");
        var response = await PatchItemAsync("missing-list-id", item.Id);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task ToggleItem_WithoutUserId_Returns401()
    {
        var list = await CreateList("Groceries");
        var item = await AddItemAsync(list.Id, "Milk");
        var response = await SendAsUserAsync(
            HttpMethod.Patch, $"/api/shopping-lists/{list.Id}/items/{item.Id}");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    private Task<HttpResponseMessage> CompleteListAsync(string listId, string? userId = null) =>
        SendAsUserAsync(HttpMethod.Post, $"/api/shopping-lists/{listId}/complete", null, userId ?? _userId);

    private Task<HttpResponseMessage> GetArchivedAsUser() =>
        SendAsUserAsync(HttpMethod.Get, "/api/shopping-lists?status=archived", null, _userId);

    [Test]
    public async Task Complete_FlipsStatusToArchived_AndStampsArchivedAt()
    {
        var list = await CreateList("Groceries");
        await AddItemAsync(list.Id, "Milk");

        var response = await CompleteListAsync(list.Id);
        var completed = await response.Content.ReadFromJsonAsync<ShoppingListWithItems>(JsonOptions);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(completed!.Items.Select(i => i.Text), Is.EquivalentTo(new[] { "Milk" }));

        var archived = await (await GetArchivedAsUser()).Content.ReadFromJsonAsync<List<ShoppingListWithItems>>(JsonOptions);
        Assert.That(archived!.Count, Is.EqualTo(1));
        Assert.That(archived[0].Name, Is.EqualTo("Groceries"));
    }

    [Test]
    public async Task Complete_SetsArchivedAt_AndArchivedExcludedFromDefaultGet()
    {
        var list = await CreateList("Groceries");
        var response = await CompleteListAsync(list.Id);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var completed = await response.Content.ReadFromJsonAsync<ShoppingList>(JsonOptions);
        Assert.That(completed!.Status, Is.EqualTo(ShoppingListStatus.Archived));
        Assert.That(completed.ArchivedAt, Is.Not.Null);

        var active = await (await GetAsUser()).Content.ReadFromJsonAsync<List<ShoppingListWithItems>>(JsonOptions);
        Assert.That(active, Is.Empty);
    }

    [Test]
    public async Task Complete_Returns400_WhenAlreadyArchived()
    {
        var list = await CreateList("Groceries");
        await CompleteListAsync(list.Id);

        var response = await CompleteListAsync(list.Id);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Complete_Returns404_WhenListBelongsToOtherUser()
    {
        var otherUserId = await SeedTestUserAsync("Other Owner");
        var theirs = await CreateList("Theirs", otherUserId);

        var response = await CompleteListAsync(theirs.Id);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Complete_Returns404_WhenListDoesNotExist()
    {
        var response = await CompleteListAsync("missing-list-id");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Complete_WithoutUserId_Returns401()
    {
        var list = await CreateList("Groceries");
        var response = await SendAsUserAsync(HttpMethod.Post, $"/api/shopping-lists/{list.Id}/complete");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task ArchivedGet_ReturnsOnlyRequestingUsersArchivedLists()
    {
        var otherUserId = await SeedTestUserAsync("Other Owner");
        var mine = await CreateList("Mine");
        var theirs = await CreateList("Theirs", otherUserId);
        await CompleteListAsync(mine.Id);
        await CompleteListAsync(theirs.Id, otherUserId);

        var archived = await (await GetArchivedAsUser()).Content.ReadFromJsonAsync<List<ShoppingListWithItems>>(JsonOptions);

        Assert.That(archived!.Count, Is.EqualTo(1));
        Assert.That(archived[0].Name, Is.EqualTo("Mine"));
    }

    [Test]
    public async Task ArchivedGet_WithoutUserId_Returns401()
    {
        var response = await SendAsUserAsync(HttpMethod.Get, "/api/shopping-lists?status=archived");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task AddItem_Returns400_WhenListIsArchived()
    {
        var list = await CreateList("Groceries");
        await CompleteListAsync(list.Id);

        var response = await PostItemAsync(list.Id, new { Text = "Milk" });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task ToggleItem_Returns400_WhenListIsArchived()
    {
        var list = await CreateList("Groceries");
        var item = await AddItemAsync(list.Id, "Milk");
        await CompleteListAsync(list.Id);

        var response = await PatchItemAsync(list.Id, item.Id);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task ArchivedList_RemainsFullyReadable()
    {
        var list = await CreateList("Groceries");
        await AddItemAsync(list.Id, "Milk");
        await AddItemAsync(list.Id, "Eggs");
        await CompleteListAsync(list.Id);

        var archived = await (await GetArchivedAsUser()).Content.ReadFromJsonAsync<List<ShoppingListWithItems>>(JsonOptions);

        Assert.That(archived!.Count, Is.EqualTo(1));
        Assert.That(archived[0].Items.Select(i => i.Text), Is.EquivalentTo(new[] { "Milk", "Eggs" }));
    }

    private Task<HttpResponseMessage> DeleteListAsync(string listId, string? userId = null) =>
        SendAsUserAsync(HttpMethod.Delete, $"/api/shopping-lists/{listId}", null, userId ?? _userId);

    [Test]
    public async Task Delete_RemovesListAndItsItems()
    {
        var list = await CreateList("Groceries");
        await AddItemAsync(list.Id, "Milk");

        var response = await DeleteListAsync(list.Id);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var active = await (await GetAsUser()).Content.ReadFromJsonAsync<List<ShoppingListWithItems>>(JsonOptions);
        Assert.That(active, Is.Empty);

        var archived = await (await GetArchivedAsUser()).Content.ReadFromJsonAsync<List<ShoppingListWithItems>>(JsonOptions);
        Assert.That(archived, Is.Empty);
    }

    [Test]
    public async Task Delete_RemovesArchivedList()
    {
        var list = await CreateList("Groceries");
        await CompleteListAsync(list.Id);

        var response = await DeleteListAsync(list.Id);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var archived = await (await GetArchivedAsUser()).Content.ReadFromJsonAsync<List<ShoppingListWithItems>>(JsonOptions);
        Assert.That(archived, Is.Empty);
    }

    [Test]
    public async Task Delete_Returns404_WhenListBelongsToOtherUser()
    {
        var otherUserId = await SeedTestUserAsync("Other Owner");
        var theirs = await CreateList("Theirs", otherUserId);

        var response = await DeleteListAsync(theirs.Id);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var theirsLists = await SendAsUserAsync(HttpMethod.Get, "/api/shopping-lists", null, otherUserId);
        var items = await theirsLists.Content.ReadFromJsonAsync<List<ShoppingListWithItems>>(JsonOptions);
        Assert.That(items!.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task Delete_Returns404_WhenListDoesNotExist()
    {
        var response = await DeleteListAsync("missing-list-id");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Delete_WithoutUserId_Returns401()
    {
        var list = await CreateList("Groceries");
        var response = await SendAsUserAsync(HttpMethod.Delete, $"/api/shopping-lists/{list.Id}");
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

    // --- POST /api/shopping-lists with premadeListId ---

    private async Task<SavedItem> CreateSavedItemAsync(string name, string? userId = null)
    {
        var response = await SendAsUserAsync(
            HttpMethod.Post, "/api/saved-items", new { Name = name }, userId ?? _userId);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<SavedItem>())!;
    }

    private async Task<PremadeList> CreatePremadeListAsync(string name, string? userId = null)
    {
        var response = await SendAsUserAsync(
            HttpMethod.Post, "/api/premade-lists", new { Name = name }, userId ?? _userId);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<PremadeList>())!;
    }

    private async Task AddMembershipAsync(string listId, string savedItemId, string? userId = null)
    {
        var response = await SendAsUserAsync(
            HttpMethod.Post, $"/api/premade-lists/{listId}/items", new { SavedItemId = savedItemId }, userId ?? _userId);
        response.EnsureSuccessStatusCode();
    }

    [Test]
    public async Task Create_WithPremadeList_ExpandsSavedItemsAsUncheckedSnapshots()
    {
        var saved1 = await CreateSavedItemAsync("Milk");
        var saved2 = await CreateSavedItemAsync("Eggs");
        var premade = await CreatePremadeListAsync("Weekly groceries");
        await AddMembershipAsync(premade.Id, saved1.Id);
        await AddMembershipAsync(premade.Id, saved2.Id);

        var response = await PostAsUser(new { Name = "Shop run", PremadeListId = premade.Id });
        var created = await response.Content.ReadFromJsonAsync<ShoppingListWithItems>(JsonOptions);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(created!.Name, Is.EqualTo("Shop run"));
        Assert.That(created.Items.Select(i => i.Text), Is.EquivalentTo(new[] { "Milk", "Eggs" }));
        Assert.That(created.Items.All(i => !i.IsChecked), Is.True);
    }

    [Test]
    public async Task Create_WithNullPremadeListId_BehavesUnchanged()
    {
        var response = await PostAsUser(new { Name = "Plain", PremadeListId = (string?)null });
        var created = await response.Content.ReadFromJsonAsync<ShoppingListWithItems>(JsonOptions);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(created!.Items, Is.Empty);
    }

    [Test]
    public async Task Create_WithUnknownPremadeListId_Returns404()
    {
        var response = await PostAsUser(new { Name = "Shop run", PremadeListId = "does-not-exist" });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Create_WithForeignPremadeListId_Returns404()
    {
        var otherUserId = await SeedTestUserAsync("Other Owner");
        var theirs = await CreatePremadeListAsync("Theirs", otherUserId);

        var response = await PostAsUser(new { Name = "Shop run", PremadeListId = theirs.Id });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        var lists = await (await GetAsUser()).Content.ReadFromJsonAsync<List<ShoppingListWithItems>>(JsonOptions);
        Assert.That(lists, Is.Empty);
    }
}
