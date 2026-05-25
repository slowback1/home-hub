using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/bookmarks")]
public class BookmarkController : ApplicationController
{
    private readonly ICrud<Bookmark> _bookmarks;

    public BookmarkController(ICrudFactory factory) : base(factory)
    {
        _bookmarks = Factory.GetCrud<Bookmark>();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Bookmark>>> List()
    {
        var items = await _bookmarks.QueryAsync(_ => true);
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<Bookmark>> Create([FromBody] CreateBookmarkRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Url))
            return BadRequest("Url is required.");

        var bookmark = new Bookmark
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name ?? string.Empty,
            Url = request.Url,
            Description = request.Description,
            Starred = false,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _bookmarks.CreateAsync(bookmark);
        return Ok(created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Bookmark>> Update(string id, [FromBody] UpdateBookmarkRequest request)
    {
        var existing = await _bookmarks.GetByIdAsync(id);
        if (existing is null)
            return NotFound();

        existing.Name = request.Name ?? string.Empty;
        existing.Url = request.Url ?? existing.Url;
        existing.Description = request.Description;

        var updated = await _bookmarks.UpdateAsync(id, existing);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        var existing = await _bookmarks.GetByIdAsync(id);
        if (existing is null)
            return NotFound();

        await _bookmarks.DeleteAsync(id);
        return NoContent();
    }

    [HttpPatch("{id}/star")]
    public async Task<ActionResult<Bookmark>> ToggleStar(string id)
    {
        var existing = await _bookmarks.GetByIdAsync(id);
        if (existing is null)
            return NotFound();

        existing.Starred = !existing.Starred;
        var updated = await _bookmarks.UpdateAsync(id, existing);
        return Ok(updated);
    }

    public record CreateBookmarkRequest(string? Url, string? Name, string? Description);
    public record UpdateBookmarkRequest(string? Url, string? Name, string? Description);
}
