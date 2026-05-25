using System.Net;
using System.Net.Http.Json;
using Common.Models;
using InMemory;

namespace WebAPI.Integration.Tests.Controllers;

public class ChoreTaskCompletionTests : ControllerTestBase
{
    private record CreateRequest(string Name, bool IsRecurring, int? IntervalDays, DateTime? DoDate);

    [SetUp]
    public void ClearState()
    {
        InMemoryCrud<ChoreTask>.ClearStaticState();
        InMemoryCrud<TaskCompletion>.ClearStaticState();
    }

    // --- POST /api/tasks/{id}/complete ---

    [Test]
    public async Task Complete_OneOffTask_StampsCompletedAt_AndDisappearsFromActive()
    {
        var created = await PostAsync<CreateRequest, ChoreTask>("/api/tasks",
            new CreateRequest("Plan vacation", false, null, null));

        var response = await PostRawAsync($"/api/tasks/{created!.Id}/complete", new { });
        var completed = await response.Content.ReadFromJsonAsync<ChoreTask>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(completed!.CompletedAt, Is.Not.Null);

        var active = await GetAsync<List<ChoreTask>>("/api/tasks", includeToken: false);
        Assert.That(active!.Any(t => t.Id == created.Id), Is.False);
    }

    [Test]
    public async Task Complete_RecurringTask_AdvancesDoDate_AndRemainsActive()
    {
        var today = DateTime.UtcNow.Date;
        var created = await PostAsync<CreateRequest, ChoreTask>("/api/tasks",
            new CreateRequest("Sweep floors", true, 7, today));

        var response = await PostRawAsync($"/api/tasks/{created!.Id}/complete", new { });
        var completed = await response.Content.ReadFromJsonAsync<ChoreTask>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(completed!.CompletedAt, Is.Null);
        Assert.That(completed.DoDate, Is.Not.Null);
        Assert.That(completed.DoDate!.Value.Date, Is.EqualTo(today.AddDays(7)));

        var active = await GetAsync<List<ChoreTask>>("/api/tasks", includeToken: false);
        Assert.That(active!.Any(t => t.Id == created.Id), Is.True);
    }

    [Test]
    public async Task Complete_Returns404_WhenTaskNotFound()
    {
        var response = await PostRawAsync("/api/tasks/nonexistent/complete", new { });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    // --- DELETE /api/tasks/{id}/completions/latest ---

    [Test]
    public async Task Undo_OneOffTask_ClearsCompletedAt_AndReappearsInActive()
    {
        var created = await PostAsync<CreateRequest, ChoreTask>("/api/tasks",
            new CreateRequest("Plan vacation", false, null, null));
        await PostRawAsync($"/api/tasks/{created!.Id}/complete", new { });

        var response = await DeleteRawAsync($"/api/tasks/{created.Id}/completions/latest");
        var restored = await response.Content.ReadFromJsonAsync<ChoreTask>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(restored!.CompletedAt, Is.Null);

        var active = await GetAsync<List<ChoreTask>>("/api/tasks", includeToken: false);
        Assert.That(active!.Any(t => t.Id == created.Id), Is.True);
    }

    [Test]
    public async Task Undo_RecurringTask_RestoresPreviousDoDate()
    {
        var originalDoDate = new DateTime(2026, 5, 20, 0, 0, 0, DateTimeKind.Utc);
        var created = await PostAsync<CreateRequest, ChoreTask>("/api/tasks",
            new CreateRequest("Sweep floors", true, 7, originalDoDate));
        await PostRawAsync($"/api/tasks/{created!.Id}/complete", new { });

        var response = await DeleteRawAsync($"/api/tasks/{created.Id}/completions/latest");
        var restored = await response.Content.ReadFromJsonAsync<ChoreTask>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(restored!.DoDate!.Value.Date, Is.EqualTo(originalDoDate.Date));
    }

    [Test]
    public async Task Undo_Returns404_WhenTaskNotFound()
    {
        var response = await DeleteRawAsync("/api/tasks/nonexistent/completions/latest");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Undo_Returns404_WhenNoCompletionsExist()
    {
        var created = await PostAsync<CreateRequest, ChoreTask>("/api/tasks",
            new CreateRequest("Plan vacation", false, null, null));

        var response = await DeleteRawAsync($"/api/tasks/{created!.Id}/completions/latest");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task GetActive_ExcludesCompletedOneOffTasks()
    {
        var created = await PostAsync<CreateRequest, ChoreTask>("/api/tasks",
            new CreateRequest("Done task", false, null, null));
        await PostRawAsync($"/api/tasks/{created!.Id}/complete", new { });

        var active = await GetAsync<List<ChoreTask>>("/api/tasks", includeToken: false);
        Assert.That(active!.Any(t => t.Id == created.Id), Is.False);
    }

    [Test]
    public async Task GetHistory_ReturnsCompletedOneOffTasks()
    {
        var created = await PostAsync<CreateRequest, ChoreTask>("/api/tasks",
            new CreateRequest("Finished task", false, null, null));
        await PostRawAsync($"/api/tasks/{created!.Id}/complete", new { });

        var history = await GetAsync<List<ChoreTask>>("/api/tasks/history", includeToken: false);
        Assert.That(history!.Any(t => t.Id == created.Id), Is.True);
    }
}
