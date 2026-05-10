using System.Net;
using System.Net.Http.Json;
using Common.Interfaces;
using Common.Models;
using InMemory;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace WebAPI.Integration.Tests.Controllers;

public class ComfyUiGenerateTests
{
    private FakeComfyUiClient _comfyUiClient = null!;
    private HttpClient _client = null!;
    private WebApplicationFactory<Program> _factory = null!;

    [SetUp]
    public void SetUp()
    {
        InMemoryGenericCrud<ComfyUiWorkflow>.ClearStaticState();
        _comfyUiClient = new FakeComfyUiClient();
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                builder.ConfigureServices(services =>
                {
                    services.AddScoped<IComfyUiClient>(_ => _comfyUiClient);
                });
            });
        _client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    private async Task<string> CreateWorkflowAsync(string json = """{"1":{"text":"{{prompt}}"}}""")
    {
        var response = await _client.PostAsJsonAsync("/api/comfyui/workflows",
            new { Name = "Test", WorkflowJson = json });
        var created = await response.Content.ReadFromJsonAsync<WorkflowListItem>();
        return created!.Id;
    }

    private record WorkflowListItem(string Id, string Name);
    private record GenerateRequest(string WorkflowId, string Prompt, string? NegativePrompt = null);
    private record GenerateResponse(string ImageBase64);

    [Test]
    public async Task Generate_ReturnsBase64Image_OnSuccess()
    {
        var id = await CreateWorkflowAsync();
        _comfyUiClient.ImageBytes = [0x89, 0x50, 0x4E, 0x47];

        var response = await _client.PostAsJsonAsync("/api/comfyui/generate",
            new GenerateRequest(id, "a cat"));
        var result = await response.Content.ReadFromJsonAsync<GenerateResponse>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(result!.ImageBase64, Does.StartWith("data:image/png;base64,"));
    }

    [Test]
    public async Task Generate_InjectsPromptIntoWorkflow()
    {
        var id = await CreateWorkflowAsync("""{"text":"{{prompt}}"}""");

        await _client.PostAsJsonAsync("/api/comfyui/generate",
            new GenerateRequest(id, "a dog"));

        Assert.That(_comfyUiClient.LastWorkflowJson, Does.Contain("a dog"));
        Assert.That(_comfyUiClient.LastWorkflowJson, Does.Not.Contain("{{prompt}}"));
    }

    [Test]
    public async Task Generate_InjectsNegativePromptIntoWorkflow()
    {
        var id = await CreateWorkflowAsync("""{"pos":"{{prompt}}","neg":"{{negative_prompt}}"}""");

        await _client.PostAsJsonAsync("/api/comfyui/generate",
            new GenerateRequest(id, "a cat", "blurry"));

        Assert.That(_comfyUiClient.LastWorkflowJson, Does.Contain("blurry"));
        Assert.That(_comfyUiClient.LastWorkflowJson, Does.Not.Contain("{{negative_prompt}}"));
    }

    [Test]
    public async Task Generate_ReplacesNegativePromptWithEmpty_WhenOmitted()
    {
        var id = await CreateWorkflowAsync("""{"neg":"{{negative_prompt}}"}""");

        await _client.PostAsJsonAsync("/api/comfyui/generate",
            new GenerateRequest(id, "a cat"));

        Assert.That(_comfyUiClient.LastWorkflowJson, Does.Not.Contain("{{negative_prompt}}"));
    }

    [Test]
    public async Task Generate_Returns404_WhenWorkflowNotFound()
    {
        var response = await _client.PostAsJsonAsync("/api/comfyui/generate",
            new GenerateRequest("nonexistent-id", "a cat"));

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task Generate_Returns500WithMessage_WhenClientThrows()
    {
        var id = await CreateWorkflowAsync();
        _comfyUiClient.ShouldThrow = true;

        var response = await _client.PostAsJsonAsync("/api/comfyui/generate",
            new GenerateRequest(id, "a cat"));
        var body = await response.Content.ReadAsStringAsync();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        Assert.That(body, Does.Contain("ComfyUI"));
    }

    private class FakeComfyUiClient : IComfyUiClient
    {
        public byte[] ImageBytes { get; set; } = [0x89];
        public bool ShouldThrow { get; set; }
        public string? LastWorkflowJson { get; private set; }

        public Task<string> SubmitPromptAsync(string workflowJson)
        {
            LastWorkflowJson = workflowJson;
            if (ShouldThrow) throw new HttpRequestException("ComfyUI unreachable");
            return Task.FromResult("test-prompt-id");
        }

        public Task<byte[]> PollForImageAsync(string promptId)
        {
            if (ShouldThrow) throw new HttpRequestException("ComfyUI unreachable");
            return Task.FromResult(ImageBytes);
        }
    }
}
