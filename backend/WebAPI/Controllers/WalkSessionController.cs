using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace WebAPI.Controllers;

[Route("api/walk-sessions")]
public class WalkSessionController : ApplicationController
{
    private readonly ICrud<WalkSession> _sessions;

    public WalkSessionController(ICrudFactory factory) : base(factory)
    {
        _sessions = Factory.GetCrud<WalkSession>();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WalkSession>>> List()
    {
        var sessions = await _sessions.QueryAsync(_ => true);
        return Ok(sessions.OrderByDescending(s => s.StartedAt));
    }

    [HttpPost]
    public async Task<ActionResult<WalkSession>> Create([FromBody] CreateWalkSessionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ClientId))
            return BadRequest("ClientId is required.");

        if (request.StartedAt == default)
            return BadRequest("StartedAt is required.");

        if (request.DurationSeconds < 0)
            return BadRequest("DurationSeconds must be non-negative.");

        if (request.StepCount < 0)
            return BadRequest("StepCount must be non-negative.");

        var existing = await _sessions.GetByQueryAsync(s => s.ClientId == request.ClientId);
        if (existing != null)
            return Ok(existing);

        var session = new WalkSession
        {
            Id = Guid.NewGuid().ToString(),
            ClientId = request.ClientId,
            StartedAt = DateTime.SpecifyKind(request.StartedAt, DateTimeKind.Utc),
            DurationSeconds = request.DurationSeconds,
            StepCount = request.StepCount,
            SyncedAt = DateTime.UtcNow,
        };

        var created = await _sessions.CreateAsync(session);
        return Ok(created);
    }

    public record CreateWalkSessionRequest(
        string? ClientId,
        DateTime StartedAt,
        int DurationSeconds,
        int StepCount);
}
