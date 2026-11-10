using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/premade-lists")]
public class PremadeListController : ApplicationController
{
    private readonly ICrud<PremadeList> _lists;
    private readonly ICrud<PremadeListItem> _memberships;

    public PremadeListController(ICrudFactory factory) : base(factory)
    {
        _lists = Factory.GetCrud<PremadeList>();
        _memberships = Factory.GetCrud<PremadeListItem>();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PremadeListResponse>>> List()
    {
        if (CurrentUserId is null) return Unauthorized();

        var lists = await _lists.QueryAsync(l => l.UserId == CurrentUserId);

        var responses = new List<PremadeListResponse>();
        foreach (var list in lists)
            responses.Add(await ToResponse(list));

        return Ok(responses);
    }

    [HttpPost]
    public async Task<ActionResult<PremadeListResponse>> Create([FromBody] CreatePremadeListRequest request)
    {
        if (CurrentUserId is null) return Unauthorized();
        if (string.IsNullOrWhiteSpace(request.Name)) return BadRequest("Name is required.");

        var list = new PremadeList
        {
            Id = Guid.NewGuid().ToString(),
            UserId = CurrentUserId,
            Name = request.Name.Trim(),
            CreatedAt = DateTime.UtcNow,
        };

        var created = await _lists.CreateAsync(list);
        return Ok(await ToResponse(created));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(string id)
    {
        if (CurrentUserId is null) return Unauthorized();

        var list = await _lists.GetByIdAsync(id);
        if (list is null || list.UserId != CurrentUserId) return NotFound();

        var memberships = await _memberships.QueryAsync(m => m.PremadeListId == list.Id);
        foreach (var membership in memberships)
            await _memberships.DeleteAsync(membership.Id);

        await _lists.DeleteAsync(id);
        return NoContent();
    }

    private async Task<PremadeListResponse> ToResponse(PremadeList list)
    {
        var memberships = await _memberships.QueryAsync(m => m.PremadeListId == list.Id);
        return new PremadeListResponse
        {
            Id = list.Id,
            UserId = list.UserId,
            Name = list.Name,
            CreatedAt = list.CreatedAt,
            SavedItemIds = [.. memberships.Select(m => m.SavedItemId)]
        };
    }

    public record CreatePremadeListRequest(string? Name);
    public record PremadeListResponse
    {
        public required string Id { get; init; }
        public required string UserId { get; init; }
        public required string Name { get; init; }
        public required DateTime CreatedAt { get; init; }
        public required string[] SavedItemIds { get; init; }
    }
}
