using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace Logic.Audiobook;

public class MockAudiobookService : IAudiobookService
{
    private readonly ConcurrentDictionary<string, AudiobookJob> _jobs = new();
    private readonly HashSet<string> _deletedFiles = new();
    private readonly List<string> _voiceSamples = ["default"];
    private readonly object _voiceSamplesLock = new();

    // Minimal valid M4B/MP4 byte sequence (ftyp box)
    private static readonly byte[] FakeM4bBytes =
    [
        0x00, 0x00, 0x00, 0x18, 0x66, 0x74, 0x79, 0x70,
        0x4d, 0x34, 0x42, 0x20, 0x00, 0x00, 0x00, 0x00,
        0x4d, 0x34, 0x42, 0x20, 0x4d, 0x34, 0x41, 0x20
    ];

    public Task<IEnumerable<AudiobookJob>> ListJobsAsync()
    {
        var ordered = _jobs.Values.OrderBy(j => j.CreatedAt);
        return Task.FromResult<IEnumerable<AudiobookJob>>(ordered);
    }

    public Task<AudiobookJob> SubmitJobAsync(string epubFilename, byte[] epubBytes, string voiceSampleName)
    {
        var job = new AudiobookJob
        {
            Id = Guid.NewGuid().ToString(),
            Status = AudiobookJobStatus.Queued,
            EpubFilename = epubFilename,
            VoiceSampleName = voiceSampleName,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
        _jobs[job.Id] = job;
        _ = AdvanceJobAsync(job.Id, epubFilename);
        return Task.FromResult(job);
    }

    public Task<AudiobookJob> GetJobAsync(string id)
    {
        if (!_jobs.TryGetValue(id, out var job))
            throw new KeyNotFoundException($"Job {id} not found");
        return Task.FromResult(job);
    }

    public Task CancelJobAsync(string id)
    {
        if (!_jobs.TryGetValue(id, out var job))
            throw new KeyNotFoundException($"Job {id} not found");

        if (job.Status is AudiobookJobStatus.Completed or AudiobookJobStatus.Failed or AudiobookJobStatus.Cancelled)
            throw new InvalidOperationException($"Cannot cancel job in {job.Status} state");

        job.Status = AudiobookJobStatus.Cancelled;
        job.UpdatedAt = DateTimeOffset.UtcNow;
        return Task.CompletedTask;
    }

    public Task<Stream> GetFileAsync(string id)
    {
        if (!_jobs.TryGetValue(id, out var job))
            throw new KeyNotFoundException($"Job {id} not found");

        if (job.Status != AudiobookJobStatus.Completed)
            throw new InvalidOperationException($"Job {id} is not completed");

        if (_deletedFiles.Contains(id))
            throw new FileNotFoundException($"File for job {id} has been deleted");

        return Task.FromResult<Stream>(new MemoryStream(FakeM4bBytes));
    }

    public Task DeleteFileAsync(string id)
    {
        if (!_jobs.TryGetValue(id, out var job))
            throw new KeyNotFoundException($"Job {id} not found");

        if (job.Status != AudiobookJobStatus.Completed)
            throw new InvalidOperationException($"Job {id} is not completed");

        _deletedFiles.Add(id);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<string>> ListVoiceSamplesAsync()
    {
        lock (_voiceSamplesLock)
        {
            return Task.FromResult<IEnumerable<string>>(_voiceSamples.ToList());
        }
    }

    public Task UploadVoiceSampleAsync(string name, byte[] wavBytes)
    {
        lock (_voiceSamplesLock)
        {
            _voiceSamples.Add(name);
        }
        return Task.CompletedTask;
    }

    public Task DeleteVoiceSampleAsync(string name)
    {
        lock (_voiceSamplesLock)
        {
            if (!_voiceSamples.Remove(name))
                throw new KeyNotFoundException($"Voice sample '{name}' not found");
        }
        return Task.CompletedTask;
    }

    public Task ResetAsync()
    {
        _jobs.Clear();
        _deletedFiles.Clear();
        lock (_voiceSamplesLock)
        {
            _voiceSamples.Clear();
            _voiceSamples.Add("default");
        }
        return Task.CompletedTask;
    }

    // Internal test helper — allows tests to force a job to a specific status
    public Task ForceStatusAsync(string id, AudiobookJobStatus status)
    {
        if (!_jobs.TryGetValue(id, out var job))
            throw new KeyNotFoundException($"Job {id} not found");
        job.Status = status;
        job.UpdatedAt = DateTimeOffset.UtcNow;
        return Task.CompletedTask;
    }

    private async Task AdvanceJobAsync(string id, string epubFilename)
    {
        await Task.Delay(1000);

        if (!_jobs.TryGetValue(id, out var job) || job.Status != AudiobookJobStatus.Queued)
            return;

        job.Status = AudiobookJobStatus.InProgress;
        job.UpdatedAt = DateTimeOffset.UtcNow;

        await Task.Delay(3000);

        if (!_jobs.TryGetValue(id, out job) || job.Status != AudiobookJobStatus.InProgress)
            return;

        if (epubFilename.Contains("fail", StringComparison.OrdinalIgnoreCase))
        {
            job.Status = AudiobookJobStatus.Failed;
            job.ErrorMessage = "Conversion failed: mock failure triggered by filename";
        }
        else
        {
            job.Status = AudiobookJobStatus.Completed;
        }
        job.UpdatedAt = DateTimeOffset.UtcNow;
    }
}
