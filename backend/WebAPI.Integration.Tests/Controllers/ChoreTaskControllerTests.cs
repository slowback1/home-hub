using System.Net;
using System.Net.Http.Json;
using Common.Models;
using InMemory;

namespace WebAPI.Integration.Tests.Controllers;

public class ChoreTaskControllerTests : ControllerTestBase
{

    [SetUp]
    public void ClearState()
    {
        InMemoryCrud<ChoreTask>.ClearStaticState();
        InMemoryCrud<TaskCompletion>.ClearStaticState();
    }

    // --- GET /api/tasks ---

    [Test]
    public async Task GetActive_ReturnsEmptyList_WhenNoTasksExist()
    {
        var response = await GetRawAsync("/api/tasks", includeToken: false);
        var items = await response.Content.ReadFromJsonAsync<List<ChoreTask>>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(items, Is.Empty);
    }

    // --- POST /api/tasks ---

    [Test]
    public async Task Create_ReturnsCreatedTask_WithCorrectFields()
    {
        var doDate = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        var response = await PostRawAsync("/api/tasks",
            new { Name = "Sweep floors", IsRecurring = true, IntervalDays = 7, DoDate = doDate });
        var created = await response.Content.ReadFromJsonAsync<ChoreTask>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(created!.Name, Is.EqualTo("Sweep floors"));
        Assert.That(created.IsRecurring, Is.True);
        Assert.That(created.IntervalDays, Is.EqualTo(7));
        Assert.That(created.Id, Is.Not.Empty);
    }

    [Test]
    public async Task Create_OneOffTask_SetsIntervalDaysToNull()
    {
        var response = await PostRawAsync("/api/tasks",
            new { Name = "Plan vacation", IsRecurring = false, IntervalDays = 7, DoDate = (DateTime?)null });
        var created = await response.Content.ReadFromJsonAsync<ChoreTask>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(created!.IntervalDays, Is.Null);
    }

    [Test]
    public async Task Create_Returns400_WhenNameIsEmpty()
    {
        var response = await PostRawAsync("/api/tasks",
            new { Name = "", IsRecurring = false, IntervalDays = (int?)null, DoDate = (DateTime?)null });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Create_Returns400_WhenRecurringWithoutIntervalDays()
    {
        var response = await PostRawAsync("/api/tasks",
            new { Name = "Sweep floors", IsRecurring = true, IntervalDays = (int?)null, DoDate = (DateTime?)null });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Create_Returns400_WhenRecurringWithZeroIntervalDays()
    {
        var response = await PostRawAsync("/api/tasks",
            new { Name = "Sweep floors", IsRecurring = true, IntervalDays = 0, DoDate = (DateTime?)null });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    // --- PUT /api/tasks/{id} ---

    [Test]
    public async Task Update_ChangesNameAndDoDate()
    {
        var created = await PostAsync<object, ChoreTask>("/api/tasks",
            new { Name = "Old name", IsRecurring = false, IntervalDays = (int?)null, DoDate = (DateTime?)null });
        var newDate = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc);

        var response = await PutRawAsync($"/api/tasks/{created!.Id}",
            new { Name = "New name", IsRecurring = false, IntervalDays = (int?)null, DoDate = newDate });
        var updated = await response.Content.ReadFromJsonAsync<ChoreTask>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(updated!.Name, Is.EqualTo("New name"));
    }

    [Test]
    public async Task Update_Returns404_WhenTaskNotFound()
    {
        var response = await PutRawAsync("/api/tasks/nonexistent",
            new { Name = "Name", IsRecurring = false, IntervalDays = (int?)null, DoDate = (DateTime?)null });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Update_Returns400_WhenNameIsEmpty()
    {
        var created = await PostAsync<object, ChoreTask>("/api/tasks",
            new { Name = "Original", IsRecurring = false, IntervalDays = (int?)null, DoDate = (DateTime?)null });

        var response = await PutRawAsync($"/api/tasks/{created!.Id}",
            new { Name = "", IsRecurring = false, IntervalDays = (int?)null, DoDate = (DateTime?)null });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    // --- DELETE /api/tasks/{id} ---

    [Test]
    public async Task Delete_Returns204_AndTaskNoLongerReturned()
    {
        var created = await PostAsync<object, ChoreTask>("/api/tasks",
            new { Name = "Delete me", IsRecurring = false, IntervalDays = (int?)null, DoDate = (DateTime?)null });

        var deleteResponse = await DeleteRawAsync($"/api/tasks/{created!.Id}");
        Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var getResponse = await GetRawAsync("/api/tasks", includeToken: false);
        var items = await getResponse.Content.ReadFromJsonAsync<List<ChoreTask>>();
        Assert.That(items!.Any(t => t.Id == created.Id), Is.False);
    }

    [Test]
    public async Task Delete_Returns404_WhenTaskNotFound()
    {
        var response = await DeleteRawAsync("/api/tasks/nonexistent");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    // --- GET /api/tasks/history ---

    [Test]
    public async Task GetHistory_ReturnsOk_WithEmptyList_WhenNoCompletedTasksExist()
    {
        await PostAsync<object, ChoreTask>("/api/tasks",
            new { Name = "Active task", IsRecurring = false, IntervalDays = (int?)null, DoDate = (DateTime?)null });

        var response = await GetRawAsync("/api/tasks/history", includeToken: false);
        var items = await response.Content.ReadFromJsonAsync<List<ChoreTask>>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(items, Is.Empty);
    }
}
