using System.Collections.Generic;
using Common.Interfaces;
using Logic.FeatureFlags;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/feature-flags")]
public class FeatureFlagController(
    IFeatureFlagProvider provider,
    UpdateFeatureFlagUseCase updateUseCase) : Controller
{
    [HttpGet("")]
    public async Task<ActionResult> GetAll()
    {
        var flags = await provider.GetFeatureFlags();
        return Ok(flags);
    }

    [HttpPatch("{name}")]
    public async Task<ActionResult> Toggle(string name, [FromBody] ToggleFeatureFlagRequest request)
    {
        try
        {
            var updated = await updateUseCase.ExecuteAsync(name, request.IsEnabled);
            return Ok(updated);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}

public record ToggleFeatureFlagRequest(bool IsEnabled);
