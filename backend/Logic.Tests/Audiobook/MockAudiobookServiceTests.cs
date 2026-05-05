using Logic.Audiobook;
using Common.Models;

namespace Logic.Tests.Audiobook;

public class MockAudiobookServiceTests
{
    private MockAudiobookService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _service = new MockAudiobookService();
    }

    // --- SubmitJobAsync ---

    [Test]
    public async Task SubmitJobAsync_ReturnsJobInQueuedState()
    {
        var job = await _service.SubmitJobAsync("book.epub", [], "default");

        Assert.That(job.Status, Is.EqualTo(AudiobookJobStatus.Queued));
        Assert.That(job.EpubFilename, Is.EqualTo("book.epub"));
        Assert.That(job.VoiceSampleName, Is.EqualTo("default"));
        Assert.That(job.Id, Is.Not.Empty);
    }

    [Test]
    public async Task SubmitJobAsync_AssignsCreatedAt()
    {
        var before = DateTimeOffset.UtcNow;
        var job = await _service.SubmitJobAsync("book.epub", [], "default");
        var after = DateTimeOffset.UtcNow;

        Assert.That(job.CreatedAt, Is.GreaterThanOrEqualTo(before));
        Assert.That(job.CreatedAt, Is.LessThanOrEqualTo(after));
    }

    // --- ListJobsAsync ---

    [Test]
    public async Task ListJobsAsync_ReturnsJobsOldestFirst()
    {
        await _service.SubmitJobAsync("first.epub", [], "default");
        await Task.Delay(10);
        await _service.SubmitJobAsync("second.epub", [], "default");

        var jobs = (await _service.ListJobsAsync()).ToList();

        Assert.That(jobs[0].EpubFilename, Is.EqualTo("first.epub"));
        Assert.That(jobs[1].EpubFilename, Is.EqualTo("second.epub"));
    }

    [Test]
    public async Task ListJobsAsync_ReturnsEmptyListWhenNoJobs()
    {
        var jobs = await _service.ListJobsAsync();

        Assert.That(jobs, Is.Empty);
    }

    // --- GetJobAsync ---

    [Test]
    public async Task GetJobAsync_ReturnsJobById()
    {
        var submitted = await _service.SubmitJobAsync("book.epub", [], "default");

        var found = await _service.GetJobAsync(submitted.Id);

        Assert.That(found.Id, Is.EqualTo(submitted.Id));
    }

    [Test]
    public void GetJobAsync_ThrowsKeyNotFoundForUnknownId()
    {
        Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetJobAsync("unknown-id"));
    }

    // --- CancelJobAsync ---

    [Test]
    public async Task CancelJobAsync_SetsStatusToCancelledForQueuedJob()
    {
        var job = await _service.SubmitJobAsync("book.epub", [], "default");

        await _service.CancelJobAsync(job.Id);

        var updated = await _service.GetJobAsync(job.Id);
        Assert.That(updated.Status, Is.EqualTo(AudiobookJobStatus.Cancelled));
    }

    [Test]
    public async Task CancelJobAsync_ThrowsForCompletedJob()
    {
        var job = await _service.SubmitJobAsync("book.epub", [], "default");
        await _service.ForceStatusAsync(job.Id, AudiobookJobStatus.Completed);

        Assert.ThrowsAsync<InvalidOperationException>(() => _service.CancelJobAsync(job.Id));
    }

    [Test]
    public async Task CancelJobAsync_ThrowsForFailedJob()
    {
        var job = await _service.SubmitJobAsync("book.epub", [], "default");
        await _service.ForceStatusAsync(job.Id, AudiobookJobStatus.Failed);

        Assert.ThrowsAsync<InvalidOperationException>(() => _service.CancelJobAsync(job.Id));
    }

    [Test]
    public async Task CancelJobAsync_ThrowsForAlreadyCancelledJob()
    {
        var job = await _service.SubmitJobAsync("book.epub", [], "default");
        await _service.CancelJobAsync(job.Id);

        Assert.ThrowsAsync<InvalidOperationException>(() => _service.CancelJobAsync(job.Id));
    }

    // --- State progression ---

    [Test]
    public async Task Job_ProgressesToCompletedForNormalFilename()
    {
        var job = await _service.SubmitJobAsync("book.epub", [], "default");

        await Task.Delay(6000);

        var updated = await _service.GetJobAsync(job.Id);
        Assert.That(updated.Status, Is.EqualTo(AudiobookJobStatus.Completed));
    }

    [Test]
    public async Task Job_ProgressesToFailedWhenFilenameContainsFail()
    {
        var job = await _service.SubmitJobAsync("fail-this-book.epub", [], "default");

        await Task.Delay(6000);

        var updated = await _service.GetJobAsync(job.Id);
        Assert.That(updated.Status, Is.EqualTo(AudiobookJobStatus.Failed));
        Assert.That(updated.ErrorMessage, Is.Not.Empty);
    }

    // --- GetFileAsync ---

    [Test]
    public async Task GetFileAsync_ReturnsStreamForCompletedJob()
    {
        var job = await _service.SubmitJobAsync("book.epub", [], "default");
        await _service.ForceStatusAsync(job.Id, AudiobookJobStatus.Completed);

        var stream = await _service.GetFileAsync(job.Id);

        Assert.That(stream, Is.Not.Null);
        Assert.That(stream.Length, Is.GreaterThan(0));
    }

    [Test]
    public async Task GetFileAsync_ThrowsForQueuedJob()
    {
        var job = await _service.SubmitJobAsync("book.epub", [], "default");

        Assert.ThrowsAsync<InvalidOperationException>(() => _service.GetFileAsync(job.Id));
    }

    [Test]
    public async Task GetFileAsync_ThrowsFileNotFoundAfterFileDeletion()
    {
        var job = await _service.SubmitJobAsync("book.epub", [], "default");
        await _service.ForceStatusAsync(job.Id, AudiobookJobStatus.Completed);
        await _service.DeleteFileAsync(job.Id);

        Assert.ThrowsAsync<FileNotFoundException>(() => _service.GetFileAsync(job.Id));
    }

    // --- DeleteFileAsync ---

    [Test]
    public async Task DeleteFileAsync_SucceedsForCompletedJob()
    {
        var job = await _service.SubmitJobAsync("book.epub", [], "default");
        await _service.ForceStatusAsync(job.Id, AudiobookJobStatus.Completed);

        Assert.DoesNotThrowAsync(() => _service.DeleteFileAsync(job.Id));
    }

    [Test]
    public async Task DeleteFileAsync_ThrowsForNonCompletedJob()
    {
        var job = await _service.SubmitJobAsync("book.epub", [], "default");

        Assert.ThrowsAsync<InvalidOperationException>(() => _service.DeleteFileAsync(job.Id));
    }

    // --- Voice samples ---

    [Test]
    public async Task ListVoiceSamplesAsync_ReturnsPreseededDefault()
    {
        var samples = await _service.ListVoiceSamplesAsync();

        Assert.That(samples, Contains.Item("default"));
    }

    [Test]
    public async Task UploadVoiceSampleAsync_AddsNewSample()
    {
        await _service.UploadVoiceSampleAsync("narrator", []);

        var samples = await _service.ListVoiceSamplesAsync();
        Assert.That(samples, Contains.Item("narrator"));
    }

    [Test]
    public async Task DeleteVoiceSampleAsync_RemovesSample()
    {
        await _service.UploadVoiceSampleAsync("narrator", []);

        await _service.DeleteVoiceSampleAsync("narrator");

        var samples = await _service.ListVoiceSamplesAsync();
        Assert.That(samples, Does.Not.Contain("narrator"));
    }

    [Test]
    public void DeleteVoiceSampleAsync_ThrowsKeyNotFoundForUnknownSample()
    {
        Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteVoiceSampleAsync("nonexistent"));
    }
}
