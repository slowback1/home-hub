using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Common.Interfaces;
using Common.Models;

namespace Logic.Audiobook;

public class GpuServiceClient(HttpClient httpClient, ISystemConfigProvider configProvider) : IAudiobookService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    private async Task<string> GetBaseUrlAsync()
    {
        var entry = await configProvider.GetAsync("audiobook", "url");
        return entry.Value.TrimEnd('/');
    }

    private async Task<string> GetApiKeyAsync()
    {
        var entry = await configProvider.GetSecretAsync("audiobook", "api_key");
        return entry.Value;
    }

    private async Task<HttpRequestMessage> BuildRequestAsync(HttpMethod method, string path)
    {
        var baseUrl = await GetBaseUrlAsync();
        var apiKey = await GetApiKeyAsync();
        var request = new HttpRequestMessage(method, $"{baseUrl}/{path}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        return request;
    }

    private static async Task HandleErrorResponseAsync(HttpResponseMessage response)
    {
        if (response.StatusCode == HttpStatusCode.NotFound)
            throw new KeyNotFoundException($"Resource not found: {response.RequestMessage?.RequestUri}");
        if (response.StatusCode == HttpStatusCode.Conflict)
            throw new InvalidOperationException($"Operation not allowed in current state: {response.RequestMessage?.RequestUri}");
        response.EnsureSuccessStatusCode();
    }

    private static AudiobookJob MapJob(GpuJobDto dto) => new()
    {
        Id = dto.Id,
        Status = MapStatus(dto.Status),
        EpubFilename = dto.EpubFilename,
        VoiceSampleName = dto.VoiceSampleName,
        CreatedAt = dto.CreatedAt,
        UpdatedAt = dto.UpdatedAt,
        ErrorMessage = dto.ErrorMessage
    };

    private static AudiobookJobStatus MapStatus(string status) => status switch
    {
        "queued" => AudiobookJobStatus.Queued,
        "in_progress" => AudiobookJobStatus.InProgress,
        "completed" => AudiobookJobStatus.Completed,
        "failed" => AudiobookJobStatus.Failed,
        "cancelled" => AudiobookJobStatus.Cancelled,
        _ => throw new InvalidOperationException($"Unknown job status: {status}")
    };

    public async Task<IEnumerable<AudiobookJob>> ListJobsAsync()
    {
        var request = await BuildRequestAsync(HttpMethod.Get, "jobs");
        var response = await httpClient.SendAsync(request);
        await HandleErrorResponseAsync(response);
        var json = await response.Content.ReadAsStringAsync();
        var dtos = JsonSerializer.Deserialize<List<GpuJobDto>>(json, JsonOptions) ?? [];
        return dtos.Select(MapJob);
    }

    public async Task<AudiobookJob> SubmitJobAsync(string epubFilename, byte[] epubBytes, string voiceSampleName)
    {
        var request = await BuildRequestAsync(HttpMethod.Post, "jobs");
        var form = new MultipartFormDataContent();
        form.Add(new ByteArrayContent(epubBytes) { Headers = { ContentType = new MediaTypeHeaderValue("application/epub+zip") } },
            "epub_file", epubFilename);
        form.Add(new StringContent(voiceSampleName), "voice_sample_name");
        request.Content = form;

        var response = await httpClient.SendAsync(request);
        await HandleErrorResponseAsync(response);
        var submitJson = await response.Content.ReadAsStringAsync();
        var dto = JsonSerializer.Deserialize<GpuJobDto>(submitJson, JsonOptions)
            ?? throw new InvalidOperationException("Empty response from GPU service.");
        return MapJob(dto);
    }

    public async Task<AudiobookJob> GetJobAsync(string id)
    {
        var request = await BuildRequestAsync(HttpMethod.Get, $"jobs/{Uri.EscapeDataString(id)}");
        var response = await httpClient.SendAsync(request);
        await HandleErrorResponseAsync(response);
        var getJson = await response.Content.ReadAsStringAsync();
        var dto = JsonSerializer.Deserialize<GpuJobDto>(getJson, JsonOptions)
            ?? throw new InvalidOperationException("Empty response from GPU service.");
        return MapJob(dto);
    }

    public async Task CancelJobAsync(string id)
    {
        var request = await BuildRequestAsync(HttpMethod.Delete, $"jobs/{Uri.EscapeDataString(id)}");
        var response = await httpClient.SendAsync(request);
        await HandleErrorResponseAsync(response);
    }

    public Task DeleteJobAsync(string id) => throw new NotImplementedException();

    public async Task<Stream> GetFileAsync(string id)
    {
        var request = await BuildRequestAsync(HttpMethod.Get, $"jobs/{Uri.EscapeDataString(id)}/file");
        var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        await HandleErrorResponseAsync(response);
        return await response.Content.ReadAsStreamAsync();
    }

    public async Task DeleteFileAsync(string id)
    {
        var request = await BuildRequestAsync(HttpMethod.Delete, $"jobs/{Uri.EscapeDataString(id)}/file");
        var response = await httpClient.SendAsync(request);
        await HandleErrorResponseAsync(response);
    }

    public async Task<IEnumerable<string>> ListVoiceSamplesAsync()
    {
        var request = await BuildRequestAsync(HttpMethod.Get, "voice-samples");
        var response = await httpClient.SendAsync(request);
        await HandleErrorResponseAsync(response);
        var samplesJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<string>>(samplesJson, JsonOptions) ?? [];
    }

    public async Task UploadVoiceSampleAsync(string name, byte[] wavBytes)
    {
        var request = await BuildRequestAsync(HttpMethod.Post, "voice-samples");
        var form = new MultipartFormDataContent();
        form.Add(new ByteArrayContent(wavBytes) { Headers = { ContentType = new MediaTypeHeaderValue("audio/wav") } },
            "file", $"{name}.wav");
        form.Add(new StringContent(name), "name");
        request.Content = form;

        var response = await httpClient.SendAsync(request);
        await HandleErrorResponseAsync(response);
    }

    public async Task DeleteVoiceSampleAsync(string name)
    {
        var request = await BuildRequestAsync(HttpMethod.Delete, $"voice-samples/{Uri.EscapeDataString(name)}");
        var response = await httpClient.SendAsync(request);
        await HandleErrorResponseAsync(response);
    }

    private sealed class GpuJobDto
    {
        public string Id { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string EpubFilename { get; set; } = string.Empty;
        public string VoiceSampleName { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
