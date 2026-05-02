using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/activity")]
public class ActivityPickController : ControllerBase
{
    private readonly IActivityPickRepository _repository;

    public ActivityPickController(IActivityPickRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("current")]
    public async Task<ActionResult<ActivityPick>> GetCurrent()
    {
        var pick = await _repository.GetCurrentAsync();
        if (pick is null)
            return NoContent();

        return Ok(pick);
    }

    [HttpGet("history")]
    public async Task<ActionResult<IEnumerable<ActivityPick>>> GetHistory()
    {
        var to = DateTime.UtcNow;
        var from = to.AddDays(-7);
        var picks = await _repository.GetRangeAsync(from, to);
        return Ok(picks);
    }
}
