using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Common.Models;

namespace Common.Interfaces;

public interface IAudiobookService
{
    Task<IEnumerable<AudiobookJob>> ListJobsAsync();
    Task<AudiobookJob> SubmitJobAsync(string epubFilename, byte[] epubBytes, string voiceSampleName);
    Task<AudiobookJob> GetJobAsync(string id);
    Task CancelJobAsync(string id);
    Task DeleteJobAsync(string id);
    Task<Stream> GetFileAsync(string id);
    Task DeleteFileAsync(string id);
    Task<IEnumerable<string>> ListVoiceSamplesAsync();
    Task UploadVoiceSampleAsync(string name, byte[] wavBytes);
    Task DeleteVoiceSampleAsync(string name);
}
