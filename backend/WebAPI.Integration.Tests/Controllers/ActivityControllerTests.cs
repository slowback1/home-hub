using System.Net;
using System.Net.Http.Json;
using Common.Models;
using InMemory;

namespace WebAPI.Integration.Tests.Controllers;

public class ActivityControllerTests : ControllerTestBase
{
    private string _userId = string.Empty;

    [SetUp]
    public async Task ClearActivityState()
    {
        InMemoryCrud<Activity>.ClearStaticState();
        InMemoryCrud<AppUser>.ClearStaticState();
        _userId = await SeedTestUserAsync("Activity Owner");
    }

    private Task<HttpResponseMessage> PostAsUser(object body) =>
        SendAsUserAsync(HttpMethod.Post, "/api/activities", body, _userId);

    private Task<HttpResponseMessage> GetAsUser() =>
        SendAsUserAsync(HttpMethod.Get, "/api/activities", null, _userId);

    [Test]
    public async Task GetAll_ReturnsEmptyList_WhenNoActivitiesExist()
    {
        var response = await GetAsUser();
        var items = await response.Content.ReadFromJsonAsync<List<Activity>>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(items, Is.Empty);
    }

    [Test]
    public async Task Create_ReturnsCreatedActivity_WithCorrectFields()
    {
        var response = await PostAsUser(new { Name = "Play Chess", Weight = 3 });
        var created = await response.Content.ReadFromJsonAsync<Activity>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(created!.Name, Is.EqualTo("Play Chess"));
        Assert.That(created.Weight, Is.EqualTo(3));
        Assert.That(created.Id, Is.Not.Empty);
        Assert.That(created.UserId, Is.EqualTo(_userId));
    }

    [Test]
    public async Task Create_AppearsInGetAll_AfterCreation()
    {
        await PostAsUser(new { Name = "Read a Book", Weight = 2 });

        var response = await GetAsUser();
        var items = await response.Content.ReadFromJsonAsync<List<Activity>>();

        Assert.That(items!.Any(a => a.Name == "Read a Book"), Is.True);
    }

    [Test]
    public async Task Create_Returns400_WhenWeightBelowRange()
    {
        var response = await PostAsUser(new { Name = "Bad Activity", Weight = 0 });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Create_Returns400_WhenWeightAboveRange()
    {
        var response = await PostAsUser(new { Name = "Bad Activity", Weight = 6 });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Update_ChangesNameAndWeight()
    {
        var createResp = await PostAsUser(new { Name = "Old Name", Weight = 1 });
        var created = await createResp.Content.ReadFromJsonAsync<Activity>();

        var updateResp = await SendAsUserAsync(HttpMethod.Put,
            $"/api/activities/{created!.Id}", new { Name = "New Name", Weight = 5 }, _userId);
        var updated = await updateResp.Content.ReadFromJsonAsync<Activity>();

        Assert.That(updateResp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(updated!.Name, Is.EqualTo("New Name"));
        Assert.That(updated.Weight, Is.EqualTo(5));
    }

    [Test]
    public async Task Update_Returns404_WhenIdNotFound()
    {
        var response = await SendAsUserAsync(HttpMethod.Put,
            "/api/activities/nonexistent-id", new { Name = "X", Weight = 1 }, _userId);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Delete_RemovesActivity_FromGetAll()
    {
        var createResp = await PostAsUser(new { Name = "To Delete", Weight = 2 });
        var created = await createResp.Content.ReadFromJsonAsync<Activity>();

        var deleteResp = await SendAsUserAsync(HttpMethod.Delete,
            $"/api/activities/{created!.Id}", null, _userId);
        var allResp = await GetAsUser();
        var items = await allResp.Content.ReadFromJsonAsync<List<Activity>>();

        Assert.That(deleteResp.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        Assert.That(items!.Any(a => a.Id == created.Id), Is.False);
    }

    [Test]
    public async Task Delete_Returns404_WhenIdNotFound()
    {
        var response = await SendAsUserAsync(HttpMethod.Delete,
            "/api/activities/nonexistent-id", null, _userId);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    // --- per-user scoping ---

    [Test]
    public async Task GetAll_ReturnsOnlyRequestingUsersActivities()
    {
        var otherUserId = await SeedTestUserAsync("Other Owner");

        await PostAsUser(new { Name = "Mine", Weight = 1 });
        await SendAsUserAsync(HttpMethod.Post, "/api/activities",
            new { Name = "Theirs", Weight = 1 }, otherUserId);

        var items = await (await GetAsUser()).Content.ReadFromJsonAsync<List<Activity>>();

        Assert.That(items!.Count, Is.EqualTo(1));
        Assert.That(items[0].Name, Is.EqualTo("Mine"));
    }

    [Test]
    public async Task Update_Returns404_WhenActivityBelongsToAnotherUser()
    {
        var otherUserId = await SeedTestUserAsync("Other Owner");
        var createResp = await SendAsUserAsync(HttpMethod.Post, "/api/activities",
            new { Name = "Theirs", Weight = 1 }, otherUserId);
        var created = await createResp.Content.ReadFromJsonAsync<Activity>();

        var response = await SendAsUserAsync(HttpMethod.Put,
            $"/api/activities/{created!.Id}", new { Name = "Hijacked", Weight = 1 }, _userId);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Delete_Returns404_WhenActivityBelongsToAnotherUser()
    {
        var otherUserId = await SeedTestUserAsync("Other Owner");
        var createResp = await SendAsUserAsync(HttpMethod.Post, "/api/activities",
            new { Name = "Theirs", Weight = 1 }, otherUserId);
        var created = await createResp.Content.ReadFromJsonAsync<Activity>();

        var response = await SendAsUserAsync(HttpMethod.Delete,
            $"/api/activities/{created!.Id}", null, _userId);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task GetAll_Returns401_WithoutUserIdHeader()
    {
        var response = await GetRawAsync("/api/activities", includeToken: false);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task Create_Returns401_WithoutUserIdHeader()
    {
        var response = await PostRawAsync("/api/activities",
            new { Name = "No User", Weight = 1 }, includeToken: false);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }
}
