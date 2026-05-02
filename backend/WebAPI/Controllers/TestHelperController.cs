using Common.Interfaces;
using Common.Models;
using InMemory;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// Test-only endpoints for seeding and clearing state. Returns 404 outside of Test/E2E environments.
/// </summary>
[Route("api/test")]
public class TestHelperController(IActivityPickRepository repository, IWebHostEnvironment env)
    : ControllerBase
{
    [HttpPost("activity-picks")]
    public async Task<ActionResult> SeedActivityPick([FromBody] SeedPickRequest request)
    {
        if (!env.IsEnvironment("Test") && !env.IsEnvironment("E2E"))
            return NotFound();

        await repository.WriteAsync(new ActivityPick
        {
            ActivityName = request.ActivityName,
            PickedAt = request.PickedAt
        });
        return Ok();
    }

    [HttpDelete("activity-picks")]
    public ActionResult ClearActivityPicks()
    {
        if (!env.IsEnvironment("Test") && !env.IsEnvironment("E2E"))
            return NotFound();

        InMemoryActivityPickRepository.ClearStaticState();
        return NoContent();
    }

    public record SeedPickRequest(string ActivityName, DateTime PickedAt);
}
