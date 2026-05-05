using System.Net;
using System.Net.Http.Json;
using System.Text;
using Common.Interfaces;
using Common.Models;
using Logic.Audiobook;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace WebAPI.Integration.Tests.Controllers;

public class AudiobookControllerTests
{
    private MockAudiobookService _service = null!;
    private HttpClient _client = null!;
    private WebApplicationFactory<Program> _factory = null!;

    [SetUp]
    public void SetUp()
    {
        _service = new MockAudiobookService();
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton<IAudiobookService>(_service);
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

    // --- GET /api/audiobook/jobs ---

    [Test]
    public async Task ListJobs_Returns200WithEmptyList_WhenNoJobs()
    {
        var response = await _client.GetAsync("/api/audiobook/jobs");
        var jobs = await response.Content.ReadFromJsonAsync<List<AudiobookJob>>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(jobs, Is.Empty);
    }

    [Test]
    public async Task ListJobs_Returns200WithJobs_AfterSubmission()
    {
        await _service.SubmitJobAsync("book.epub", [], "default");

        var response = await _client.GetAsync("/api/audiobook/jobs");
        var jobs = await response.Content.ReadFromJsonAsync<List<AudiobookJob>>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(jobs, Has.Count.EqualTo(1));
        Assert.That(jobs![0].EpubFilename, Is.EqualTo("book.epub"));
    }

    // --- POST /api/audiobook/jobs ---

    [Test]
    public async Task SubmitJob_Returns201WithJob_ForValidEpub()
    {
        var form = new MultipartFormDataContent();
        form.Add(new ByteArrayContent(Encoding.UTF8.GetBytes("fake epub content")), "epubFile", "book.epub");
        form.Add(new StringContent("default"), "voiceSampleName");

        var response = await _client.PostAsync("/api/audiobook/jobs", form);
        var job = await response.Content.ReadFromJsonAsync<AudiobookJob>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        Assert.That(job!.EpubFilename, Is.EqualTo("book.epub"));
        Assert.That(job.Status, Is.EqualTo(AudiobookJobStatus.Queued));
    }

    // --- GET /api/audiobook/jobs/{id} ---

    [Test]
    public async Task GetJob_Returns200WithJob_WhenExists()
    {
        var submitted = await _service.SubmitJobAsync("book.epub", [], "default");

        var response = await _client.GetAsync($"/api/audiobook/jobs/{submitted.Id}");
        var job = await response.Content.ReadFromJsonAsync<AudiobookJob>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(job!.Id, Is.EqualTo(submitted.Id));
    }

    [Test]
    public async Task GetJob_Returns404_ForUnknownId()
    {
        var response = await _client.GetAsync("/api/audiobook/jobs/unknown-id");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    // --- DELETE /api/audiobook/jobs/{id} ---

    [Test]
    public async Task CancelJob_Returns204_ForQueuedJob()
    {
        var submitted = await _service.SubmitJobAsync("book.epub", [], "default");

        var response = await _client.DeleteAsync($"/api/audiobook/jobs/{submitted.Id}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task CancelJob_Returns404_ForUnknownId()
    {
        var response = await _client.DeleteAsync("/api/audiobook/jobs/unknown-id");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task CancelJob_Returns409_ForCompletedJob()
    {
        var submitted = await _service.SubmitJobAsync("book.epub", [], "default");
        await _service.ForceStatusAsync(submitted.Id, AudiobookJobStatus.Completed);

        var response = await _client.DeleteAsync($"/api/audiobook/jobs/{submitted.Id}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
    }

    // --- GET /api/audiobook/jobs/{id}/file ---

    [Test]
    public async Task GetFile_Returns200WithStream_ForCompletedJob()
    {
        var submitted = await _service.SubmitJobAsync("book.epub", [], "default");
        await _service.ForceStatusAsync(submitted.Id, AudiobookJobStatus.Completed);

        var response = await _client.GetAsync($"/api/audiobook/jobs/{submitted.Id}/file");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response.Content.Headers.ContentType!.MediaType, Is.EqualTo("audio/mp4"));
    }

    [Test]
    public async Task GetFile_Returns409_ForQueuedJob()
    {
        var submitted = await _service.SubmitJobAsync("book.epub", [], "default");

        var response = await _client.GetAsync($"/api/audiobook/jobs/{submitted.Id}/file");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
    }

    // --- DELETE /api/audiobook/jobs/{id}/file ---

    [Test]
    public async Task DeleteFile_Returns204_ForCompletedJob()
    {
        var submitted = await _service.SubmitJobAsync("book.epub", [], "default");
        await _service.ForceStatusAsync(submitted.Id, AudiobookJobStatus.Completed);

        var response = await _client.DeleteAsync($"/api/audiobook/jobs/{submitted.Id}/file");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task DeleteFile_Returns409_ForQueuedJob()
    {
        var submitted = await _service.SubmitJobAsync("book.epub", [], "default");

        var response = await _client.DeleteAsync($"/api/audiobook/jobs/{submitted.Id}/file");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
    }

    // --- GET /api/audiobook/voice-samples ---

    [Test]
    public async Task ListVoiceSamples_Returns200WithList()
    {
        var response = await _client.GetAsync("/api/audiobook/voice-samples");
        var samples = await response.Content.ReadFromJsonAsync<List<string>>();

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(samples, Contains.Item("default"));
    }

    // --- POST /api/audiobook/voice-samples ---

    [Test]
    public async Task UploadVoiceSample_Returns201_ForWavFile()
    {
        var form = new MultipartFormDataContent();
        form.Add(new ByteArrayContent(new byte[] { 0x52, 0x49, 0x46, 0x46 }), "file", "narrator.wav");
        form.Add(new StringContent("narrator"), "name");

        var response = await _client.PostAsync("/api/audiobook/voice-samples", form);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
    }

    [Test]
    public async Task UploadVoiceSample_Returns400_ForNonWavFile()
    {
        var form = new MultipartFormDataContent();
        form.Add(new ByteArrayContent(new byte[] { 0x01, 0x02 }), "file", "narrator.mp3");
        form.Add(new StringContent("narrator"), "name");

        var response = await _client.PostAsync("/api/audiobook/voice-samples", form);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    // --- DELETE /api/audiobook/voice-samples/{name} ---

    [Test]
    public async Task DeleteVoiceSample_Returns204_ForExistingSample()
    {
        var response = await _client.DeleteAsync("/api/audiobook/voice-samples/default");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
    }

    [Test]
    public async Task DeleteVoiceSample_Returns404_ForUnknownSample()
    {
        var response = await _client.DeleteAsync("/api/audiobook/voice-samples/nonexistent");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
