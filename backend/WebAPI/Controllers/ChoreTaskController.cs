using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/tasks")]
public class ChoreTaskController : ApplicationController
{
    private readonly ICrud<ChoreTask> _tasks;
    private readonly ICrud<TaskCompletion> _completions;

    public ChoreTaskController(ICrudFactory factory) : base(factory)
    {
        _tasks = Factory.GetCrud<ChoreTask>();
        _completions = Factory.GetCrud<TaskCompletion>();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChoreTask>>> GetActive()
    {
        var items = await _tasks.QueryAsync(t => t.CompletedAt == null);
        return Ok(items);
    }

    [HttpGet("history")]
    public async Task<ActionResult<IEnumerable<ChoreTask>>> GetHistory()
    {
        var items = await _tasks.QueryAsync(t => t.CompletedAt != null);
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<ChoreTask>> Create([FromBody] CreateChoreTaskRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Name is required.");

        if (request.IsRecurring && (request.IntervalDays is null or < 1))
            return BadRequest("IntervalDays must be >= 1 for recurring tasks.");

        var task = new ChoreTask
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            IsRecurring = request.IsRecurring,
            IntervalDays = request.IsRecurring ? request.IntervalDays : null,
            DoDate = request.DoDate,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _tasks.CreateAsync(task);
        return Ok(created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ChoreTask>> Update(string id, [FromBody] UpdateChoreTaskRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Name is required.");

        if (request.IsRecurring && (request.IntervalDays is null or < 1))
            return BadRequest("IntervalDays must be >= 1 for recurring tasks.");

        var existing = await _tasks.GetByIdAsync(id);
        if (existing is null)
            return NotFound();

        existing.Name = request.Name;
        existing.IsRecurring = request.IsRecurring;
        existing.IntervalDays = request.IsRecurring ? request.IntervalDays : null;
        existing.DoDate = request.DoDate;

        var updated = await _tasks.UpdateAsync(id, existing);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var existing = await _tasks.GetByIdAsync(id);
        if (existing is null)
            return NotFound();

        var completions = await _completions.QueryAsync(c => c.TaskId == id);
        foreach (var completion in completions)
            await _completions.DeleteAsync(completion.Id);

        await _tasks.DeleteAsync(id);
        return NoContent();
    }

    public record CreateChoreTaskRequest(string Name, bool IsRecurring, int? IntervalDays, DateTime? DoDate);
    public record UpdateChoreTaskRequest(string Name, bool IsRecurring, int? IntervalDays, DateTime? DoDate);
}
