using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/saved-items")]
public class SavedItemController : ApplicationController
{
    private readonly ICrud<SavedItem> _savedItems;

    public SavedItemController(ICrudFactory factory) : base(factory)
    {
        _savedItems = Factory.GetCrud<SavedItem>();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SavedItem>>> List()
    {
        if (CurrentUserId is null) return Unauthorized();
        var savedItems = await _savedItems.QueryAsync(s => s.UserId == CurrentUserId);
        return Ok(savedItems);
    }

    [HttpPost]
    public async Task<ActionResult<SavedItem>> Create([FromBody] CreateSavedItemRequest request)
    {
        if (CurrentUserId is null) return Unauthorized();
        if (string.IsNullOrWhiteSpace(request.Name)) return BadRequest("Name is required.");

        var savedItem = new SavedItem
        {
            Id = Guid.NewGuid().ToString(),
            UserId = CurrentUserId,
            Name = request.Name.Trim(),
            CreatedAt = DateTime.UtcNow,
        };
        var created = await _savedItems.CreateAsync(savedItem);
        return Ok(created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SavedItem>> Update(string id, [FromBody] UpdateSavedItemRequest request)
    {
        if (CurrentUserId is null) return Unauthorized();
        if (string.IsNullOrWhiteSpace(request.Name)) return BadRequest("Name is required.");

        var existing = await _savedItems.GetByIdAsync(id);
        if (existing is null || existing.UserId != CurrentUserId) return NotFound();

        existing.Name = request.Name.Trim();
        var updated = await _savedItems.UpdateAsync(id, existing);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        if (CurrentUserId is null) return Unauthorized();

        var existing = await _savedItems.GetByIdAsync(id);
        if (existing is null || existing.UserId != CurrentUserId) return NotFound();

        await _savedItems.DeleteAsync(id);
        return NoContent();
    }

    public record CreateSavedItemRequest(string? Name);
    public record UpdateSavedItemRequest(string? Name);
}
