using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/activity")]
public class ActivityPickController(ICrudFactory factory, IActivityPickRepository repository) : ApplicationController(factory)
{
    private readonly IActivityPickRepository _repository = repository;

    [HttpGet("current")]
    public async Task<ActionResult<ActivityPick>> GetCurrent()
    {
        if (CurrentUserId is null)
            return Unauthorized();

        var pick = await _repository.GetCurrentAsync(CurrentUserId);
        if (pick is null)
            return NoContent();

        return Ok(pick);
    }

    [HttpGet("history")]
    public async Task<ActionResult<IEnumerable<ActivityPick>>> GetHistory()
    {
        if (CurrentUserId is null)
            return Unauthorized();

        var to = DateTime.UtcNow;
        var from = to.AddDays(-7);
        var picks = await _repository.GetRangeAsync(CurrentUserId, from, to);
        return Ok(picks);
    }
}
