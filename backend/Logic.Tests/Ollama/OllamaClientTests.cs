using System.Net;
using System.Net.Http;
using System.Text;
using Logic.Ollama;
using Logic.SystemConfig;
using SC = Common.Models.SystemConfig;

namespace Logic.Tests.Ollama;

public class OllamaClientTests
{
    private static DictionarySystemConfigProvider MakeConfig(string url = "http://ollama:11434") =>
        new([
            new SC { Id = "ollama::url", Namespace = "ollama", Key = "url", Value = url, Type = "string" }
        ]);

    private static OllamaClient MakeClient(HttpStatusCode status, string body) =>
        new(new HttpClient(new FakeHandler(status, body)), MakeConfig());

    private class FakeHandler(HttpStatusCode status, string body) : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            return Task.FromResult(new HttpResponseMessage(status) { Content = content });
        }
    }

    private const string SuccessJson = """
        {
            "model": "llama3.2",
            "message": {
                "role": "assistant",
                "content": "Pick an activity!"
            },
            "done": true
        }
        """;

    [Test]
    public async Task ChatAsync_ReturnsContentFromResponse()
    {
        var client = MakeClient(HttpStatusCode.OK, SuccessJson);

        var result = await client.ChatAsync("llama3.2", "Which activity should I do?");

        Assert.That(result, Is.EqualTo("Pick an activity!"));
    }

    [Test]
    public async Task ChatAsync_SendsPostToCorrectEndpoint()
    {
        var handler = new FakeHandler(HttpStatusCode.OK, SuccessJson);
        var client = new OllamaClient(new HttpClient(handler), MakeConfig("http://ollama:11434"));

        await client.ChatAsync("llama3.2", "hello");

        Assert.That(handler.LastRequest!.Method, Is.EqualTo(HttpMethod.Post));
        Assert.That(handler.LastRequest.RequestUri!.ToString(), Is.EqualTo("http://ollama:11434/api/chat"));
    }

    [Test]
    public async Task ChatAsync_SendsModelAndPromptInBody()
    {
        var handler = new FakeHandler(HttpStatusCode.OK, SuccessJson);
        var client = new OllamaClient(new HttpClient(handler), MakeConfig());

        await client.ChatAsync("llama3.2", "my prompt");

        var body = await handler.LastRequest!.Content!.ReadAsStringAsync();
        Assert.That(body, Does.Contain("llama3.2"));
        Assert.That(body, Does.Contain("my prompt"));
    }

    [Test]
    public async Task ChatAsync_SendsStreamFalse()
    {
        var handler = new FakeHandler(HttpStatusCode.OK, SuccessJson);
        var client = new OllamaClient(new HttpClient(handler), MakeConfig());

        await client.ChatAsync("llama3.2", "hello");

        var body = await handler.LastRequest!.Content!.ReadAsStringAsync();
        Assert.That(body, Does.Contain("\"stream\":false").Or.Contain("\"stream\": false"));
    }

    [Test]
    public void ChatAsync_ThrowsOnHttpError()
    {
        var client = MakeClient(HttpStatusCode.InternalServerError, "error");

        Assert.ThrowsAsync<HttpRequestException>(() => client.ChatAsync("llama3.2", "hello"));
    }

    [Test]
    public async Task ChatAsync_DoesNotSendAuthorizationHeader()
    {
        var handler = new FakeHandler(HttpStatusCode.OK, SuccessJson);
        var client = new OllamaClient(new HttpClient(handler), MakeConfig());

        await client.ChatAsync("llama3.2", "hello");

        Assert.That(handler.LastRequest!.Headers.Authorization, Is.Null);
    }
}
