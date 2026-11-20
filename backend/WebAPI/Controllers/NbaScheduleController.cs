using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/nba")]
public class NbaScheduleController(INbaScheduleRepository repository) : ControllerBase
{
    private readonly INbaScheduleRepository _repository = repository;

    [HttpGet("schedule")]
    public async Task<ActionResult<IEnumerable<NbaGame>>> GetSchedule(
        [FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var rangeStart = from ?? DateTime.UtcNow.Date;
        var rangeEnd = to ?? DateTime.UtcNow.Date.AddDays(3);

        var games = await _repository.GetRangeAsync(rangeStart, rangeEnd);
        return Ok(games.OrderBy(g => g.StartAt));
    }
}
