using Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/system-config")]
public class SystemConfigController(ISystemConfigProvider systemConfigProvider) : Controller
{
    [HttpGet("")]
    public async Task<ActionResult> GetAll()
    {
        var entries = await systemConfigProvider.GetAllAsync();
        return Ok(entries);
    }

    [HttpGet("{namespace}/{key}")]
    public async Task<ActionResult> GetOne(string @namespace, string key)
    {
        try
        {
            var entry = await systemConfigProvider.GetAsync(@namespace, key);
            return Ok(entry);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPut("{namespace}/{key}")]
    public async Task<ActionResult> Update(string @namespace, string key, [FromBody] UpdateSystemConfigRequest request)
    {
        try
        {
            var updated = await systemConfigProvider.UpdateAsync(@namespace, key, request.Value);
            return Ok(updated);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}

public record UpdateSystemConfigRequest(string Value);
