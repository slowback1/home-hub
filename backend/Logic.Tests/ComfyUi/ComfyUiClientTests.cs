using System.Net;
using System.Net.Http;
using System.Text;
using Logic.ComfyUi;
using Logic.SystemConfig;
using SC = Common.Models.SystemConfig;

namespace Logic.Tests.ComfyUi;

public class ComfyUiClientTests
{
    private static DictionarySystemConfigProvider MakeConfig(string url = "http://comfyui:8188") =>
        new([
            new SC { Id = "comfyui::base_url", Namespace = "comfyui", Key = "base_url", Value = url, Type = "string" }
        ]);

    private class FakeHandler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
    {
        public List<HttpRequestMessage> Requests { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(request);
            return Task.FromResult(respond(request));
        }
    }

    private static FakeHandler SingleResponse(HttpStatusCode status, string body) =>
        new(_ => new HttpResponseMessage(status)
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json")
        });

    // --- SubmitPromptAsync ---

    [Test]
    public async Task SubmitPromptAsync_PostsToPromptEndpoint()
    {
        var handler = SingleResponse(HttpStatusCode.OK, """{"prompt_id":"abc123"}""");
        var client = new ComfyUiClient(new HttpClient(handler), MakeConfig());

        await client.SubmitPromptAsync("""{"1":{"inputs":{}}}""");

        Assert.That(handler.Requests[0].Method, Is.EqualTo(HttpMethod.Post));
        Assert.That(handler.Requests[0].RequestUri!.ToString(), Is.EqualTo("http://comfyui:8188/prompt"));
    }

    [Test]
    public async Task SubmitPromptAsync_ReturnsPromptId()
    {
        var handler = SingleResponse(HttpStatusCode.OK, """{"prompt_id":"abc123"}""");
        var client = new ComfyUiClient(new HttpClient(handler), MakeConfig());

        var promptId = await client.SubmitPromptAsync("""{"1":{"inputs":{}}}""");

        Assert.That(promptId, Is.EqualTo("abc123"));
    }

    [Test]
    public async Task SubmitPromptAsync_SendsWorkflowJsonAsBody()
    {
        var handler = SingleResponse(HttpStatusCode.OK, """{"prompt_id":"abc123"}""");
        var client = new ComfyUiClient(new HttpClient(handler), MakeConfig());
        const string workflow = """{"1":{"class_type":"KSampler"}}""";

        await client.SubmitPromptAsync(workflow);

        var body = await handler.Requests[0].Content!.ReadAsStringAsync();
        Assert.That(body, Does.Contain("KSampler"));
    }

    [Test]
    public void SubmitPromptAsync_ThrowsOnHttpError()
    {
        var handler = SingleResponse(HttpStatusCode.InternalServerError, "error");
        var client = new ComfyUiClient(new HttpClient(handler), MakeConfig());

        Assert.ThrowsAsync<HttpRequestException>(() => client.SubmitPromptAsync("""{}"""));
    }

    // --- PollForImageAsync ---

    [Test]
    public async Task PollForImageAsync_ReturnsImageBytesWhenComplete()
    {
        var pngBytes = new byte[] { 0x89, 0x50, 0x4E, 0x47 };
        var handler = new FakeHandler(req =>
        {
            if (req.RequestUri!.AbsolutePath.Contains("/history/"))
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        """{"test-id":{"outputs":{"9":{"images":[{"filename":"out.png","subfolder":"","type":"output"}]}}}}""",
                        Encoding.UTF8, "application/json")
                };
            // /view request
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(pngBytes)
            };
        });
        var client = new ComfyUiClient(new HttpClient(handler), MakeConfig());

        var result = await client.PollForImageAsync("test-id");

        Assert.That(result, Is.EqualTo(pngBytes));
    }

    [Test]
    public async Task PollForImageAsync_PollsHistoryUntilComplete()
    {
        var pngBytes = new byte[] { 0x89, 0x50, 0x4E, 0x47 };
        var callCount = 0;
        var handler = new FakeHandler(req =>
        {
            if (req.RequestUri!.AbsolutePath.Contains("/history/"))
            {
                callCount++;
                // Return empty on first call, complete on second
                var body = callCount == 1
                    ? "{}"
                    : """{"test-id":{"outputs":{"9":{"images":[{"filename":"out.png","subfolder":"","type":"output"}]}}}}""";
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(body, Encoding.UTF8, "application/json")
                };
            }
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(pngBytes)
            };
        });
        var client = new ComfyUiClient(new HttpClient(handler), MakeConfig(), pollIntervalMs: 1);

        await client.PollForImageAsync("test-id");

        Assert.That(callCount, Is.GreaterThanOrEqualTo(2));
    }

    [Test]
    public async Task PollForImageAsync_FetchesImageWithCorrectQueryParams()
    {
        var pngBytes = new byte[] { 0x89 };
        var handler = new FakeHandler(req =>
        {
            if (req.RequestUri!.AbsolutePath.Contains("/history/"))
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        """{"test-id":{"outputs":{"9":{"images":[{"filename":"out.png","subfolder":"sub","type":"output"}]}}}}""",
                        Encoding.UTF8, "application/json")
                };
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(pngBytes) };
        });
        var client = new ComfyUiClient(new HttpClient(handler), MakeConfig());

        await client.PollForImageAsync("test-id");

        var viewRequest = handler.Requests.First(r => r.RequestUri!.AbsolutePath.Contains("/view"));
        var query = viewRequest.RequestUri!.Query;
        Assert.That(query, Does.Contain("filename=out.png"));
        Assert.That(query, Does.Contain("subfolder=sub"));
        Assert.That(query, Does.Contain("type=output"));
    }
}
