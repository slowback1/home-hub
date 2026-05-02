using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/activities")]
public class ActivityController : ApplicationController
{
    private readonly ICrud<Activity> _activities;

    public ActivityController(ICrudFactory factory) : base(factory)
    {
        _activities = Factory.GetCrud<Activity>();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Activity>>> GetAll()
    {
        var items = await _activities.QueryAsync(_ => true);
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<Activity>> Create([FromBody] CreateActivityRequest request)
    {
        if (request.Weight < 1 || request.Weight > 5)
            return BadRequest("Weight must be between 1 and 5.");

        var activity = new Activity
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            Weight = request.Weight
        };

        var created = await _activities.CreateAsync(activity);
        return Ok(created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Activity>> Update(string id, [FromBody] UpdateActivityRequest request)
    {
        if (request.Weight < 1 || request.Weight > 5)
            return BadRequest("Weight must be between 1 and 5.");

        var existing = await _activities.GetByIdAsync(id);
        if (existing is null)
            return NotFound();

        existing.Name = request.Name;
        existing.Weight = request.Weight;

        var updated = await _activities.UpdateAsync(id, existing);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var deleted = await _activities.DeleteAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }

    public record CreateActivityRequest(string Name, int Weight);
    public record UpdateActivityRequest(string Name, int Weight);
}
