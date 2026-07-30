using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/wheels")]
public class WheelController : ApplicationController
{
    private readonly ICrud<Wheel> _wheels;

    public WheelController(ICrudFactory factory) : base(factory)
    {
        _wheels = Factory.GetCrud<Wheel>();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Wheel>>> List()
    {
        var wheels = await _wheels.QueryAsync(_ => true);
        return Ok(wheels);
    }

    [HttpPost]
    public async Task<ActionResult<Wheel>> Create([FromBody] CreateWheelRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Name is required.");

        var wheel = new Wheel
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name.Trim(),
            Items = request.Items ?? string.Empty,
            CreatedAt = DateTime.UtcNow,
        };

        var created = await _wheels.CreateAsync(wheel);
        return Ok(created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Wheel>> Update(string id, [FromBody] UpdateWheelRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Name is required.");

        var existing = await _wheels.GetByIdAsync(id);
        if (existing is null)
            return NotFound();

        existing.Name = request.Name.Trim();
        existing.Items = request.Items ?? string.Empty;

        var updated = await _wheels.UpdateAsync(id, existing);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var existing = await _wheels.GetByIdAsync(id);
        if (existing is null)
            return NotFound();

        await _wheels.DeleteAsync(id);
        return NoContent();
    }

    public record CreateWheelRequest(string? Name, string? Items);
    public record UpdateWheelRequest(string? Name, string? Items);
}
