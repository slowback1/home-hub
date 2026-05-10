using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;
using InMemory;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace WebAPI.Integration.Tests.Controllers;

public class ComfyUiControllerTests
{
    private record CreateWorkflowRequest(string Name, string WorkflowJson);
    private record WorkflowListItem(string Id, string Name);

    private HttpClient _client = null!;
    private WebApplicationFactory<Program> _factory = null!;

    [SetUp]
    public void SetUp()
    {
        InMemoryGenericCrud<ComfyUiWorkflow>.ClearStaticState();
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                builder.ConfigureServices(services =>
                    services.AddScoped<IComfyUiClient>(_ => new NoOpComfyUiClient()));
            });
        _client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    private class NoOpComfyUiClient : IComfyUiClient
    {
        public Task<string> SubmitPromptAsync(string workflowJson) => Task.FromResult("unused");
        public Task<byte[]> PollForImageAsync(string promptId) => Task.FromResult(Array.Empty<byte>());
    }

    [Test]
    public async Task GetWorkflows_ReturnsEmptyList_WhenNoneExist()
    {
        var response = await _client.GetAsync("/api/comfyui/workflows");
        var items = await response.Content.ReadFromJsonAsync<List<WorkflowListItem>>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(items, Is.Empty);
    }

    [Test]
    public async Task CreateWorkflow_ReturnsCreatedWorkflow()
    {
        var response = await _client.PostAsJsonAsync("/api/comfyui/workflows",
            new CreateWorkflowRequest("My Workflow", """{"1":{}}"""));
        var created = await response.Content.ReadFromJsonAsync<WorkflowListItem>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(created!.Name, Is.EqualTo("My Workflow"));
        Assert.That(created.Id, Is.Not.Empty);
    }

    [Test]
    public async Task CreateWorkflow_AppearsInGetAll()
    {
        await _client.PostAsJsonAsync("/api/comfyui/workflows",
            new CreateWorkflowRequest("Test Workflow", """{"1":{}}"""));

        var response = await _client.GetAsync("/api/comfyui/workflows");
        var items = await response.Content.ReadFromJsonAsync<List<WorkflowListItem>>();

        Assert.That(items!.Any(w => w.Name == "Test Workflow"), Is.True);
    }

    [Test]
    public async Task GetWorkflows_DoesNotReturnWorkflowJson()
    {
        await _client.PostAsJsonAsync("/api/comfyui/workflows",
            new CreateWorkflowRequest("Test", """{"secret":"data"}"""));

        var response = await _client.GetAsync("/api/comfyui/workflows");
        var body = await response.Content.ReadAsStringAsync();

        Assert.That(body, Does.Not.Contain("secret"));
    }

    [Test]
    public async Task DeleteWorkflow_RemovesItFromList()
    {
        var createResp = await _client.PostAsJsonAsync("/api/comfyui/workflows",
            new CreateWorkflowRequest("To Delete", """{"1":{}}"""));
        var created = await createResp.Content.ReadFromJsonAsync<WorkflowListItem>();

        var deleteResp = await _client.DeleteAsync($"/api/comfyui/workflows/{created!.Id}");
        var listResp = await _client.GetAsync("/api/comfyui/workflows");
        var items = await listResp.Content.ReadFromJsonAsync<List<WorkflowListItem>>();

        Assert.That(deleteResp.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        Assert.That(items!.Any(w => w.Id == created.Id), Is.False);
    }

    [Test]
    public async Task DeleteWorkflow_Returns404_WhenNotFound()
    {
        var response = await _client.DeleteAsync("/api/comfyui/workflows/nonexistent-id");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
