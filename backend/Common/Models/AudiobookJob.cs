using System;

namespace Common.Models;

public class AudiobookJob
{
    public string Id { get; set; } = string.Empty;
    public AudiobookJobStatus Status { get; set; }
    public string EpubFilename { get; set; } = string.Empty;
    public string VoiceSampleName { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public string? ErrorMessage { get; set; }
}
