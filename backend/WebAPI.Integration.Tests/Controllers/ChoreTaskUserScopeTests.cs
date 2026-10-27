using System.Net;
using System.Net.Http.Json;
using Common.Models;
using InMemory;

namespace WebAPI.Integration.Tests.Controllers;

public class ChoreTaskUserScopeTests : ControllerTestBase
{
    private string _userA = string.Empty;
    private string _userB = string.Empty;

    [SetUp]
    public async Task ClearState()
    {
        InMemoryCrud<ChoreTask>.ClearStaticState();
        InMemoryCrud<TaskCompletion>.ClearStaticState();
        InMemoryCrud<AppUser>.ClearStaticState();
        _userA = await SeedTestUserAsync("Scope User A");
        _userB = await SeedTestUserAsync("Scope User B");
    }

    private async Task SeedTask(string name, string? userId, bool completed = false)
    {
        var response = await PostRawAsync("/api/test/tasks",
            new
            {
                Name = name,
                UserId = userId,
                CompletedAt = completed ? DateTime.UtcNow : (DateTime?)null
            },
            includeToken: false);
        response.EnsureSuccessStatusCode();
    }

    [Test]
    public async Task GetActive_ReturnsOwnAndGlobalTasks_ExcludesOtherUsersTasks()
    {
        await SeedTask("A Task", _userA);
        await SeedTask("B Task", _userB);
        await SeedTask("Global Task", null);

        var response = await GetRawAsUserAsync("/api/tasks", _userA);
        response.EnsureSuccessStatusCode();
        var tasks = await response.Content.ReadFromJsonAsync<List<ChoreTask>>();

        Assert.That(tasks!.Select(t => t.Name), Is.EquivalentTo(new[] { "A Task", "Global Task" }));
    }

    [Test]
    public async Task GetHistory_ReturnsCompletedTasksVisibleToUser()
    {
        await SeedTask("A Done", _userA, completed: true);
        await SeedTask("B Done", _userB, completed: true);
        await SeedTask("Global Done", null, completed: true);

        var response = await GetRawAsUserAsync("/api/tasks/history", _userA);
        response.EnsureSuccessStatusCode();
        var tasks = await response.Content.ReadFromJsonAsync<List<ChoreTask>>();

        Assert.That(tasks!.Select(t => t.Name), Is.EquivalentTo(new[] { "A Done", "Global Done" }));
    }

    [Test]
    public async Task GetActive_WithoutUserHeader_Returns401()
    {
        var response = await GetRawAsync("/api/tasks");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task Create_PersonalTask_StampsRequestingUserId()
    {
        var response = await SendAsUserAsync(HttpMethod.Post, "/api/tasks",
            new { Name = "Personal", IsRecurring = false, IntervalDays = (int?)null, DoDate = (DateTime?)null, IsGlobal = false },
            _userA);
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<ChoreTask>();

        Assert.That(created!.UserId, Is.EqualTo(_userA));
    }

    [Test]
    public async Task Create_GlobalTask_LeavesUserIdNull()
    {
        var response = await SendAsUserAsync(HttpMethod.Post, "/api/tasks",
            new { Name = "Shared", IsRecurring = false, IntervalDays = (int?)null, DoDate = (DateTime?)null, IsGlobal = true },
            _userA);
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<ChoreTask>();

        Assert.That(created!.UserId, Is.Null);
    }

    [Test]
    public async Task Create_WithoutIsGlobal_DefaultsToGlobal()
    {
        var response = await SendAsUserAsync(HttpMethod.Post, "/api/tasks",
            new { Name = "Default", IsRecurring = false, IntervalDays = (int?)null, DoDate = (DateTime?)null },
            _userA);
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<ChoreTask>();

        Assert.That(created!.UserId, Is.Null);
    }

    [Test]
    public async Task Create_PersonalTask_WithoutUserHeader_Returns401()
    {
        var response = await PostRawAsync("/api/tasks",
            new { Name = "Personal", IsRecurring = false, IntervalDays = (int?)null, DoDate = (DateTime?)null, IsGlobal = false });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    private async Task<ChoreTask> CreatePersonalTask(string name, string userId)
    {
        var response = await SendAsUserAsync(HttpMethod.Post, "/api/tasks",
            new { Name = name, IsRecurring = false, IntervalDays = (int?)null, DoDate = (DateTime?)null, IsGlobal = false },
            userId);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ChoreTask>())!;
    }

    [Test]
    public async Task Update_NonOwnerPersonalTask_Returns404()
    {
        var task = await CreatePersonalTask("A private", _userA);
        var response = await SendAsUserAsync(HttpMethod.Put, $"/api/tasks/{task.Id}",
            new { Name = "Hijacked", IsRecurring = false, IntervalDays = (int?)null, DoDate = (DateTime?)null },
            _userB);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Delete_NonOwnerPersonalTask_Returns404()
    {
        var task = await CreatePersonalTask("A private", _userA);
        var response = await SendAsUserAsync(HttpMethod.Delete, $"/api/tasks/{task.Id}", userId: _userB);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Complete_NonOwnerPersonalTask_Returns404()
    {
        var task = await CreatePersonalTask("A private", _userA);
        var response = await SendAsUserAsync(HttpMethod.Post, $"/api/tasks/{task.Id}/complete", new { }, _userB);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Undo_NonOwnerPersonalTask_Returns404()
    {
        var task = await CreatePersonalTask("A private", _userA);
        await SendAsUserAsync(HttpMethod.Post, $"/api/tasks/{task.Id}/complete", new { }, _userA);

        var response = await SendAsUserAsync(HttpMethod.Delete, $"/api/tasks/{task.Id}/completions/latest", userId: _userB);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Update_OwnerPersonalTask_ReturnsOk()
    {
        var task = await CreatePersonalTask("A private", _userA);
        var response = await SendAsUserAsync(HttpMethod.Put, $"/api/tasks/{task.Id}",
            new { Name = "Renamed", IsRecurring = false, IntervalDays = (int?)null, DoDate = (DateTime?)null },
            _userA);
        var updated = await response.Content.ReadFromJsonAsync<ChoreTask>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(updated!.Name, Is.EqualTo("Renamed"));
    }

    [Test]
    public async Task Complete_GlobalTask_AsNonOwner_ReturnsOk_AndSharedForEveryone()
    {
        var response = await SendAsUserAsync(HttpMethod.Post, "/api/tasks",
            new { Name = "Shared chore", IsRecurring = false, IntervalDays = (int?)null, DoDate = (DateTime?)null, IsGlobal = true },
            _userA);
        var task = (await response.Content.ReadFromJsonAsync<ChoreTask>())!;

        var completeResponse = await SendAsUserAsync(HttpMethod.Post, $"/api/tasks/{task.Id}/complete", new { }, _userB);
        Assert.That(completeResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var aActive = await GetRawAsUserAsync("/api/tasks", _userA);
        var aTasks = await aActive.Content.ReadFromJsonAsync<List<ChoreTask>>();
        var bActive = await GetRawAsUserAsync("/api/tasks", _userB);
        var bTasks = await bActive.Content.ReadFromJsonAsync<List<ChoreTask>>();

        Assert.That(aTasks!.Any(t => t.Id == task.Id), Is.False);
        Assert.That(bTasks!.Any(t => t.Id == task.Id), Is.False);
    }
}
