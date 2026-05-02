using System.Net;
using System.Net.Http.Json;
using Common.Models;
using InMemory;

namespace WebAPI.Integration.Tests.Controllers;

public class ActivityControllerTests : ControllerTestBase
{
    private record CreateActivityRequest(string Name, int Weight);
    private record UpdateActivityRequest(string Name, int Weight);

    [SetUp]
    public void ClearActivityState() => InMemoryCrud<Activity>.ClearStaticState();

    [Test]
    public async Task GetAll_ReturnsEmptyList_WhenNoActivitiesExist()
    {
        var response = await GetRawAsync("/api/activities", includeToken: false);
        var items = await response.Content.ReadFromJsonAsync<List<Activity>>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(items, Is.Empty);
    }

    [Test]
    public async Task Create_ReturnsCreatedActivity_WithCorrectFields()
    {
        var response = await PostRawAsync("/api/activities",
            new CreateActivityRequest("Play Chess", 3));
        var created = await response.Content.ReadFromJsonAsync<Activity>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(created!.Name, Is.EqualTo("Play Chess"));
        Assert.That(created.Weight, Is.EqualTo(3));
        Assert.That(created.Id, Is.Not.Empty);
    }

    [Test]
    public async Task Create_AppearsInGetAll_AfterCreation()
    {
        await PostRawAsync("/api/activities", new CreateActivityRequest("Read a Book", 2));

        var response = await GetRawAsync("/api/activities", includeToken: false);
        var items = await response.Content.ReadFromJsonAsync<List<Activity>>();

        Assert.That(items!.Any(a => a.Name == "Read a Book"), Is.True);
    }

    [Test]
    public async Task Create_Returns400_WhenWeightBelowRange()
    {
        var response = await PostRawAsync("/api/activities",
            new CreateActivityRequest("Bad Activity", 0));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Create_Returns400_WhenWeightAboveRange()
    {
        var response = await PostRawAsync("/api/activities",
            new CreateActivityRequest("Bad Activity", 6));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Update_ChangesNameAndWeight()
    {
        var createResp = await PostRawAsync("/api/activities",
            new CreateActivityRequest("Old Name", 1));
        var created = await createResp.Content.ReadFromJsonAsync<Activity>();

        var updateResp = await PutRawAsync($"/api/activities/{created!.Id}",
            new UpdateActivityRequest("New Name", 5));
        var updated = await updateResp.Content.ReadFromJsonAsync<Activity>();

        Assert.That(updateResp.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(updated!.Name, Is.EqualTo("New Name"));
        Assert.That(updated.Weight, Is.EqualTo(5));
    }

    [Test]
    public async Task Update_Returns404_WhenIdNotFound()
    {
        var response = await PutRawAsync("/api/activities/nonexistent-id",
            new UpdateActivityRequest("X", 1));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Delete_RemovesActivity_FromGetAll()
    {
        var createResp = await PostRawAsync("/api/activities",
            new CreateActivityRequest("To Delete", 2));
        var created = await createResp.Content.ReadFromJsonAsync<Activity>();

        var deleteResp = await DeleteRawAsync($"/api/activities/{created!.Id}");
        var allResp = await GetRawAsync("/api/activities", includeToken: false);
        var items = await allResp.Content.ReadFromJsonAsync<List<Activity>>();

        Assert.That(deleteResp.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        Assert.That(items!.Any(a => a.Id == created.Id), Is.False);
    }

    [Test]
    public async Task Delete_Returns404_WhenIdNotFound()
    {
        var response = await DeleteRawAsync("/api/activities/nonexistent-id");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
