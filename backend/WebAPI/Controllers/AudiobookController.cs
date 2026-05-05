using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/audiobook")]
public class AudiobookController(IAudiobookService service) : ControllerBase
{
    // --- Jobs ---

    [HttpGet("jobs")]
    public async Task<ActionResult<IEnumerable<AudiobookJob>>> ListJobs()
    {
        var jobs = await service.ListJobsAsync();
        return Ok(jobs);
    }

    [HttpPost("jobs")]
    public async Task<ActionResult<AudiobookJob>> SubmitJob(IFormFile epubFile, [FromForm] string voiceSampleName)
    {
        using var ms = new MemoryStream();
        await epubFile.CopyToAsync(ms);
        var job = await service.SubmitJobAsync(epubFile.FileName, ms.ToArray(), voiceSampleName);
        return CreatedAtAction(nameof(GetJob), new { id = job.Id }, job);
    }

    [HttpGet("jobs/{id}")]
    public async Task<ActionResult<AudiobookJob>> GetJob(string id)
    {
        try
        {
            var job = await service.GetJobAsync(id);
            return Ok(job);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("jobs/{id}")]
    public async Task<ActionResult> CancelJob(string id)
    {
        try
        {
            await service.CancelJobAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException)
        {
            return Conflict();
        }
    }

    [HttpGet("jobs/{id}/file")]
    public async Task<ActionResult> GetFile(string id)
    {
        try
        {
            var stream = await service.GetFileAsync(id);
            var job = await service.GetJobAsync(id);
            var filename = Path.ChangeExtension(job.EpubFilename, ".m4b");
            return File(stream, "audio/mp4", filename);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (FileNotFoundException)
        {
            return StatusCode(410);
        }
        catch (InvalidOperationException)
        {
            return Conflict();
        }
    }

    [HttpDelete("jobs/{id}/file")]
    public async Task<ActionResult> DeleteFile(string id)
    {
        try
        {
            await service.DeleteFileAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException)
        {
            return Conflict();
        }
    }

    // --- Voice Samples ---

    [HttpGet("voice-samples")]
    public async Task<ActionResult<IEnumerable<string>>> ListVoiceSamples()
    {
        var samples = await service.ListVoiceSamplesAsync();
        return Ok(samples);
    }

    [HttpPost("voice-samples")]
    public async Task<ActionResult> UploadVoiceSample(IFormFile file, [FromForm] string name)
    {
        if (!Path.GetExtension(file.FileName).Equals(".wav", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Only .wav files are accepted.");

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        await service.UploadVoiceSampleAsync(name, ms.ToArray());
        return Created();
    }

    [HttpDelete("voice-samples/{name}")]
    public async Task<ActionResult> DeleteVoiceSample(string name)
    {
        try
        {
            await service.DeleteVoiceSampleAsync(name);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
