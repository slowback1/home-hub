using System.Net;
using System.Net.Http;
using System.Text;
using Common.Models;
using Logic.Audiobook;
using Logic.SystemConfig;
using SC = Common.Models.SystemConfig;

namespace Logic.Tests.Audiobook;

public class GpuServiceClientTests
{
    private static DictionarySystemConfigProvider MakeConfig(string url = "http://gpu:8765", string apiKey = "test-key") =>
        new([
            new SC { Id = "audiobook::url", Namespace = "audiobook", Key = "url", Value = url, Type = "string" },
            new SC { Id = "audiobook::api_key", Namespace = "audiobook", Key = "api_key", Value = apiKey, Type = "string", IsSecret = true }
        ]);

    private static GpuServiceClient MakeClient(HttpStatusCode status, string body, string? contentType = null) =>
        new(new HttpClient(new FakeHandler(status, body, contentType)), MakeConfig());

    private class FakeHandler(HttpStatusCode status, string body, string? contentType) : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            var content = new StringContent(body, Encoding.UTF8, contentType ?? "application/json");
            return Task.FromResult(new HttpResponseMessage(status) { Content = content });
        }
    }

    private const string SingleJobJson = """
        {
            "id": "abc-123",
            "status": "queued",
            "epub_filename": "book.epub",
            "voice_sample_name": "default",
            "created_at": "2026-05-05T12:00:00+00:00",
            "updated_at": "2026-05-05T12:00:00+00:00",
            "error_message": null,
            "pid": null
        }
        """;

    private const string CompletedJobJson = """
        {
            "id": "abc-123",
            "status": "completed",
            "epub_filename": "book.epub",
            "voice_sample_name": "default",
            "created_at": "2026-05-05T12:00:00+00:00",
            "updated_at": "2026-05-05T12:01:00+00:00",
            "error_message": null,
            "pid": null
        }
        """;

    private const string FailedJobJson = """
        {
            "id": "abc-123",
            "status": "failed",
            "epub_filename": "book.epub",
            "voice_sample_name": "default",
            "created_at": "2026-05-05T12:00:00+00:00",
            "updated_at": "2026-05-05T12:01:00+00:00",
            "error_message": "TTS model error",
            "pid": null
        }
        """;

    // --- ListJobsAsync ---

    [Test]
    public async Task ListJobsAsync_MapsResponseToAudiobookJobs()
    {
        var client = MakeClient(HttpStatusCode.OK, $"[{SingleJobJson}]");

        var jobs = (await client.ListJobsAsync()).ToList();

        Assert.That(jobs, Has.Count.EqualTo(1));
        Assert.That(jobs[0].Id, Is.EqualTo("abc-123"));
        Assert.That(jobs[0].Status, Is.EqualTo(AudiobookJobStatus.Queued));
        Assert.That(jobs[0].EpubFilename, Is.EqualTo("book.epub"));
        Assert.That(jobs[0].VoiceSampleName, Is.EqualTo("default"));
    }

    // --- GetJobAsync ---

    [Test]
    public async Task GetJobAsync_MapsAllStatusValues()
    {
        foreach (var (json, expected) in new (string, AudiobookJobStatus)[]
        {
            (SingleJobJson.Replace("\"queued\"", "\"in_progress\""), AudiobookJobStatus.InProgress),
            (CompletedJobJson, AudiobookJobStatus.Completed),
            (FailedJobJson, AudiobookJobStatus.Failed),
            (SingleJobJson.Replace("\"queued\"", "\"cancelled\""), AudiobookJobStatus.Cancelled)
        })
        {
            var client = MakeClient(HttpStatusCode.OK, json);
            var job = await client.GetJobAsync("abc-123");
            Assert.That(job.Status, Is.EqualTo(expected));
        }
    }

    [Test]
    public async Task GetJobAsync_MapsErrorMessage()
    {
        var client = MakeClient(HttpStatusCode.OK, FailedJobJson);

        var job = await client.GetJobAsync("abc-123");

        Assert.That(job.ErrorMessage, Is.EqualTo("TTS model error"));
    }

    [Test]
    public void GetJobAsync_ThrowsKeyNotFoundOn404()
    {
        var client = MakeClient(HttpStatusCode.NotFound, "{}");

        Assert.ThrowsAsync<KeyNotFoundException>(() => client.GetJobAsync("missing"));
    }

    // --- CancelJobAsync ---

    [Test]
    public async Task CancelJobAsync_SendsDeleteRequest()
    {
        var handler = new FakeHandler(HttpStatusCode.NoContent, "", null);
        var gpuClient = new GpuServiceClient(new HttpClient(handler), MakeConfig());

        await gpuClient.CancelJobAsync("abc-123");

        Assert.That(handler.LastRequest!.Method, Is.EqualTo(HttpMethod.Delete));
        Assert.That(handler.LastRequest.RequestUri!.PathAndQuery, Does.Contain("abc-123"));
    }

    [Test]
    public void CancelJobAsync_ThrowsInvalidOperationOn409()
    {
        var client = MakeClient(HttpStatusCode.Conflict, "{}");

        Assert.ThrowsAsync<InvalidOperationException>(() => client.CancelJobAsync("abc-123"));
    }

    // --- GetFileAsync ---

    [Test]
    public async Task GetFileAsync_ReturnsStream()
    {
        var client = MakeClient(HttpStatusCode.OK, "fake-audio-bytes", "audio/mp4");

        var stream = await client.GetFileAsync("abc-123");

        Assert.That(stream, Is.Not.Null);
        Assert.That(stream.Length, Is.GreaterThan(0));
    }

    [Test]
    public void GetFileAsync_ThrowsInvalidOperationOn409()
    {
        var client = MakeClient(HttpStatusCode.Conflict, "{}");

        Assert.ThrowsAsync<InvalidOperationException>(() => client.GetFileAsync("abc-123"));
    }

    // --- Authorization header ---

    [Test]
    public async Task AllRequests_IncludeBearerToken()
    {
        var handler = new FakeHandler(HttpStatusCode.OK, "[]", null);
        var gpuClient = new GpuServiceClient(new HttpClient(handler), MakeConfig(apiKey: "secret-key"));

        await gpuClient.ListJobsAsync();

        var auth = handler.LastRequest!.Headers.Authorization;
        Assert.That(auth, Is.Not.Null);
        Assert.That(auth!.Scheme, Is.EqualTo("Bearer"));
        Assert.That(auth.Parameter, Is.EqualTo("secret-key"));
    }

    // --- ListVoiceSamplesAsync ---

    [Test]
    public async Task ListVoiceSamplesAsync_ReturnsNames()
    {
        var client = MakeClient(HttpStatusCode.OK, """["narrator","default"]""");

        var samples = (await client.ListVoiceSamplesAsync()).ToList();

        Assert.That(samples, Has.Count.EqualTo(2));
        Assert.That(samples, Contains.Item("narrator"));
    }

    // --- DeleteVoiceSampleAsync ---

    [Test]
    public void DeleteVoiceSampleAsync_ThrowsKeyNotFoundOn404()
    {
        var client = MakeClient(HttpStatusCode.NotFound, "{}");

        Assert.ThrowsAsync<KeyNotFoundException>(() => client.DeleteVoiceSampleAsync("narrator"));
    }
}
