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
}
