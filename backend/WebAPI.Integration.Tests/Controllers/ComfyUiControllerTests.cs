using System.Net;
using System.Net.Http.Json;
using Common.Models;
using InMemory;

namespace WebAPI.Integration.Tests.Controllers;

public class ComfyUiControllerTests : ControllerTestBase
{
    private record CreateWorkflowRequest(string Name, string WorkflowJson);
    private record WorkflowListItem(string Id, string Name);

    [SetUp]
    public void ClearWorkflowState() => InMemoryGenericCrud<ComfyUiWorkflow>.ClearStaticState();

    [Test]
    public async Task GetWorkflows_ReturnsEmptyList_WhenNoneExist()
    {
        var response = await GetRawAsync("/api/comfyui/workflows");
        var items = await response.Content.ReadFromJsonAsync<List<WorkflowListItem>>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(items, Is.Empty);
    }

    [Test]
    public async Task CreateWorkflow_ReturnsCreatedWorkflow()
    {
        var response = await PostRawAsync("/api/comfyui/workflows",
            new CreateWorkflowRequest("My Workflow", """{"1":{}}"""));
        var created = await response.Content.ReadFromJsonAsync<WorkflowListItem>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(created!.Name, Is.EqualTo("My Workflow"));
        Assert.That(created.Id, Is.Not.Empty);
    }

    [Test]
    public async Task CreateWorkflow_AppearsInGetAll()
    {
        await PostRawAsync("/api/comfyui/workflows",
            new CreateWorkflowRequest("Test Workflow", """{"1":{}}"""));

        var response = await GetRawAsync("/api/comfyui/workflows");
        var items = await response.Content.ReadFromJsonAsync<List<WorkflowListItem>>();

        Assert.That(items!.Any(w => w.Name == "Test Workflow"), Is.True);
    }

    [Test]
    public async Task GetWorkflows_DoesNotReturnWorkflowJson()
    {
        await PostRawAsync("/api/comfyui/workflows",
            new CreateWorkflowRequest("Test", """{"secret":"data"}"""));

        var response = await GetRawAsync("/api/comfyui/workflows");
        var body = await response.Content.ReadAsStringAsync();

        Assert.That(body, Does.Not.Contain("secret"));
    }

    [Test]
    public async Task DeleteWorkflow_RemovesItFromList()
    {
        var createResp = await PostRawAsync("/api/comfyui/workflows",
            new CreateWorkflowRequest("To Delete", """{"1":{}}"""));
        var created = await createResp.Content.ReadFromJsonAsync<WorkflowListItem>();

        var deleteResp = await DeleteRawAsync($"/api/comfyui/workflows/{created!.Id}");
        var listResp = await GetRawAsync("/api/comfyui/workflows");
        var items = await listResp.Content.ReadFromJsonAsync<List<WorkflowListItem>>();

        Assert.That(deleteResp.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        Assert.That(items!.Any(w => w.Id == created.Id), Is.False);
    }

    [Test]
    public async Task DeleteWorkflow_Returns404_WhenNotFound()
    {
        var response = await DeleteRawAsync("/api/comfyui/workflows/nonexistent-id");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
